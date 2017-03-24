using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpObjectCaching.Core;
using HttpObjectCaching.Core.DataSources;
using HttpObjectCaching.Core.Extras;

namespace HttpObjectCaching.Caches
{
    public class PermanentCache : DataCache
    {

        public IPermanentRepository Repository
        {
            get
            {
                var cDB = baseDataSource.CacheDatabase;
                if (cDB == null)
                {
                    var cname = CacheSystem._Config.PermanentElement.Class;
                    try
                    {
                        cDB = Activator.CreateInstance(Type.GetType(cname)) as IPermanentRepository;
                    }
                    catch
                    {
                        
                    }


                    baseDataSource.CacheDatabase = cDB;
                }

                return baseDataSource.CacheDatabase;
            }
            set { baseDataSource.CacheDatabase = value; }
        }
        private PermanentDataSource baseDataSource { get; set; }


        public PermanentCache()
            : base(new PermanentDataSource())
        {
            Area = CacheArea.Permanent;
            Name = "BasePermanent";

            baseDataSource = base.DataSource as PermanentDataSource;
            var repo = Repository;

        }



    }
}
