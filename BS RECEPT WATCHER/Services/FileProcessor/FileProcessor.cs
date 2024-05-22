using BS_RECEPT_WATCHER.Models;
using BS_RECEPT_WATCHER.Services.Api;
using BS_RECEPT_WATCHER.Services.AsmXmlFile;
using BS_RECEPT_WATCHER.Services.Data;
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
        private readonly IUnitStorageProvider _unitStorageProvider;
        private readonly IUnitStorageCreator _unitStorageCreator;
        public FileProcessor(IPrinter printer, IApiService apiService, IAsmXmlFileGen asmXmlFileGen, IConfiguration configuration, IUnitStorageProvider unitStorageProvider, IUnitStorageCreator unitStorageCreator)
        {
            _apiService = apiService;
            _printer = printer;
            _asmXmlFileGen = asmXmlFileGen;
            _configuration = configuration;
            _unitStorageProvider = unitStorageProvider;
            _unitStorageCreator = unitStorageCreator;
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
                    string isDoublon = parts[19];
                    string magasin = parts[27];
                    string stockClient = String.Empty;

                    if (uidArticle.Contains("-"))
                    {
                        stockClient = uidArticle.Substring(uidArticle.Length - 3);
                        string separator = "-";
                        int index = uidArticle.IndexOf(separator);
                        uidArticle = uidArticle.Substring(0, index);
                    }

                    cpt++;
                    uidUnitStorage = uidUnitStorage + cpt.ToString("D3");
                    await CreateUnitStorageAndPrint(uidArticle, quantite, uidUnitStorage, isDoublon, magasin, stockClient);
                }
            }
        }

        private async Task CreateUnitStorageAndPrint(string uidArticle, string quantite, string uidUnitStorage, string isDoublon, string magasin, string stockClient)
        {
            try
            {
                UnitStorage unitStorage = await CreateUnitStorageWithLatestUid(uidArticle, quantite, uidUnitStorage, magasin);

                if (isDoublon == "10")
                {
                    ProcessDoublon(unitStorage);
                    _unitStorageCreator.CreateUnitStorage(unitStorage);
                    AddLineToXml(unitStorage, stockClient);

                    Log.Information($"Package Unit n° {unitStorage.UidUnitStorage} avec le code composant n°{unitStorage.UidArticle} et une quantité de {unitStorage.Quantite} créée.");

                }
                else
                {
                    ProcessSingle(unitStorage);
                    _unitStorageCreator.CreateUnitStorage(unitStorage);
                    AddLineToXml(unitStorage, stockClient);

                    Log.Information($"Package Unit n° {unitStorage.UidUnitStorage} avec le code composant n°{unitStorage.UidArticle} et une quantité de {unitStorage.Quantite} créée.");
                }
            }catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            
        }

        private async Task<UnitStorage> CreateUnitStorageWithLatestUid(string uidArticle, string quantite, string uidUnitStorage, string magasin)
        {
            try
            {
                var latestUnitStorageUid =  _unitStorageProvider.GetLatestUidWithUnitStorage(uidUnitStorage);

                UnitStorage unitStorage = new UnitStorage()
                {
                    UidUnitStorage = latestUnitStorageUid,
                    Quantite = quantite,
                    UidArticle = uidArticle,
                    Magasin = magasin,
                    
                };
                return unitStorage;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return null;
            
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

        private void AddLineToXml(UnitStorage unitStorage, string stockClient)
        {
            _asmXmlFileGen.GenerateXmlToCreatePackageUnit(unitStorage.UidUnitStorage, unitStorage.UidArticle, unitStorage.Quantite, unitStorage.Magasin, stockClient);
            Log.Information($"Fichier Xml créé pour la bobine n°{unitStorage.UidUnitStorage}");
        }
    }
}
