using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    [Serializable]
    public class testModel
    {

        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime? LastSeen { get; set; }

        public testModel2 subtest { get; set; }
    }

    [Serializable]
    public class testModel2
    {
        public string info { get; set; }
    }
}
