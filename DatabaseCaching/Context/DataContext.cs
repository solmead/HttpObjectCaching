using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DatabaseCaching.Models;
using HttpObjectCaching;

namespace DatabaseCaching.Context
{
    class DataContext : DbContext 
    {

        public DataContext()
            : base("DbCachingConnection")
        {
            
        }


        public static void UpgradeDB()
        {
            Database.SetInitializer<DataContext>(null);
            try
            {
                var dbMigrator = new DbMigrator(new DatabaseCaching.Migrations.Configuration());
                if (dbMigrator.GetPendingMigrations().Count() > 0)
                {
                    dbMigrator.Update();
                    dbMigrator.Update();
                }

            }
            catch (Exception ex)
            {
                throw ex;
                Debug.WriteLine(ex.ToString());
            }
        }
        public ObjectContext ObjectContext
        {
            get { return ((IObjectContextAdapter)this).ObjectContext; }
        }
        public static void ClearContext()
        {
            DataContext context = null;
            Cache.SetItem(CacheArea.Request, "DbCachingDataContext", context);
        }
        public static DataContext Current
        {
            get
            {
                return Cache.GetItem<DataContext>(CacheArea.Request, "DbCachingDataContext", () => new DataContext());
            }
        }

        public DbSet<CachedEntry> CachedEntries { get; set; }
    }
}
