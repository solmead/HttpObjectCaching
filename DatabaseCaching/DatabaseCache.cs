using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls.WebParts;
using HttpObjectCaching.Core;
using HttpObjectCaching.Core.DataSources;
using SqlCeDatabaseCaching.Context;
using SqlCeDatabaseCaching.Helpers;
using SqlCeDatabaseCaching.Models;
using SqlCeDatabaseCaching.Properties;
using HttpObjectCaching;
using HttpObjectCaching.CacheAreas;

namespace HttpObjectCaching.Caches
{
    public class DatabaseCache : DataCache
    {
        public DatabaseCache() : base(new SqlCeDataSource())
        {

        }

        public override CacheArea Area { get { return CacheArea.Permanent; } }
        public override string Name { get { return "DatabaseDefault"; } }
        
    }
}
