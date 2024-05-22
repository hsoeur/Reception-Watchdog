using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS_RECEPT_WATCHER.Services.AsmXmlFile
{
    public interface IAsmXmlFileGen
    {
        public void GenerateXmlToCreatePackageUnit(string uidUnitStorage, string articleUnitStorage, string quantityUnitStorage, string magasin, string stockClient);
    }
}
