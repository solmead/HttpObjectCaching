using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CachingAopExtensions;
using HttpObjectCaching;

namespace TestCacheMVC.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var rq = Cache.GetItem<string>(CacheArea.Request, "Test",(string) null);
            Cache.SetItem<string>(CacheArea.Request, "Test", "Chris");
            var rq2 = Cache.GetItem<string>(CacheArea.Request, "Test", (string)null);



            var test =  await TestClass.DoSomething();
            Debug.WriteLine(test);
            test.Add(1974);
            Cache.SetItem<string>(CacheArea.Request, "Test", "Powell");
            test =  await TestClass.DoSomething();
            test.Add(2015);
            var rq3 = Cache.GetItem<string>(CacheArea.Request, "Test", (string)null);
            return View();
        }

        public async Task<ActionResult> About()
        {
            ViewBag.Message = "Your application description page.";

            var test =  await TestClass.DoSomething();
            Debug.WriteLine(test);
            test = await TestClass.DoSomething();
            Debug.WriteLine(test);
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}