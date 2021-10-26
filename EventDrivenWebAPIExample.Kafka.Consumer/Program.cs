using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EventDrivenWebAPIExample.Kafka.Consumer
{
    class Program
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

					var connectionString = Environment.GetEnvironmentVariable("COMPANYAPI_CONNECTION_STRING");
					services.AddDbContext<ICompanyDbContext, CompanyDbContext>(
						options => { options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)); },
						ServiceLifetime.Scoped);

					services.AddScoped<IDepartmentService, DepartmentService>();
					services.AddScoped<IKafkaConsumer, KafkaConsumer>();
					services.AddScoped<ILoggerStateFactory, LoggerStateFactory>();
					AddMappedServices<Department, DepartmentKafkaExecutor>(services);

					services.AddScoped<IAssemblyService, AssemblyService>();
					services.AddSingleton<IMappedServices>(mp => new MappedServices() { Services = mappedServices });
					services.AddHostedService<KafkaHostedService>();

					ConfigureLogging(services);
				});
		}

		private static void ConfigureLogging(IServiceCollection services)
		{
			/* Switching to using "Serilog" log provider for everything
                NOTE: Call to ClearProviders() is what turns off the default Console Logging

                Output to the Console is now controlled by the WriteTo format below
                DevOps can control the Log output with environment variables
                    LOG_MINIMUMLEVEL - values like INFORMATION, WARNING, ERROR
                    LOG_JSON - true means to output log to console in JSON format
            */
			LogLevel level = LogLevel.None;
			LoggingLevelSwitch serilogLevel = new()
			{
				MinimumLevel = LogEventLevel.Information
			};

			if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LOG_MINIMUMLEVEL")))
			{
				Enum.TryParse(Environment.GetEnvironmentVariable("LOG_MINIMUMLEVEL"), out level);
				LogEventLevel eventLevel = LogEventLevel.Information;
				Enum.TryParse(Environment.GetEnvironmentVariable("LOG_MINIMUMLEVEL"), out eventLevel);
				serilogLevel.MinimumLevel = eventLevel;
			}

			bool useJson = Environment.GetEnvironmentVariable("LOG_JSON")?.ToLower() == "true";

			var config = new LoggerConfiguration()
				.Enrich.FromLogContext()
				.ReadFrom.Configuration(Configuration);

			if (useJson)
				config.WriteTo.Console(new ElasticsearchJsonFormatter());
			else
				config.WriteTo.Console(outputTemplate: "[{Timestamp:MM-dd-yyyy HH:mm:ss.SSS} {Level:u3}] {Message:lj} {TransactionID}{NewLine}{Exception}", theme: SystemConsoleTheme.Literate);

			if (level != LogLevel.None)
				config.MinimumLevel.ControlledBy(serilogLevel);

			Log.Logger = config.CreateLogger();

			services.AddLogging(lb =>
			{
				lb.ClearProviders();
				lb.AddSerilog();
				lb.AddDebug(); //Write to VS Output window (controlled by appsettings "Logging" section)
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
}
