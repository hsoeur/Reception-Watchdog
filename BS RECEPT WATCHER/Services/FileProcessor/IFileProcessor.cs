using BS_RECEPT_WATCHER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS_RECEPT_WATCHER.Services.FileProcessor
{
    public interface IFileProcessor
    {
        public Task ParseFile(string file);
        public bool IsFileValid(string file);

    }
}
