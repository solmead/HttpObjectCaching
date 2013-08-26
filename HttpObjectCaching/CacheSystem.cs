using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace HttpObjectCaching
{
    public class CacheSystem
    {
        public delegate void SessionEvent(string sessionId);
        public event SessionEvent SessionExpired;

        private string _sessionId = "";

        private CacheSystem()
        {
            
        }

        public static CacheSystem Instance
        {
            get
            {
                var context = HttpContext.Current;
                if (context != null)
                {
                    if (context.Items.Contains("CurrentCacheInstance"))
                    {
                        var ctx = (CacheSystem) context.Items["CurrentCacheInstance"];
                        return ctx;
                    }
                    else
                    {
                        var ctx = new CacheSystem();
                        context.Items.Add("CurrentCacheInstance",ctx);
                        return ctx;
                    }
                }
                else
                {
                    try
                    {
                        var t = (CacheSystem)Thread.GetData(Thread.GetNamedDataSlot("CurrentCacheInstance"));
                        if (t == null)
                        {
                            t = new CacheSystem();
                            Thread.SetData(Thread.GetNamedDataSlot("CurrentCacheInstance"), t);
                        }
                        return t;
                    }
                    catch
                    { 
                        var t = new CacheSystem();
                        Thread.SetData(Thread.GetNamedDataSlot("CurrentCacheInstance"), t);
                        return t;
                    }
                }
            }
        }

        public string SessionId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_sessionId))
                {
                    var context = HttpContext.Current;
                    if (context != null && context.Session != null)
                    {
                        _sessionId = context.Session.SessionID;
                    }
                    else
                    {
                        _sessionId = Guid.NewGuid().ToString();
                    }
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
                    sess = new Dictionary<string, object>();
                    HttpRuntime.Cache.Insert("Session_" + SessionId, sess, null,
                                             System.Web.Caching.Cache.NoAbsoluteExpiration,
                                             new TimeSpan(0, 30, 0),
                                             CacheItemPriority.Default,
                                                this.ReportRemovedCallback);
                }
                return sess;
            }
        }



        public tt GetFromThread<tt>(string name)
        {
            try
            {
                var t = (tt)Thread.GetData(Thread.GetNamedDataSlot(name.ToUpper()));
                return t;
            }
            catch
            {
                
            }
            return default(tt);
        }

        public void SetInThread<tt>(string name, tt obj)
        {
            Thread.SetData(Thread.GetNamedDataSlot(name.ToUpper()), obj);
        }


        public tt GetFromApplication<tt>(string name)
        {
            try
            {
                var t = (tt)HttpRuntime.Cache[name.ToUpper()];
                return t;
            }
            catch
            {
                
            }
            return default(tt);
        }

        public void SetInApplication<tt>(string name, tt obj)
        {
            if (obj != null)
            {
                HttpRuntime.Cache[name.ToUpper()] = obj;
            }
            else
            {
                HttpRuntime.Cache.Remove(name.ToUpper());
            }
        }


        public tt GetFromRequest<tt>(string name)
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                if (context.Items.Contains(name.ToUpper()))
                {
                    var ctx = (tt)context.Items[name.ToUpper()];
                    return ctx;
                }
                return default(tt);
            }
            else {
                return GetFromThread<tt>(name.ToUpper());
            }
        }

        public void SetInRequest<tt>(string name, tt obj)
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                if (context.Items.Contains(name.ToUpper()))
                {
                    context.Items.Remove(name.ToUpper());
                }
                context.Items.Add(name.ToUpper(), obj);
            }
            else
            {
                SetInThread(name, obj);
            }
        }
        
        public tt GetFromSession<tt>(string name)
        {
            if (Session.ContainsKey(name.ToUpper()))
            {
                return (tt)Session[name.ToUpper()];
            }
            return default(tt);
        }

        public void SetInSession<tt>(string name, tt obj)
        {
            if (Session.ContainsKey(name.ToUpper()))
            {
                Session.Remove(name.ToUpper());
            }
            Session.Add(name.ToUpper(), obj);
        }

    }
}
