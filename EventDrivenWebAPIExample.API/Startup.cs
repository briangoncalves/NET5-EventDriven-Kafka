using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using EventDrivenWebAPIExample.Domain;
using EventDrivenWebAPIExample.Infrastructure.Interface;
using EventDrivenWebAPIExample.Infrastructure;
using EventDrivenWebAPIExample.Services.Interface;
using EventDrivenWebAPIExample.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using EventDrivenWebAPIExample.Infrastructure.Mongo;
using EventDrivenWebAPIExample.Infrastructure.Interface.Mongo;

namespace EventDrivenWebAPIExample.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            services.AddScoped<IKafkaMessengerService, KafkaMessengerService>();
            services.AddScoped<IKafkaConfig, KafkaConfig>();

            var dbSettings = new MongoDBSettings()
            {
                ConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING"),
                DatabaseName = Environment.GetEnvironmentVariable("MONGO_DB_NAME")
            };
            services.AddSingleton<IMongoDBSettings>(_ => dbSettings);
            services.AddTransient<IMongoClient>(_ => dbSettings.CreateClient());
            services.AddScoped<IConnectionThrottlingPipeline, ConnectionThrottlingPipeline>();
            services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));

            string kafkaHost = Environment.GetEnvironmentVariable("KAFKA_HOST");
            string kafkaTopic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
            var senders = new List<KafkaSender>
            {
                new KafkaSender
                {
                    Topic = kafkaTopic
                }
            };
            services.AddSingleton<IKafkaConfig>(kc =>
                new KafkaConfig() { Host = kafkaHost, Sender = senders }
            );
            services.AddScoped<IMovieService, MovieService>();
            services.AddScoped(p => new ProducerBuilder<string, string>(new ProducerConfig
            {
                BootstrapServers = kafkaHost
            }).Build());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EventDrivenWebAPIExample.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EventDrivenWebAPIExample.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
