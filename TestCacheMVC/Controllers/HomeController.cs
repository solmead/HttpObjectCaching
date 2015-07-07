using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CachingAopExtensions;
using HttpObjectCaching;
using HttpObjectCaching.Core.Collections;

namespace TestCacheMVC.Controllers
{
    public class HomeController : Controller
    {
        public ListOnCache<int> BaseList = new ListOnCache<int>("BaseList", CacheArea.Distributed);
        public async Task<ActionResult> Index()
        {
            //BaseList.Add(DateTime.Now.Minute);
            //string joined = string.Join(",", BaseList.ToArray());
            //Debug.WriteLine(joined);
            //var rq = Cache.GetItem<string>(CacheArea.Request, "Test",(string) null);
            //Cache.SetItem<string>(CacheArea.Request, "Test", "Chris");
            //var rq2 = Cache.GetItem<string>(CacheArea.Request, "Test", (string)null);



            var test = TestClass.DoSomething2();
            Debug.WriteLine(test);
            test.Add(1974);
            //
            CacheAOP.SetItem<int, string, List<int>>(TestClass.DoSomething2, null, new List<int>() { 5, 4, 3, 2, 1 });
            CacheAOP.SetItem(TestClass.DoSomething3, new List<int>() { 5, 4, 3, 2, 1 });
            //Cache.SetItem<string>(CacheArea.Request, "Test", "Powell");
            test = TestClass.DoSomething3();
            test.Add(2015);
            //var rq3 = Cache.GetItem<string>(CacheArea.Request, "Test", (string)null);
            return View();
        }

        public async Task<ActionResult> About()
        {
            ViewBag.Message = "Your application description page.";
            BaseList.Clear();
            string joined = string.Join(",", BaseList.ToArray());
            Debug.WriteLine(joined);
            //var test =  await TestClass.DoSomething();
            //Debug.WriteLine(test);
            //test = await TestClass.DoSomething();
            //Debug.WriteLine(test);
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}