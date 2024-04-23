using BS_RECEPT_WATCHER.Models;
using CmdGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS_RECEPT_WATCHER.Services.Print
{
    public class Printer : IPrinter
    {
        public void Print(string fileToPrint)
        {
            string fileNameOutForBrady = "out_file.txt";

            string commandBrady = $"copy \"{fileNameOutForBrady}\" LPT1";

            CommandGenerator commandGenerator = new CommandGenerator(commandBrady);
            commandGenerator.Execute();
        }

        public string PrintDoublon(string template, UnitStorage unitStorage)
        {
            string templateContent = File.ReadAllText(template);

            templateContent = templateContent.Replace("#UidUnitStorage#", unitStorage.UidUnitStorage);
            templateContent = templateContent.Replace("#UidCodeArticle#", unitStorage.UIDArticle);
            templateContent = templateContent.Replace("#Quantite#", unitStorage.Quantite);
            templateContent = templateContent.Replace("#Date#", DateTime.Now.ToString("dd/MM/yyyy"));
            templateContent = templateContent.Replace("#QuantiteEtiquettes#", "2");

            File.WriteAllText("out_file.txt", templateContent);

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

            File.WriteAllText("out_file.txt", templateContent);

            return templateContent;
        }

        

    }
}
