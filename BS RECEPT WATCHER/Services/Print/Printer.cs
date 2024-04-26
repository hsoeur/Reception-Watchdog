using BS_RECEPT_WATCHER.Models;
using CmdGenerator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS_RECEPT_WATCHER.Services.Print
{
    public class Printer : IPrinter
    {
        public void Print(string fileToPrint)
        {
            /*
            string fileNameOutForBrady = @"C:\Windows\System32\out_file.txt";

            string commandBrady = $"copy \"{fileNameOutForBrady}\" LPT1";

            CommandGenerator commandGenerator = new CommandGenerator(commandBrady);
            commandGenerator.Execute();
            */

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = false,
                WorkingDirectory = @"C:\bs\BS RECEPT WATCHER\"
            };

            Process process = new Process { StartInfo = psi };

            // Démarrer le processus
            process.Start();

            // Écrire la commande dans le terminal CMD
            process.StandardInput.WriteLine("copy \"C:\\bs\\BS RECEPT WATCHER\\out_file.txt\" LPT1");

            // Fermer le flux d'entrée et attendre la fin de l'exécution
            process.StandardInput.Close();
            process.WaitForExit();

        }

        public string PrintDoublon(string template, UnitStorage unitStorage)
        {
            string templateContent = File.ReadAllText(template);

            templateContent = templateContent.Replace("#UidUnitStorage#", unitStorage.UidUnitStorage);
            templateContent = templateContent.Replace("#UidCodeArticle#", unitStorage.UIDArticle);
            templateContent = templateContent.Replace("#Quantite#", unitStorage.Quantite);
            templateContent = templateContent.Replace("#Date#", DateTime.Now.ToString("dd/MM/yyyy"));
            templateContent = templateContent.Replace("#QuantiteEtiquettes#", "2");

            File.WriteAllText(@"C:\bs\BS RECEPT WATCHER\out_file.txt", templateContent);

            return templateContent;
        }

        public string PrintUnit(string template, UnitStorage unitStorage)
        {
            string templateContent = File.ReadAllText(template);

            templateContent = templateContent.Replace("#UidUnitStorage#", unitStorage.UidUnitStorage);
            templateContent = templateContent.Replace("#UidCodeArticle#", unitStorage.UIDArticle);
            templateContent = templateContent.Replace("#Quantite#", unitStorage.Quantite);
            templateContent = templateContent.Replace("#Date#", DateTime.Now.ToString("dd/MM/yyyy"));
            templateContent = templateContent.Replace("#QuantiteEtiquettes#", "1");

            File.WriteAllText(@"C:\bs\BS RECEPT WATCHER\out_file.txt", templateContent);

            return templateContent;
        }

        

    }
}
