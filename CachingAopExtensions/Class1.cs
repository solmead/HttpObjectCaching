using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpObjectCaching;

namespace CachingAopExtensions
{
    class Class1
    {
        [CacheResultsAspect(CacheArea=CacheArea.Global, LifeSpanSeconds=60)]
        public async Task<List<int>> DoSomething()
        {
            return new List<int>() {5,4,3,2,1};
        }


        [CacheResultsAspect(CacheArea = CacheArea.Global, LifeSpanSeconds = 60)]
        public List<int> DoSomething2()
        {
            return new List<int>() {6,7,8,9,10};
        } 
    }
}
