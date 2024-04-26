using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS_RECEPT_WATCHER.Services.Data
{
    public interface IUnitStorageProvider
    {
        public string GetLatestUidWithUnitStorage(string uid);
    }
}
