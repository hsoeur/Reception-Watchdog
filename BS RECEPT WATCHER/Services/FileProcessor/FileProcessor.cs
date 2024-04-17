using BS_RECEPT_WATCHER.Models;
using BS_RECEPT_WATCHER.Services.Api;
using BS_RECEPT_WATCHER.Services.AsmXmlFile;
using BS_RECEPT_WATCHER.Services.Print;
using Serilog;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS_RECEPT_WATCHER.Services.FileProcessor
{

   
    public class FileProcessor : IFileProcessor
    {
        private readonly IPrinter _printer;
        private readonly IApiService _apiService;
        private readonly IAsmXmlFileGen _asmXmlFileGen;
        private readonly IConfiguration _configuration;
        public FileProcessor(IPrinter printer, IApiService apiService, IAsmXmlFileGen asmXmlFileGen, IConfiguration configuration)
        {
            _apiService = apiService;
            _printer = printer;
            _asmXmlFileGen = asmXmlFileGen;
            _configuration = configuration;
        }

        public async Task ParseFile(string file)
        {
            List<UnitStorage> units = new List<UnitStorage>();

            using (StreamReader reader = new StreamReader(file))
            {
                string line;
                int cpt = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    Log.Debug(line);

                    string[] parts = line.Split(';');
                    string uidArticle = parts[0];
                    string quantite = parts[5];
                    string uidUnitStorage = parts[10];
                    string isDoublon = parts[24];

                    cpt++;
                    uidUnitStorage = uidUnitStorage + cpt.ToString("D3");
                    await CreateUnitStorageAndPrint(uidArticle, quantite, uidUnitStorage, isDoublon);
                }
            }
        }

        private async Task CreateUnitStorageAndPrint(string uidArticle, string quantite, string uidUnitStorage, string isDoublon)
        {
            UnitStorage unitStorage = await CreateUnitStorageWithLatestUid(uidArticle, quantite, uidUnitStorage);

            if (isDoublon == "10")
            {
                ProcessDoublon(unitStorage);
                await _apiService.CreateUnitStorage(unitStorage);
                AddLineToXml(unitStorage);

                Log.Information($"Package Unit n° {unitStorage.UIDUnitStorage} avec le code composant n°{unitStorage.UIDArticle} et une quantité de {unitStorage.Quantite} créée.");

            }
            else
            {
                ProcessSingle(unitStorage);
                await _apiService.CreateUnitStorage(unitStorage);
                AddLineToXml(unitStorage);

                Log.Information($"Package Unit n° {unitStorage.UIDUnitStorage} avec le code composant n°{unitStorage.UIDArticle} et une quantité de {unitStorage.Quantite} créée.");
            }
        }

        private async Task<UnitStorage> CreateUnitStorageWithLatestUid(string uidArticle, string quantite, string uidUnitStorage)
        {
            var latestUnitStorageUid = await _apiService.GetLatestUidWithUnitStorage(uidUnitStorage);

            UnitStorage unitStorage = new UnitStorage()
            {
                UIDUnitStorage = latestUnitStorageUid,
                Quantite = quantite,
                UIDArticle = uidArticle
            };
            return unitStorage;
        }

        private void ProcessSingle(UnitStorage unitStorage)
        {
            _printer.PrintUnit(_configuration["Print:Template"], unitStorage);
            _printer.Print(_configuration["Print:OutFile"]);
        }

        private void ProcessDoublon(UnitStorage unitStorage)
        {
            _printer.PrintDoublon(_configuration["Print:Template"], unitStorage);
            _printer.Print(_configuration["Print:OutFile"]);
        }

        private void AddLineToXml(UnitStorage unitStorage)
        {
            _asmXmlFileGen.GenerateXmlToCreatePackageUnit(unitStorage.UIDUnitStorage, unitStorage.UIDArticle, unitStorage.Quantite);
            Log.Information($"Fichier Xml créé pour la bobine n°{unitStorage.UIDUnitStorage}");
        }
    }
}
