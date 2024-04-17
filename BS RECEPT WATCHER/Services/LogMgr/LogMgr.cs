using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS_RECEPT_WATCHER.Services.LogMgr
{
    public class LogMgr : ILogMgr
    {


        public void WriteCreatedFileContentInLogsFile(string fileToWathPath, string logFilePath)
        {
            try
            {
                string fileContent = File.ReadAllText(fileToWathPath);

                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    writer.WriteLine($"Contenu du fichier modifié à {DateTime.Now}:");
                    writer.WriteLine();
                    writer.WriteLine(fileContent);
                    writer.WriteLine("----------------------------------------");

                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
