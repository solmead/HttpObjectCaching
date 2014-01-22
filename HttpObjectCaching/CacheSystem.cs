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

namespace HttpObjectCaching
{
    public class CacheSystem
    {
        public delegate void SessionEvent(string sessionId);
        public event SessionEvent SessionExpired;

        private object sessionCreateLock = new object();
        private static object CacheSystemCreateLock = new object();

        private string _sessionId = "";

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

        public string SessionId
        {
            get
            {
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
            set { _sessionId = value; }
        }

        public void ClearSession()
        {
            var sess = HttpRuntime.Cache["Session_" + SessionId] as NameValueCollection;
            if (sess != null)
            {
                HttpRuntime.Cache.Remove("Session_" + SessionId);
            }
        }

        public void ReportRemovedCallback(String key, object value,
        CacheItemRemovedReason removedReason)
        {
            
        }

        public Dictionary<string, object> Session
        {
            get {
                var sess = HttpRuntime.Cache["Session_" + SessionId] as Dictionary<string, object>;
                if (sess == null)
                {
                    lock (sessionCreateLock)
                    {
                        sess = HttpRuntime.Cache["Session_" + SessionId] as Dictionary<string, object>;
                        if (sess == null)
                        {
                            sess = new Dictionary<string, object>();
                            HttpRuntime.Cache.Insert("Session_" + SessionId, sess, null,
                                System.Web.Caching.Cache.NoAbsoluteExpiration,
                                new TimeSpan(0, 30, 0),
                                CacheItemPriority.Default,
                                this.ReportRemovedCallback);
                        }
                    }
                }
                return sess;
            }
        }
    }
}
