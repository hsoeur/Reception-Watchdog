using BS_RECEPT_WATCHER.Models;
using BS_RECEPT_WATCHER.Services.Api;
using BS_RECEPT_WATCHER.Services.AsmXmlFile;
using BS_RECEPT_WATCHER.Services.Data;
using BS_RECEPT_WATCHER.Services.FileProcessor;
using BS_RECEPT_WATCHER.Services.Print;
using Serilog;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.WindowsService.Services.FileProcessor
{

    /// <summary>
    /// Classe responsable de parser le fichier CSV des réceptions et d'appeler les services de création des bobines dans la base SQLite, d'impression des étiquettes, et de génération des XMLs.
    /// </summary>
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
        /// <summary>
        /// Méthode pour parser le fichier csv des réceptiosns.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
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
                    string dateReception = parts[28];
                    string stockClient = string.Empty;
                    string uidArticleClient = uidArticle;

                    if (uidArticle.Contains("-"))
                    {
                        stockClient = uidArticle.Substring(uidArticle.Length - 3);
                        string separator = "-";
                        int index = uidArticle.IndexOf(separator);
                        uidArticle = uidArticle.Substring(0, index);
                    }

                    cpt++;
                    uidUnitStorage = uidUnitStorage + cpt.ToString("D3");
                    await CreateUnitStorageAndPrint(uidArticle, quantite, uidUnitStorage, isDoublon, magasin, stockClient, uidArticleClient, stockClient, dateReception);
                }
            }
        }

        /// <summary>
        /// méthode pour créer un bobine dans la base SQLite et imprimer son étiquette.
        /// </summary>
        /// <param name="uidArticle"></param>
        /// <param name="quantite"></param>
        /// <param name="uidUnitStorage"></param>
        /// <param name="isDoublon"></param>
        /// <param name="magasin"></param>
        /// <param name="stockClient"></param>
        /// <param name="uidArticleClient"></param>
        /// <param name="stockClient1"></param>
        /// <returns></returns>
        private async Task CreateUnitStorageAndPrint(string uidArticle, string quantite, string uidUnitStorage, string isDoublon, string magasin, string stockClient, string uidArticleClient, string stockClient1, string dateReception)
        {
            try
            {
                UnitStorage unitStorage = await CreateUnitStorageWithLatestUid(uidArticle, quantite, uidUnitStorage, magasin, uidArticleClient, stockClient, dateReception);

                if (isDoublon == "10")
                {
                    ProcessDoublon(unitStorage);
                    _unitStorageCreator.CreateUnitStorage(unitStorage);
                    AddLineToXml(unitStorage);

                    Log.Information($"Package Unit n° {unitStorage.UidUnitStorage} avec le code composant n°{unitStorage.UidArticle} et une quantité de {unitStorage.Quantite} créée.");

                }
                else
                {
                    ProcessSingle(unitStorage);
                    _unitStorageCreator.CreateUnitStorage(unitStorage);
                    AddLineToXml(unitStorage);

                    Log.Information($"Package Unit n° {unitStorage.UidUnitStorage} avec le code composant n°{unitStorage.UidArticle} et une quantité de {unitStorage.Quantite} créée.");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

        }

        /// <summary>
        /// Récuépere le derniers numéro unique pour une bobine et l'incrémente.
        /// </summary>
        /// <param name="uidArticle"></param>
        /// <param name="quantite"></param>
        /// <param name="uidUnitStorage"></param>
        /// <param name="magasin"></param>
        /// <param name="uidArticleClient"></param>
        /// <param name="stockClient"></param>
        /// <returns></returns>
        private async Task<UnitStorage> CreateUnitStorageWithLatestUid(string uidArticle, string quantite, string uidUnitStorage, string magasin, string uidArticleClient, string stockClient, string dateReception)
        {
            try
            {
                var latestUnitStorageUid = _unitStorageProvider.GetLatestUidWithUnitStorage(uidUnitStorage);

                UnitStorage unitStorage = new UnitStorage()
                {
                    UidUnitStorage = latestUnitStorageUid,
                    Quantite = quantite,
                    UidArticle = uidArticle,
                    Magasin = magasin,
                    UidArticleClient = uidArticleClient,
                    StockClient = stockClient,
                    DateReception = dateReception

                };

                return unitStorage;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return null;

        }
        /// <summary>
        /// Impression normale des étiquettes.
        /// </summary>
        /// <param name="unitStorage"></param>
        private void ProcessSingle(UnitStorage unitStorage)
        {
            _printer.PrintUnit(_configuration["Print:Template"], unitStorage);
            _printer.Print(_configuration["Print:OutFile"]);
        }
        /// <summary>
        /// Responsable de la gestion de l'impresssion des étiquettes en doublon
        /// </summary>
        /// <param name="unitStorage"></param>
        private void ProcessDoublon(UnitStorage unitStorage)
        {
            _printer.PrintDoublon(_configuration["Print:Template"], unitStorage);
            _printer.Print(_configuration["Print:OutFile"]);
        }
        /// <summary>
        /// Appel du service de génération des XMLs.
        /// </summary>
        /// <param name="unitStorage"></param>
        private void AddLineToXml(UnitStorage unitStorage)
        {
            _asmXmlFileGen.GenerateXmlToCreatePackageUnit(unitStorage.UidUnitStorage, unitStorage.UidArticle, unitStorage.Quantite, unitStorage.Magasin, unitStorage.UidArticleClient, unitStorage.StockClient);
            Log.Information($"Fichier Xml créé pour la bobine n°{unitStorage.UidUnitStorage}");
        }

        /// <summary>
        /// Vérifie que toutes les lignes du fichier CSV ont le même uidUnitStorage.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool IsFileValid(string file)
        {
            HashSet<string> uniqueUnitStorageUids = new HashSet<string>();

            using (StreamReader reader = new StreamReader(file))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(';');
                    if (parts.Length < 11) continue; // Sécurité

                    string uidUnitStorage = parts[10];
                    uniqueUnitStorageUids.Add(uidUnitStorage);

                    if (uniqueUnitStorageUids.Count > 1)
                    {
                        // Inutile de continuer, on a déjà plusieurs uidUnitStorage différents
                        return false;
                    }
                }
            }

            return true;
        }

    }
}
