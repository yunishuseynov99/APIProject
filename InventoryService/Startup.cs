using Common.MongoDB;
using InventoryService.Clients;
using InventoryService.Entities;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Timeout;

namespace InventoryService
{
    public class Startup
    {
        private const string AllowedOriginSetting = "AllowedOrigin";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMongo()
                .AddMongoRepository<InventoryItem>("inventoryitems");

            Random jitterer = new Random();

            services.AddHttpClient<CatalogClient>(c =>
            {
                c.BaseAddress = new Uri("https://localhost:5001");
            })
                .AddTransientHttpErrorPolicy(b => b.Or<TimeoutRejectedException>().WaitAndRetryAsync(
                    5,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                    + TimeSpan.FromMicroseconds(jitterer.Next(0, 1000)),
                    onRetry: (outcome, timespan, retryAttempt) =>
                    {
                        var serviceProvider = services.BuildServiceProvider();
                        serviceProvider.GetService<ILogger<CatalogClient>>()?
                        .LogWarning($"Delaying for {timespan.TotalSeconds} seconds, doing a retry {retryAttempt}");
                    }
                 ))
                .AddTransientHttpErrorPolicy(b => b.Or<TimeoutRejectedException>().CircuitBreakerAsync(
                    3,
                    TimeSpan.FromMicroseconds(15),
                    onBreak: (outcome, timespan) =>
                    {
                        var serviceProvider = services.BuildServiceProvider();
                        serviceProvider.GetService<ILogger<CatalogClient>>()?
                        .LogWarning($"Opening the circuit for {timespan.TotalSeconds} seconds...");
                    },
                    onReset: () =>
                    {
                        var serviceProvider = services.BuildServiceProvider();
                        serviceProvider.GetService<ILogger<CatalogClient>>()?
                        .LogWarning($"Closing the circuit...");
                    }
                 ))
                .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));

            services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "InventoryService", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "InventoryService v1"));

                app.UseCors(builder =>
                {
                    builder.WithOrigins(Configuration[AllowedOriginSetting])
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
