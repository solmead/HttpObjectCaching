using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HttpObjectCaching;
using HttpObjectCaching.Core.Collections;
using HttpObjectCaching.Core.Collections.List;

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
            var rq = Cache.GetItem<List<string>>(CacheArea.Distributed, "Test", ()=>
            {
                var lst = new List<string>() {"12","34","56"};
                return lst;
            });
            string joined = string.Join(",", rq.ToArray());
            Debug.WriteLine(joined);
            rq.Add("78");
            Cache.SetItem<List<string>>(CacheArea.Distributed, "Test", rq);



            //var rq2 = Cache.GetItem<string>(CacheArea.Request, "Test", (string)null);


            //var t = await TestClass.DoSomething();
            //Debug.WriteLine(t);
            //t.Add(1972);
            //await CacheAOP.SetItemAsync<int, string, List<int>>(TestClass.DoSomething, null, new List<int>() { 51, 41, 31, 21, 11 });
            //t = await TestClass.DoSomething();



            //var test = TestClass.DoSomething2();
            //Debug.WriteLine(test);
            //test.Add(1974);
            ////
            //CacheAOP.SetItem<int, string, List<int>>(TestClass.DoSomething2, null, new List<int>() { 5, 4, 3, 2, 1 });
            //CacheAOP.SetItem(TestClass.DoSomething3, new List<int>() { 5, 4, 3, 2, 1 });
            ////Cache.SetItem<string>(CacheArea.Request, "Test", "Powell");
            //test = TestClass.DoSomething3();
            //test.Add(2015);
            ////var rq3 = Cache.GetItem<string>(CacheArea.Request, "Test", (string)null);
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