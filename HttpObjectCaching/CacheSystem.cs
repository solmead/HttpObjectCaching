using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        public List<TaggedCacheEntry> TaggedEntries { get; set; } = new List<TaggedCacheEntry>();

        public bool CacheEnabled { get; set; }

        public void AddTaggedEntry(CacheArea cacheArea, string tags, string entryName)
        {
            if (string.IsNullOrWhiteSpace(tags) || string.IsNullOrWhiteSpace(entryName))
            {
                return;
            }
            try
            {
                var te = (from t in TaggedEntries.ToList()
                          where t.CacheArea == cacheArea && t.EntryName?.ToUpper() == entryName?.ToUpper()
                          select t).FirstOrDefault();

                if (te == null)
                {
                    te = new TaggedCacheEntry()
                    {
                        CacheArea = cacheArea,
                        EntryName = entryName,
                        Tags = ""
                    };
                    TaggedEntries.Add(te);
                }

                if (string.IsNullOrWhiteSpace(tags))
                {
                    tags = "";
                }
                if (string.IsNullOrWhiteSpace(te.Tags))
                {
                    te.Tags = "";
                }




                var tarr = (tags.ToUpper() + "," + te.Tags.ToUpper()).Split(',').ToList();
                tarr = (from t in tarr where !string.IsNullOrWhiteSpace(t) select t).Distinct().ToList();
                tarr.Insert(0, "");
                tarr.Add("");
                te.Tags = String.Join(",", tarr);
            }
            catch (Exception ex)
            {
                
            }

        }


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



        private static void SetCookieValue(string name, string value)
        {
            Cache.SetItem<string>(CacheArea.Request, "_" + name + "Id", value);
        }

        private static string GetCookieValue(string name, bool isPerminate)
        {
            var _cookieId = Cache.GetItem<string>(CacheArea.Request, "_" + name + "Id", () => Guid.NewGuid().ToString());
            var context = HttpContext.Current;
            if (context != null)
            {
                var baseData = "";
                HttpCookie cookie = null;
                try
                {
                    cookie = context.Request.Cookies["_" + name + "_Cache"];
                }
                catch (Exception)
                {

                }

                //_cookieId = MachineKey.Unprotect(Convert.FromBase64String(cookie?.Value), HttpContext.Current.Request.UserHostAddress);
                var validCookie = false;
                if (cookie != null)
                {
                    try
                    {
                        baseData = cookie.Value;

                        var cdata = MachineKey.Unprotect(Convert.FromBase64String(baseData), HttpContext.Current.Request.UserHostAddress);
                        if (cdata != null)
                        {
                            _cookieId = Encoding.UTF8.GetString(cdata);
                            validCookie = true;
                        }
                    }
                    catch
                    {

                    }
                }
                if (cookie == null || !validCookie)
                {
                    try
                    {
                        var cdata = Encoding.UTF8.GetBytes(_cookieId);
                        baseData = Convert.ToBase64String(MachineKey.Protect(cdata,
                                HttpContext.Current.Request.UserHostAddress));


                        cookie = new HttpCookie("_" + name + "_Cache", baseData);
                        //cookie.HttpOnly = true;
                        cookie.Path = FormsAuthentication.FormsCookiePath;
                        cookie.Secure = string.Equals("https", HttpContext.Current.Request.Url.Scheme, StringComparison.OrdinalIgnoreCase);
                        if (isPerminate)
                        {
                            cookie.Expires = DateTime.Now.AddYears(3);
                        }
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
            Cache.SetItem<string>(CacheArea.Request, "_" + name + "Id", _cookieId);
            return _cookieId;
        }


        public static async Task<string> CookieIdAsync()
        {
            return GetCookieValue("my_cook", false);
        }
        public static async Task CookieIdSetAsync(string value)
        {
            SetCookieValue("my_cook", value);
        }
        public static string CookieId()
        {
            return GetCookieValue("my_cook", true);
        }
        public static void CookieIdSet(string value)
        {
            SetCookieValue("my_cook", value);
        }
        public static async Task<string> SessionIdAsync()
        {
            return GetCookieValue("my_sess", false);
        }

        public static async Task SessionIdSetAsync(string value)
        {
            SetCookieValue("my_sess", value);
        }
        public static string SessionId()
        {
            return GetCookieValue("my_sess", false);
        }

        public static void SessionIdSet(string value)
        {
            SetCookieValue("my_sess", value);
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

        public List<TaggedCacheEntry> GetTaggedCacheEntries(string tag)
        {
            return (from t in TaggedEntries
                where t.Tags.ToUpper().Contains("," + tag.ToUpper() + ",")
                select t).ToList();
        }

        public async Task ClearTaggedCacheAsync(string tag)
        {
            var tList = GetTaggedCacheEntries(tag);
            foreach (var t in tList)
            {
                await Cache.SetItemAsync<object>(t.CacheArea, t.EntryName, null);
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

        public void ClearTaggedCache(string tag)
        {
            var tList = GetTaggedCacheEntries(tag);
            foreach (var t in tList)
            {
                Cache.SetItem<object>(t.CacheArea, t.EntryName, null);
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
