using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using HttpObjectCaching.CacheAreas;
using HttpObjectCaching.Core;
using HttpObjectCaching.Core.Configuration;
using HttpObjectCaching.Core.DataSources;
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
            //if (!CacheAreas.ContainsKey(CacheArea.Local))
            //{
            //    CacheAreas.Add(CacheArea.Local, new DataCache(new LocalDataSource(1)));
            //} 
        }


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

        public static async Task<string> CookieIdAsync()
        {
                var _cookieId = await Cache.GetItemAsync<string>(CacheArea.Request, "_cookieId",async () => Guid.NewGuid().ToString());
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
        public static async Task CookieIdSetAsync(string value) { 
            await Cache.SetItemAsync<string>(CacheArea.Request, "_cookieId", value); 
        }

        public static string CookieId()
        {
            var _cookieId =  Cache.GetItem<string>(CacheArea.Request, "_cookieId", () => Guid.NewGuid().ToString());
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
        public static void CookieIdSet(string value)
        {
            Cache.SetItem<string>(CacheArea.Request, "_cookieId", value);
        }
        public static async Task<string> SessionIdAsync()
        {
            var _sessionId = await Cache.GetItemAsync<string>(CacheArea.Request, "_sessionId",async () => Guid.NewGuid().ToString());
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

        public static async Task SessionIdSetAsync(string value)
        {
            await Cache.SetItemAsync<string>(CacheArea.Request, "_sessionId", value);
        }
        public static string SessionId()
        {
            var _sessionId =  Cache.GetItem<string>(CacheArea.Request, "_sessionId", () => Guid.NewGuid().ToString());
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

        public static void SessionIdSet(string value)
        {
            Cache.SetItem<string>(CacheArea.Request, "_sessionId", value);
        }

        public async Task ClearAllCacheAreasAsync()
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
        public async Task ClearCacheAsync(CacheArea area)
        {
            await GetCacheArea(area).ClearCacheAsync();
        }

        public async Task<IDictionary<string, object>> GetDataDictionaryAsync(CacheArea area)
        {
            var nvl = GetCacheArea(area) as INameValueLister;
            if (nvl != null)
            {
                return await nvl.DataDictionaryGetAsync();
            }
            return null;
        }
        public void ClearAllCacheAreas()
        {
            foreach (var area in CacheAreas.Keys)
            {
                try
                {
                     GetCacheArea(area).ClearCache();
                }
                catch (NotImplementedException)
                {

                }
            }
        }
        public void ClearCache(CacheArea area)
        {
            GetCacheArea(area).ClearCache();
        }

        public IDictionary<string, object> GetDataDictionary(CacheArea area)
        {
            var nvl = GetCacheArea(area) as INameValueLister;
            if (nvl != null)
            {
                return  nvl.DataDictionaryGet();
            }
            return null;
        }
    }
}
