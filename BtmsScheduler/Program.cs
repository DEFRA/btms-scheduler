
using System.Configuration;
using BtmsScheduler.Utils;
using BtmsScheduler.Utils.Http;
using BtmsScheduler.Utils.Logging;
using BtmsScheduler.Utils.Mongo;
using FluentValidation;
using Serilog;
using Serilog.Core;
using System.Diagnostics.CodeAnalysis;using System.Net;
using BtmsScheduler;
using Hangfire;
using Hangfire.InMemory;
using Microsoft.Extensions.Options;

//-------- Configure the WebApplication builder------------------//

var app = CreateWebApplication(args);
await app.RunAsync();


[ExcludeFromCodeCoverage]
static WebApplication CreateWebApplication(string[] args)
{
   var _builder = WebApplication.CreateBuilder(args);

   ConfigureWebApplication(_builder);

   var _app = BuildWebApplication(_builder);

   return _app;
}

[ExcludeFromCodeCoverage]
static void ConfigureWebApplication(WebApplicationBuilder _builder)
{
   _builder.Configuration.AddEnvironmentVariables();

   var logger = ConfigureLogging(_builder);

   // Load certificates into Trust Store - Note must happen before Mongo and Http client connections
   _builder.Services.AddCustomTrustStore(logger);

   ConfigureMongoDb(_builder);
   ConfigureHangfire(_builder);
   ConfigureEndpoints(_builder);

   _builder.Services.AddHttpClient();

   // calls outside the platform should be done using the named 'proxy' http client.
   _builder.Services.AddHttpProxyClient(logger);

   _builder.Services.AddValidatorsFromAssemblyContaining<Program>();
}

[ExcludeFromCodeCoverage]
static void ConfigureHangfire(WebApplicationBuilder builder)
{
    var storage = new InMemoryStorage(new InMemoryStorageOptions { MaxExpirationTime = TimeSpan.FromHours(48) });
    
    builder.Services.AddHangfire(c => c
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSerilogLogProvider()
        // Currently only running a daily job, in future we may need mongo/redis backing
        .UseStorage(storage)
    );

    // For some reason this isn't set automatically...
    // https://stackoverflow.com/questions/52113361/jobstorage-current-property-value-has-not-been-initialized-you-must-set-it-befo
    JobStorage.Current = storage;

    // Add the processing server as IHostedService
    builder.Services.AddHangfireServer(serverOptions => { serverOptions.ServerName = "Hangfire Server 1"; });
}

[ExcludeFromCodeCoverage]
static Logger ConfigureLogging(WebApplicationBuilder builder)
{
   builder.Logging.ClearProviders();
   var logger = new LoggerConfiguration()
       .ReadFrom.Configuration(builder.Configuration)
       .Enrich.With<LogLevelMapper>()
       .Enrich.WithProperty("service.version", Environment.GetEnvironmentVariable("SERVICE_VERSION"))
       .CreateLogger();
   builder.Logging.AddSerilog(logger);
   logger.Information("Starting application");
   return logger;
}

[ExcludeFromCodeCoverage]
static void ConfigureMongoDb(WebApplicationBuilder builder)
{
   builder.Services.AddSingleton<IMongoDbClientFactory>(_ =>
       new MongoDbClientFactory(builder.Configuration.GetValue<string>("Mongo:DatabaseUri")!,
           builder.Configuration.GetValue<string>("Mongo:DatabaseName")!));
}

[ExcludeFromCodeCoverage]
static void ConfigureEndpoints(WebApplicationBuilder builder)
{
   builder.Services.AddHealthChecks();
}

[ExcludeFromCodeCoverage]
static WebApplication BuildWebApplication(WebApplicationBuilder builder)
{
   var app = builder.Build();

   app.UseRouting();
   app.MapHealthChecks("/health");
   app.UseHangfireDashboard();
   
   var config = builder.Configuration
       .GetSection(ScheduleOptions.SectionName)
       .Get<ScheduleOptions>()!;
   
   foreach(KeyValuePair<string, RecurringTaskOption> entry in config.RecurringTasks)
   {
       Console.WriteLine("Registering recurring job {0}", entry.Key);
       RecurringJob.AddOrUpdate<RecurringTask>(entry.Key, (t) => t.Invoke(entry.Key, entry.Value), entry.Value.Schedule);
   }
   
   return app;
}