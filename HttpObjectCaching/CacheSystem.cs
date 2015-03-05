using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using HttpObjectCaching.CacheAreas;
using HttpObjectCaching.Core.Configuration;
using HttpObjectCaching.Helpers;

namespace HttpObjectCaching
{
    public class CacheSystem
    {
        private static object CacheSystemCreateLock = new object();

        //private string _sessionId = "";
        //private string _cookieId = "";
        public static CacheRetrieverSection _Config = ConfigurationManager.GetSection("HttpObjectCachingAreas") as CacheRetrieverSection;


        public Dictionary<CacheArea, ICacheArea> CacheAreas { get; private set; }

        public bool CacheEnabled { get; set; }

        private CacheSystem()
        {
            CacheEnabled = true;

            var cList = _Config.Entries;

            CacheAreas = new Dictionary<CacheArea, ICacheArea>();

            foreach (CacheElement c in cList)
            {
                if (!string.IsNullOrWhiteSpace(c.Class))
                {
                    var obj = Activator.CreateInstance(Type.GetType(c.Class)) as ICacheArea;
                    if (obj != null)
                    {
                        CacheAreas.Add(c.Area, obj);
                    }
                }
            }
            //var cList = (from c in GetCacheAreaList() where c.Name.Contains("Default") select c).ToList();
            //var keys = (from c in cList select c.Area).Distinct().ToList();
            //CacheAreas = (from CacheElement c in cList select new { Key = c.Area, Item = (ICacheArea)Activator.CreateInstance(Type.GetType(c.Class)) })
            //    .ToDictionary(i => i.Key, i=>i.Item);
        }

        //private static IEnumerable<ICacheArea> GetCacheAreaList()
        //{
        //    var list = new List<ICacheArea>();
        //    var assemblyList = AppDomain.CurrentDomain.GetAssemblies();
        //    foreach (var assembly in assemblyList)
        //    {
        //        try
        //        {
        //            var typeList = assembly.GetTypes();
        //            var tlst = (from t in typeList
        //                        where t.GetInterfaces().Contains(typeof(ICacheArea)) &&
        //                            t.GetConstructor(Type.EmptyTypes) != null
        //                        select ((ICacheArea)Activator.CreateInstance(t))).ToList();
        //            if (tlst.Any())
        //            {
        //                list.AddRange(tlst);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine("Issue with " + assembly.FullName);
        //        }
        //    }
        //    //return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
        //    //        from t in assembly.GetTypes()
        //    //        where t.GetInterfaces().Contains(typeof(ICacheArea)) &&
        //    //              t.GetConstructor(Type.EmptyTypes) != null
        //    //        select ((ICacheArea)Activator.CreateInstance(t))).ToList();
        //    return list;
        //}

        public ICacheArea GetCacheArea(CacheArea area)
        {
            if (CacheAreas.ContainsKey(area))
            {
                return CacheAreas[area];
            }
            //area doesn't exist, go through each level till we find a level that works.
            var maxval = (from int v in Enum.GetValues(typeof (CacheArea)) select v).Max();
            for (var a = (int)area; a<= maxval; a++)
            {
                var ea = (CacheArea) a;
                if (CacheAreas.ContainsKey(ea))
                {
                    return CacheAreas[ea];
                }
            }

            return null;
        }


        public static CacheSystem Instance
        {

            get
            {
                var ctx = HttpRuntime.Cache["CurrentCacheInstance"] as CacheSystem;
                if (ctx == null)
                {
                    lock (CacheSystemCreateLock)
                    {
                        ctx = HttpRuntime.Cache["CurrentCacheInstance"] as CacheSystem;
                        if (ctx == null)
                        {
                            ctx = new CacheSystem();
                            HttpRuntime.Cache.Insert("CurrentCacheInstance", ctx);
                        }
                    }
                }
                return ctx;
            }
        }

        public static async Task<string> CookieId()
        {
                var _cookieId = await Cache.GetItemAsync<string>(CacheArea.Request, "_cookieId",  () => Guid.NewGuid().ToString());
                var context = HttpContext.Current;
                if (context != null)
                {
                    HttpCookie cookie = null;
                    try
                    {
                        cookie = context.Request.Cookies["cookieCache"];
                    }
                    catch (Exception)
                    {

                    }
                    if (cookie != null && !string.IsNullOrWhiteSpace(cookie.Value))
                    {
                        _cookieId = cookie.Value;
                    }
                    else
                    {
                        try
                        {
                            cookie = new HttpCookie("cookieCache", _cookieId);
                            cookie.HttpOnly = true;
                            cookie.Path = FormsAuthentication.FormsCookiePath;
                            cookie.Secure = string.Equals("https", HttpContext.Current.Request.Url.Scheme, StringComparison.OrdinalIgnoreCase);
                            if (HttpContext.Current.Request.Url.Host.Split('.').Length > 2)
                            {
                                cookie.Domain = HttpContext.Current.Request.Url.Host;
                            }
                            context.Response.SetCookie(cookie);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                return _cookieId;
            }
        public static async Task CookieIdSet(string value) { 
            await Cache.SetItemAsync<string>(CacheArea.Request, "_cookieId", value); 
        }

        public static async Task<string> SessionId()
        {
            var _sessionId = await Cache.GetItemAsync<string>(CacheArea.Request, "_sessionId", () => Guid.NewGuid().ToString());
            var context = HttpContext.Current;
            if (context != null && context.Session != null)
            {
                _sessionId = context.Session.SessionID;
                context.Session["__MySessionLock"] = "112233";
            }
            if (string.IsNullOrWhiteSpace(_sessionId))
            {
                _sessionId = Guid.NewGuid().ToString();
            }
            return _sessionId;
        }

        public static async Task SessionIdSet(string value)
        {
            await Cache.SetItemAsync<string>(CacheArea.Request, "_sessionId", value);
        }
        

        public async Task ClearAllCacheAreas()
        {
            foreach (var area in CacheAreas.Keys)
            {
                try
                {
                    await GetCacheArea(area).ClearCacheAsync();
                }
                catch (NotImplementedException)
                {
                    
                }
            }
        }
        public async Task ClearCache(CacheArea area)
        {
            await GetCacheArea(area).ClearCacheAsync();
        }

        public async Task<IDictionary<string, object>> GetDataDictionary(CacheArea area)
        {
            var nvl = GetCacheArea(area) as INameValueLister;
            if (nvl != null)
            {
                return await nvl.DataDictionaryGet();
            }
            return null;
        }
    }
}
