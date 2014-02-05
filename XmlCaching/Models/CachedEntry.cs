using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlCaching.Models
{
    [Serializable]
    public class CachedEntry
    {

        public string Name { get; set; }
        public string Object { get; set; }
        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
        public DateTime? TimeOut { get; set; }
    }
}
