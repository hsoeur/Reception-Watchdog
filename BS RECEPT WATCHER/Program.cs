using BS_RECEPT_WATCHER.Context;
using BS_RECEPT_WATCHER.Services.Api;
using BS_RECEPT_WATCHER.Services.AsmXmlFile;
using BS_RECEPT_WATCHER.Services.Data;
using BS_RECEPT_WATCHER.Services.FileProcessor;
using BS_RECEPT_WATCHER.Services.LogMgr;
using BS_RECEPT_WATCHER.Services.Print;
using BS_RECEPT_WATCHER.Workers;
using Serilog;


internal class Program
{
    private static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddWindowsService(options =>
        {
            options.ServiceName = ".NET Service - ReceptWatcher";
        });

        IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("C:\\bs\\BS RECEPT WATCHER\\bin\\appsettings.json")
                .Build();

        Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.File(configuration["LogFiles:FunctionnalLogs"])
                    .CreateLogger();

        builder.Services.AddSingleton<IUnitStorageCreator, DatabaseUnitStorageCreator>();
        builder.Services.AddSingleton<IUnitStorageProvider, DatabaseUnitStorageProvider>();
        builder.Services.AddSingleton<IConfigurationRoot>(configuration);
        builder.Services.AddSingleton<IAsmXmlFileGen, AsmXmlFileGen>();
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<IPrinter, Printer>();
        builder.Services.AddSingleton<IFileProcessor, FileProcessor>();
        builder.Services.AddSingleton<BsReceptWatcherDbContextFactory>(); 
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<ILogMgr, LogMgr>();
        builder.Services.AddHostedService<Worker>();

        IHost host = builder.Build();
        host.Run();
    }

}