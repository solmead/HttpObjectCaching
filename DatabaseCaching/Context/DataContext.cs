﻿using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DatabaseCaching.Migrations;
using DatabaseCaching.Models;
using DatabaseCaching.Properties;
using HttpObjectCaching;

namespace DatabaseCaching.Context
{
    class DataContext : DbContext
    {
        //private static string dbfile = Settings.Default.DBDirectory + Settings.Default.BaseDBName + (Settings.Default.UseServerName ? "_" + Environment.MachineName : "") + ".sdf";
        //private static FileInfo fi = new FileInfo(System.Web.Hosting.HostingEnvironment.MapPath(dbfile));
        //private static string cecs = "Data Source=" + fi.FullName;
        //private static string cs = "Provider=System.Data.SqlServerCe.4.0; " + cecs;

        public DataContext()
            : base(Settings.Default.ConnectionName)
        {

        }


        public static void UpgradeDB()
        {
            Database.SetInitializer<DataContext>(null);
            try
            {
                var configuration = new Configuration();
                configuration.TargetDatabase = new DbConnectionInfo(Settings.Default.ConnectionName);
                var dbMigrator = new DbMigrator(configuration);
            
                if (dbMigrator.GetPendingMigrations().Count() > 0)
                {
                    dbMigrator.Update();
                    dbMigrator.Update();
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                throw ex;
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
