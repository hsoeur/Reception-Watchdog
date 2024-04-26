using BS_RECEPT_WATCHER.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS_RECEPT_WATCHER.Services.Data
{
    public class DatabaseUnitStorageProvider : IUnitStorageProvider
    {
        private readonly BsReceptWatcherDbContextFactory _dbContextFactory;
        /// <summary>
        /// Constructeur 
        /// </summary>
        /// <param name="urnDbContextFactory"></param>
        public DatabaseUnitStorageProvider(BsReceptWatcherDbContextFactory bsReceptWatcherDbContextFactory)
        {
            _dbContextFactory = bsReceptWatcherDbContextFactory;
        }

        public string GetLatestUidWithUnitStorage(string unitStorageUid)
        {
            using (var context = _dbContextFactory.CreateDbContext())
            {
                var unitS = context.UnitStorages.FirstOrDefault(us => us.UidUnitStorage == unitStorageUid);

                if (unitS != null)
                {
                    // ---------- Incrémentation des URN si ils existent déjà dans la base ---------- //
                    string prefixe = unitS.UidUnitStorage.Substring(0, 8);

                    var lastUnitStorageIndex = context.UnitStorages
                                 .Where(e => e.UidUnitStorage.StartsWith(prefixe))
                                 .OrderByDescending(e => e.UidUnitStorage)
                                 .FirstOrDefault();

                    if (lastUnitStorageIndex != null)
                    {
                        string lastUid = lastUnitStorageIndex.UidUnitStorage;
                        int index = int.Parse(lastUid.Substring(8)); // Parse the integer part
                        int latest = index + 1;

                        return prefixe + latest.ToString().PadLeft(lastUid.Length - 8, '0'); // Ensure the same length as original UID
                    }
                }

                return unitStorageUid;
            }
        }
    }
}
