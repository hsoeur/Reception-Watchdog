using BS_RECEPT_WATCHER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS_RECEPT_WATCHER.Services.Data
{
    public interface IUnitStorageCreator
    {
        /// <summary>
        /// Créer une unité de stockage avec un UID dans la table UnitStorage de la base de données.
        /// </summary>
        /// <param name="unitStorage"></param>
        public void CreateUnitStorage(UnitStorage unitStorage);
    }
}
