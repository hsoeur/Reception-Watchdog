using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BS_RECEPT_WATCHER.Services.AsmXmlFile
{
    public class AsmXmlFileGen : IAsmXmlFileGen
    {
        public void GenerateXmlToCreatePackageUnit(string uidUnitStorage, string articleUnitStorage, string quantityUnitStorage)
        {
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("PackagingList",
                new XAttribute("ImportVersion", "1.0.1.1"),
                new XElement("Packaging",
                    new XAttribute("IdPackaging", uidUnitStorage),
                    new XAttribute("ComponentName", articleUnitStorage),
                    new XAttribute("ComponentBarcode", ""),
                    new XAttribute("Quantity", quantityUnitStorage))
                    )
            );

            string date = DateTime.Now.ToString("hh_mm_ss_ffff");
            string filepath = "BSWATCHERRECEPT_CREATE_" + date + ".xml";

            using (StreamWriter writer = new StreamWriter(filepath))
            {
                doc.Save(writer);
            }
        }
    }
}
