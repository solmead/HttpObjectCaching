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
using HttpObjectCaching.Core.Collections.List;

namespace TestCacheMVC.Controllers
{
    public class HomeController : Controller
    {
        public ListOnCache<int> BaseList = new ListOnCache<int>("BaseList", CacheArea.Session);
        public async Task<ActionResult> Index()
        {
            //BaseList.Add(DateTime.Now.Minute);
            //string joined = string.Join(",", BaseList.ToArray());
            //Debug.WriteLine(joined);
            //var rq = Cache.GetItem<List<string>>(CacheArea.Cookie, "Test", ()=>
            //{
            //    var lst = new List<string>() {"12","34","56"};
            //    return lst;
            //});
            //string joined = string.Join(",", rq.ToArray());
            //Debug.WriteLine(joined);
            //rq.Add("78");
            //Cache.SetItem<List<string>>(CacheArea.Cookie, "Test", rq);



            //var rq2 = Cache.GetItem<string>(CacheArea.Request, "Test", (string)null);

            var tt = new TestClass();

            var tt1 = tt.MyList1;
            tt.MyList1 = new List<int>() {51, 41, 31, 21, 11};
            tt1 = tt.MyList1;

            var tt2 = tt.MyList2;
            tt.SetMyList(new List<int>() { 51, 41, 31, 21, 11 });
            tt2 = tt.MyList2;



            var t = await TestClass.DoSomething();
            Debug.WriteLine(t);
            t.Add(1972);
            t = await TestClass.DoSomething();
            Debug.WriteLine(t);
            await CacheAOP.SetOnFunctionAsync<int, string, List<int>>(TestClass.DoSomething, null, new List<int>() { 51, 41, 31, 21, 11 });
            t = await TestClass.DoSomething();
            Debug.WriteLine(t);



            var test = TestClass.DoSomething2(1974, "Me");
            Debug.WriteLine(test);
            test.Add(1972);
            test = TestClass.DoSomething2(1974, "Me");
            Debug.WriteLine(test);
            //
            CacheAOP.SetOnFunction<int, string, List<int>>(TestClass.DoSomething2, 
                new List<object>() {1974, "Me"}, new List<int>() { 5, 4, 3, 2, 1 });
            test = TestClass.DoSomething2(1974, "Me");
            Debug.WriteLine(test);



            test = TestClass.DoSomething2();
            Debug.WriteLine(test);
            test.Add(1977);
            test = TestClass.DoSomething2();
            Debug.WriteLine(test);
            //
            CacheAOP.SetOnFunction<int, string, List<int>>(TestClass.DoSomething2, null, new List<int>() { 5, 4, 3, 2, 1 });
            test = TestClass.DoSomething2();
            Debug.WriteLine(test);




            CacheAOP.SetOnFunction(TestClass.DoSomething3, new List<int>() { 5, 4, 3, 2, 1 });
            //Cache.SetItem<string>(CacheArea.Request, "Test", "Powell");
            test = TestClass.DoSomething3();
            test.Add(2015);
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