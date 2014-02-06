using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using HttpObjectCaching.CacheAreas;
using HttpObjectCaching.CacheAreas.Caches;

namespace HttpObjectCaching
{
    public class CacheSystem
    {
        private static object CacheSystemCreateLock = new object();

        //private string _sessionId = "";
        //private string _cookieId = "";

        public Dictionary<CacheArea, ICacheArea> CacheAreas { get; private set; }

        public bool CacheEnabled { get; set; }

        private CacheSystem()
        {
            CacheEnabled = true;

            var cList = (from c in GetCacheAreaList() where c.Name.Contains("Default") select c).ToList();
            var keys = (from c in cList select c.Area).Distinct().ToList();
            CacheAreas = keys
                .Select(item => new { Key = item , Item = (from c in cList where c.Area==item select c).First() })
                .ToDictionary(i => i.Key, i=>i.Item);
        }

        private static IEnumerable<ICacheArea> GetCacheAreaList()
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from t in assembly.GetTypes()
                    where t.GetInterfaces().Contains(typeof(ICacheArea)) &&
                          t.GetConstructor(Type.EmptyTypes) != null
                    select ((ICacheArea)Activator.CreateInstance(t))).ToList();
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

        public string CookieId
        {
            get
            {
                var _cookieId = Cache.GetItem<string>(CacheArea.Thread, "_cookieId", () => Guid.NewGuid().ToString());
                var context = HttpContext.Current;
                if (context != null)
                {
                    var cookie = context.Request.Cookies["cookieCache"];
                    if (cookie != null && !string.IsNullOrWhiteSpace(cookie.Value))
                    {
                        _cookieId = cookie.Value;
                    }
                    else
                    {
                        context.Response.SetCookie(new HttpCookie("cookieCache",_cookieId));
                    }
                }
                return _cookieId;
            }
            set { Cache.SetItem<string>(CacheArea.Thread, "_cookieId",  value); }
        }

        public string SessionId
        {
            get
            {
                var _sessionId = Cache.GetItem<string>(CacheArea.Thread, "_sessionId", () => Guid.NewGuid().ToString());
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
            set {Cache.SetItem<string>(CacheArea.Thread, "_sessionId",  value); }
        }

        [Obsolete("ClearSession is deprecated, please use ClearCache(CacheArea.Session) instead.")]
        public void ClearSession()
        {
            GetCacheArea(CacheArea.Session).ClearCache();
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
                return nvl.DataDictionary;
            }
            return null;
        }
        [Obsolete("Session is deprecated, please use GetDataDictionary(CacheArea.Session) instead.")]
        public IDictionary<string, object> Session
        {
            get { return GetDataDictionary(CacheArea.Session); }
        }
    }
}
