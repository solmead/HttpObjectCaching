using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace XmlCaching.Models
{
    [Serializable]
    public class CachedEntry
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Object { get; set; }
        public DateTime Created { get; set; }
        public DateTime Changed { get; set; }
        public DateTime? TimeOut { get; set; }
    }
}
