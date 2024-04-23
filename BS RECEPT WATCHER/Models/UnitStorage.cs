using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS_RECEPT_WATCHER.Models
{
    public class UnitStorage
    {
        [Key]
        public string UidUnitStorage { get; set; } = String.Empty;
        public string Quantite { get; set; } = String.Empty;
        public string UIDArticle { get; set; } = String.Empty;
    }
}
