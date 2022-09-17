using BMC.StreamHub;
using BMC.StreamHub.Data;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddTransient<ProjectService>();
        services.AddTransient<AlertService>();
        services.AddTransient<DashboardService>();
        services.AddTransient<MessageStreamService>();
        services.AddTransient<DeviceService>();
        services.AddTransient<UserProfileService>();
        services.AddTransient<MqttTopicService>();
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

AppConstants.MqttPort = Configuration["App:MqttPort"];
AppConstants.MqttHost = Configuration["App:MqttHost"];



await host.RunAsync();