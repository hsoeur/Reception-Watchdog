using BS_RECEPT_WATCHER.Models;
using BS_RECEPT_WATCHER.Services.Api;
using BS_RECEPT_WATCHER.Services.FileProcessor;
using BS_RECEPT_WATCHER.Services.LogMgr;
using BS_RECEPT_WATCHER.Services.Print;
using Serilog;

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
            _fileToWatchPath = _configuration["Files:FileToWatchLocal"];
            _logFilePath = _configuration["LogFiles:ContentFileProcessed"];
            _logMgr = logMgr;

        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            File.Delete(_fileToWatchPath);
            _fileWatcher = new FileSystemWatcher();
            _fileWatcher.Path = Path.GetDirectoryName(_fileToWatchPath);
            _fileWatcher.Filter = Path.GetFileName(_fileToWatchPath);

            _fileWatcher.NotifyFilter = NotifyFilters.Attributes
                         | NotifyFilters.CreationTime
                         | NotifyFilters.DirectoryName
                         | NotifyFilters.FileName
                         | NotifyFilters.LastAccess
                         | NotifyFilters.LastWrite
                         | NotifyFilters.Security
                         | NotifyFilters.Size;

            //_fileWatcher.Created += OnFileCreated;
            _fileWatcher.Changed += OnFileCreated;
            _fileWatcher.EnableRaisingEvents = true;

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _fileWatcher.Dispose();
            return base.StopAsync(cancellationToken);
        }

        private async void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                _fileWatcher.EnableRaisingEvents = false; // Désactiver les événements

                Log.Information("Traitement du Process");

                while (IsFileLocked(_fileToWatchPath))
                {
                    await Task.Delay(100);
                }

                _logMgr.WriteCreatedFileContentInLogsFile(_fileToWatchPath, _logFilePath);

                _ = ParseFileAndCreateUnitStorage();

                while (IsFileLocked(_fileToWatchPath))
                {
                    await Task.Delay(100);
                }

                File.Delete(_fileToWatchPath);

                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                Log.Error($"Une erreur est survenue lors du traitement du fichier : {ex.Message}");
            }
            finally
            {
                _fileWatcher.EnableRaisingEvents = true; // Réactiver les événements
            }
        }


        private bool IsFileLocked(string filePath)
        {
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }

            return false;
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
