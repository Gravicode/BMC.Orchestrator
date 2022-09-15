using BMC.MessageBroker;
using BMC.MessageBroker.Data;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

var configBuilder = new ConfigurationBuilder()
  .SetBasePath(Directory.GetCurrentDirectory())
  .AddJsonFile("appsettings.json", optional: false);
IConfiguration Configuration = configBuilder.Build();

AppConstants.SQLConn = Configuration["ConnectionStrings:SqlConn"];
AppConstants.RedisCon = Configuration["RedisCon"];
AppConstants.BlobConn = Configuration["ConnectionStrings:BlobConn"];
AppConstants.GMapApiKey = Configuration["GmapKey"];



await host.RunAsync();


