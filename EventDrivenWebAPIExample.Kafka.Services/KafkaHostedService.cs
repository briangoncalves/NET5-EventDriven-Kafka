using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventDrivenWebAPIExample.Kafka.Infrastructure.Interface;
using EventDrivenWebAPIExample.Kafka.Services.Interface;
using Kafka.Public;
using Kafka.Public.Loggers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventDrivenWebAPIExample.Kafka.Services
{
    public class KafkaHostedService : IHostedService
	{
        private readonly object _lockObject = new();
        private readonly ClusterClient _cluster;
        private readonly IKafkaConfig _kafkaConfig;
        private readonly IMappedServices _mappedServices;
        private readonly IServiceProvider _provider;
        private readonly IKafkaConsumer _consumer;
        private readonly ILogger<KafkaHostedService> _logger;

        public KafkaHostedService(IKafkaConfig kafkaConfig, IMappedServices mappedServices, IServiceProvider provider,
        	IKafkaConsumer consumer, 
			ILogger<KafkaHostedService> logger)
        {
        	_logger = logger;
        	_kafkaConfig = kafkaConfig;
        	_cluster = new ClusterClient(new Configuration
        	{
        		Seeds = _kafkaConfig.Host
        	}, new ConsoleLogger());

        	_mappedServices = mappedServices;
        	_provider = provider;
        	_provider.CreateScope();
        	_consumer = consumer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
		{
            _logger.LogInformation($"KafkaHostedService - StartAsync() invoked at {DateTime.Now:G}");

            if (Monitor.TryEnter(_lockObject))
            {
                Guid transactionID = Guid.NewGuid();
                try
                {

                    _kafkaConfig.Listeners.ToList().ForEach(listener =>
                    {
                        _cluster.Subscribe(listener.ConsumerGroupId, listener.Topics,
                        new ConsumerGroupConfiguration
                        {
                            AutoCommitEveryMs = 5000
                        });
                        _cluster.MessageReceived += ClusterMessageReceived;
                    });

                }
                catch (Exception ex)
                {
                    _logger.LogError($"KafkaHostedService - StartAsync() threw Exception{Environment.NewLine}{ex}");
                }
            }

            return Task.CompletedTask;
		}

        private void ClusterMessageReceived(RawKafkaRecord record)
        {
            try
            {
                _consumer.Consume(record, _mappedServices);
            }
            catch (Exception ex)
            {
                _logger.LogError($"KafkaHostedService - ClusterMessageReceived() threw Exception{Environment.NewLine}{ex}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
		{
            _logger.LogInformation($"KafkaHostedService - StopAsync() invoked at {DateTime.Now:G}");

            _cluster?.Dispose();
            return Task.CompletedTask;
		}
	}
}
