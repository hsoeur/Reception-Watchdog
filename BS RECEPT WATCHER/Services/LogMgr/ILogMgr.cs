using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS_RECEPT_WATCHER.Services.LogMgr
{
    public interface ILogMgr
    {
        public void WriteCreatedFileContentInLogsFile(string fileToWatchPath, string logFilePath);
    }
}
