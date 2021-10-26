using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using EventDrivenWebAPIExample.Kafka.Domain;
using EventDrivenWebAPIExample.Kafka.Domain.Mongo;
using EventDrivenWebAPIExample.Kafka.Infrastructure;
using EventDrivenWebAPIExample.Kafka.Infrastructure.Interface;
using EventDrivenWebAPIExample.Kafka.Infrastructure.Interface.Mongo;
using EventDrivenWebAPIExample.Kafka.Infrastructure.Mongo;
using EventDrivenWebAPIExample.Kafka.Services;
using EventDrivenWebAPIExample.Kafka.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace EventDrivenWebAPIExample.Kafka.ConsumerApp
{
    public class Program
    {
		private readonly static Dictionary<string, MessageTypeServiceMap> mappedServices = new();
		public static IConfiguration Configuration { get; } = new ConfigurationBuilder().Build();

		private static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		private static IHostBuilder CreateHostBuilder(string[] args)
		{
			string KafkaHost = Environment.GetEnvironmentVariable("KAFKA_HOST");
			string KafkaConsumerGroup = Environment.GetEnvironmentVariable("KAFKA_CONSUMER_GROUP");
			string KafkaTopic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");

			return Host.CreateDefaultBuilder(args)
				.ConfigureServices((context, services) =>
				{

					var dbSettings = new MongoDBSettings()
					{
						ConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING"),
						DatabaseName = Environment.GetEnvironmentVariable("MONGO_DB_NAME")
					};
					services.AddSingleton<IMongoDBSettings>(_ => dbSettings);
					services.AddTransient<IMongoClient>(_ => dbSettings.CreateClient());
					services.AddScoped<IConnectionThrottlingPipeline, ConnectionThrottlingPipeline>();
					services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));

					var listerners = new List<KafkaListener>
					{
						new KafkaListener
						{
							ConsumerGroupId = KafkaConsumerGroup,
							Topics = KafkaTopic.Split(",")
						}
					};
					services.AddSingleton<IKafkaConfig>(kc =>
						new KafkaConfig() { Host = KafkaHost, Listeners = listerners }
					);

					services.AddSingleton<IKafkaConsumer, KafkaConsumer>();
					AddMappedServices<Movie, MovieKafkaExecutor>(services);
					services.AddSingleton<IMappedServices>(mp => new MappedServices() { Services = mappedServices });
					services.AddLogging();
					services.AddHostedService<KafkaHostedService>();
				});
		}

		private static void AddMappedServices<TModel,
			[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
		TExecutor>(IServiceCollection services)
			where TModel : class
			where TExecutor : class, IKafkaExecutor<TModel>
		{
			services.AddScoped<IKafkaExecutor<TModel>, TExecutor>();
			mappedServices.Add(typeof(TModel).Name, new MessageTypeServiceMap
			{
				MessageType = typeof(TModel),
				ServiceType = typeof(IKafkaExecutor<TModel>)
			});
		}
	}
}