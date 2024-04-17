using BS_RECEPT_WATCHER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS_RECEPT_WATCHER.Services.Api
{
    public interface IApiService
    {
        public Task<string> CreateUnitStorage(UnitStorage unitStorage);
        public Task<string> GetLatestUidWithUnitStorage(string unitStorage);
    }
}
