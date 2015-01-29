using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HttpObjectCaching;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            foreach(CacheArea area in Enum.GetValues(typeof(CacheArea)))
            {
                Debug.WriteLine("<==========================================>");
                Debug.WriteLine(area.ToString());
                var i = Cache.GetItem<int?>(area, "test" + area.ToString(), () => 456);
                Debug.WriteLine(i);
                Cache.SetItem<int?>(area, "test" + area.ToString(), 789);
                i = Cache.GetItem<int?>(area, "test" + area.ToString(), () => 777);
                Debug.WriteLine(i);
                Debug.WriteLine("<==========================================>");
                Debug.WriteLine("");

                Debug.WriteLine("<==========================================>");
                Debug.WriteLine(area.ToString());
                var sub = new testModel2() {info = "I'm good"};
                var i2 = Cache.GetItem<testModel>(area, "test2" + area.ToString(), () => new testModel() { Name = "Chris", Age = 40, LastSeen = null, subtest = sub });
                Debug.WriteLine(i2.Name + " - " + i2.Age + " - " + i2.LastSeen + " - " + i2.subtest.info);
                Cache.SetItem<testModel>(area, "test2" + area.ToString(), new testModel() { Name = "Rachel", Age = 42, LastSeen = DateTime.Now, subtest = sub });
                sub.info = "UCIT";
                i2 = Cache.GetItem<testModel>(area, "test2" + area.ToString(), () => new testModel() { Name = "Sarah", Age = 5, LastSeen = DateTime.Now.AddDays(-200), subtest = sub });
                Debug.WriteLine(i2.Name + " - " + i2.Age + " - " + i2.LastSeen + " - " + i2.subtest.info);
                Debug.WriteLine("<==========================================>");
                Debug.WriteLine("");
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}