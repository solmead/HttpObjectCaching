using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HttpObjectCaching;
using HttpObjectCaching.Core;
using HttpObjectCaching.Core.DataSources;

namespace CachingAopExtensions
{
    public class TestClass
    {
        static IDataSource ds=new LocalDataSource();

        [CachingAspect(CacheArea = CacheArea.Global)]
        public static async Task<List<int>> DoSomething(int a = 0, string b = "test")
        {
            return new List<int>() {5,4,3,2,1};
        }


        [CachingAspect(CacheArea = CacheArea.Global)]
        public static List<int> DoSomething2(int a = 0, string b = "test")
        {
            return new List<int>() {6,7,8,9,10};
        }

        [CachingAspect(CacheArea = CacheArea.Global)]
        public static List<int> DoSomething3()
        {
            return new List<int>() { 16, 17, 18, 19, 10 };
        }


        public static void SetValues(List<int> val)
        {
            CacheAOP.SetItem(DoSomething3, val);
        }
    }
}
