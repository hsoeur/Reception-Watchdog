using BS_RECEPT_WATCHER.Context;
using BS_RECEPT_WATCHER.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS_RECEPT_WATCHER.Services.Data
{

    public class DatabaseUnitStorageCreator : IUnitStorageCreator
    {
        private readonly BsReceptWatcherDbContextFactory _dbContextFactory;
        /// <summary>
        /// Constructeur 
        /// </summary>
        /// <param name="urnDbContextFactory"></param>
        public DatabaseUnitStorageCreator(BsReceptWatcherDbContextFactory bsReceptWatcherDbContextFactory)
        {
            _dbContextFactory = bsReceptWatcherDbContextFactory;
        }
        /// <summary>
        /// Creation unité de stockage dans la base
        /// </summary>
        /// <param name="unitStorage"></param>
        public void CreateUnitStorage(UnitStorage unitStorage)
        {
            try
            {
                using (var context = _dbContextFactory.CreateDbContext())
                {

                    if (context.UnitStorages.Contains(unitStorage))
                    {

                        // ---------- Incrémentation des URN si ils existent déjà dans la base ---------- //

                        string prefixe = unitStorage.UidUnitStorage.Substring(0, 8);

                        var lastUnitStorageIndex = context.UnitStorages
                                     .Where(e => e.UidUnitStorage.StartsWith(prefixe))
                                     .OrderByDescending(e => e.UidUnitStorage)
                                     .FirstOrDefault();

                        int newUnitStorageIndex = (lastUnitStorageIndex?.UidUnitStorage != null) ? (int.Parse(lastUnitStorageIndex.UidUnitStorage.Substring(prefixe.Length)) + 1) : 1;

                        string formattedIndex = newUnitStorageIndex.ToString("D3");

                        unitStorage.UidUnitStorage = $"{prefixe}{formattedIndex}";

                        // ---------------------------------------------------------------------------- //

                        context.UnitStorages.Add(unitStorage);
                        context.SaveChanges();

                    }
                    else
                    {
                        context.UnitStorages.Add(unitStorage);
                        context.SaveChanges();
                    }

                }
            }
            catch (DbUpdateException ex)
            {

                throw;

            }

        }
    }
}
