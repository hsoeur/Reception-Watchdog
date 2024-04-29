using BS_RECEPT_WATCHER.Models;
using BS_RECEPT_WATCHER.Services.Api;
using BS_RECEPT_WATCHER.Services.FileProcessor;
using BS_RECEPT_WATCHER.Services.LogMgr;
using BS_RECEPT_WATCHER.Services.Print;

namespace BS_RECEPT_WATCHER.Workers
{
    public class Worker : BackgroundService
    {
        
        private readonly ILogger<Worker> _logger;
        private readonly ILogMgr _logMgr;
        private readonly IFileProcessor _fileProcessor;
        private readonly string _fileToWatchPath;
        private FileSystemWatcher _fileWatcher;
        private readonly string _logFilePath;
        private readonly IConfiguration _configuration;
        public Worker(ILogger<Worker> logger, IFileProcessor fileProcessor, ILogMgr logMgr, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _fileProcessor = fileProcessor;
            _fileToWatchPath = _configuration["Files:FileToWatchGpao"];
            _logFilePath = _configuration["LogFiles:ContentFileProcessed"];
            _logMgr = logMgr;

        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _fileWatcher = new FileSystemWatcher();
            _fileWatcher.Path = Path.GetDirectoryName(_fileToWatchPath);
            _fileWatcher.Filter = Path.GetFileName(_fileToWatchPath);
            _fileWatcher.Created += OnFileCreated;
            _fileWatcher.EnableRaisingEvents = true;

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _fileWatcher.Dispose();
            return base.StopAsync(cancellationToken);
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            _logMgr.WriteCreatedFileContentInLogsFile(_fileToWatchPath, _logFilePath);

            _ = ParseFileAndCreateUnitStorage();
        }

        private async Task ParseFileAndCreateUnitStorage()
        {

            await _fileProcessor.ParseFile(_fileToWatchPath);
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
     
    }
}
