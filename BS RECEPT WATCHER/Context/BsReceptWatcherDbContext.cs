using BS_RECEPT_WATCHER.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS_RECEPT_WATCHER.Context
{
    public class BsReceptWatcherDbContext : DbContext
    {
        
        public DbSet<UnitStorage> UnitStorages { get; set; }

        public BsReceptWatcherDbContext()
        {

        }
        public BsReceptWatcherDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}
