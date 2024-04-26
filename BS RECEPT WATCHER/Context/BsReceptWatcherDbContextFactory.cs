using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS_RECEPT_WATCHER.Context
{
    public class BsReceptWatcherDbContextFactory
    {
        private readonly string? _connectionString;
        private IConfigurationRoot _configuration;

        public BsReceptWatcherDbContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public BsReceptWatcherDbContextFactory(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        public BsReceptWatcherDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<BsReceptWatcherDbContext>();


            optionsBuilder.UseSqlite(_configuration["ConnectionStrings:SQLite"]);
            return new BsReceptWatcherDbContext(optionsBuilder.Options);
            
            
        }
    }
}
