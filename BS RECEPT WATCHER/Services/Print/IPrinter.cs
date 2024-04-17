using BS_RECEPT_WATCHER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS_RECEPT_WATCHER.Services.Print
{
    public interface IPrinter
    {
        void Print(string fileToPrint);
        public string PrintUnit(string template, UnitStorage unitStorage);
        public string PrintDoublon(string template, UnitStorage unitStorage);
    }
}
