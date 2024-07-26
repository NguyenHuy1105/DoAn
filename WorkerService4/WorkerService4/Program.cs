using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorkerService4.Service;
using WorkerService4.Get_Infomaiton;
using WorkerService4.Model;
using System.Reflection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((hostContext, services) =>
     {
         services.AddHttpClient();
        services.AddHostedService<MACAddressWorker>();
         services.AddHostedService<FileService>();
         services.AddHostedService<InstallFileService>();
         services.AddHostedService<HistoryService>();
         // services.AddHostedService<WinFormAppOpener>();
         services.Configure<ApiConfig>(hostContext.Configuration.GetSection("ApiConfig"));
     })
    .UseWindowsService()
    .Build();

await host.RunAsync();