using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseCaching.Helpers;
using DatabaseCaching.Models;
using HttpObjectCaching.Core.Extras;

namespace DatabaseCaching.Context
{
    public class StoreInDatabase : IPermanentRepository
    {
        public StoreInDatabase()
        {
            DataContext.UpgradeDB();
        }


        private void CleanOutTimeOutValues(DataContext database)
        {
                try
                {

                    var lst = (from ce in database.CachedEntries
                         where ce.TimeOut.HasValue && ce.TimeOut.Value < DateTime.Now
                         select ce).ToList();
                    if (lst.Count > 0)
                    {
                        database.CachedEntries.RemoveRange(lst);
                        database.SaveChanges();
                    }
                }
                catch (Exception ex)
                {

                }
        }
        private async Task CleanOutTimeOutValuesAsync(DataContext database)
        {
                try
                {
                    var lst = await
                        (from ce in database.CachedEntries
                         where ce.TimeOut.HasValue && ce.TimeOut.Value < DateTime.Now
                         select ce).ToListAsync();
                    if (lst.Count > 0)
                    {
                        database.CachedEntries.RemoveRange(lst);
                        await database.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {

                }
        }




        public async Task<byte[]> GetAsync(string name)
        {
            CachedEntry itm;
            //lock (_lock)
            {
                using (var database = new DataContext())
                {
                    itm = await(from ce in database.CachedEntries where ce.Name == name select ce).FirstOrDefaultAsync();
                }
            }
            if (itm != null && itm.TimeOut.HasValue && itm.TimeOut.Value >= DateTime.Now)
            {
                var xml = itm.Object;
                try
                {
                    if (!string.IsNullOrWhiteSpace(xml))
                    {
                        var o = Convert.FromBase64String(xml);
                        return o;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
            
            return new byte[0];
        }

        public async Task SetAsync(string name, byte[] value, TimeSpan? timeout)
        {

            var xml = Convert.ToBase64String(value);

            using (var database = new DataContext())
            {
                var itm =
                    await (from ce in database.CachedEntries where ce.Name == name select ce).FirstOrDefaultAsync();

                if (value.Length == 0)
                {
                    if (itm != null)
                    {
                        database.CachedEntries.Remove(itm);
                    }
                }
                else
                {
                    DateTime? endTime = null;
                    if (timeout.HasValue)
                    {
                        endTime = DateTime.Now.AddSeconds(timeout.Value.TotalSeconds);
                    }
                    if (itm == null)
                    {
                        itm = new CachedEntry()
                        {
                            Created = DateTime.Now,
                            Name = name,
                            Object = ""
                        };
                        database.CachedEntries.Add(itm);
                    }
                    itm.TimeOut = endTime;
                    itm.Changed = DateTime.Now;
                    itm.Object = xml;
                }


                await database.SaveChangesAsync();

                //await CleanOutTimeOutValuesAsync(database);
            }

        }

        public async Task DeleteAsync(string name)
        {
            using (var database = new DataContext())
            {
                var lst = await(from ce in database.CachedEntries where ce.Name == name select ce).ToListAsync();
                if (lst.Count > 0)
                {
                    database.CachedEntries.RemoveRange(lst);
                }
                await database.SaveChangesAsync();
            }
        }

        public Task<List<string>> GetKeysAsync()
        {
            throw new NotImplementedException();
        }

        public byte[] Get(string name)
        {

            CachedEntry itm;
            //lock (_lock)
            {
                using (var database = new DataContext())
                {
                    itm = (from ce in database.CachedEntries where ce.Name == name select ce).FirstOrDefault();
                }
            }
            if (itm != null && itm.TimeOut.HasValue && itm.TimeOut.Value >= DateTime.Now)
            {
                var xml = itm.Object;
                try
                {
                    if (!string.IsNullOrWhiteSpace(xml))
                    {
                        var o = Convert.FromBase64String(xml);
                        return o;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

            return new byte[0];
        }

        public void Set(string name, byte[] value, TimeSpan? timeout)
        {
            var xml = Convert.ToBase64String(value);

            using (var database = new DataContext())
            {
                var itm =
                    (from ce in database.CachedEntries where ce.Name == name select ce).FirstOrDefault();

                if (value.Length == 0)
                {
                    if (itm != null)
                    {
                        database.CachedEntries.Remove(itm);
                    }
                }
                else
                {
                    DateTime? endTime = null;
                    if (timeout.HasValue)
                    {
                        endTime = DateTime.Now.AddSeconds(timeout.Value.TotalSeconds);
                    }
                    if (itm == null)
                    {
                        itm = new CachedEntry()
                        {
                            Created = DateTime.Now,
                            Name = name,
                            Object = ""
                        };
                        database.CachedEntries.Add(itm);
                    }
                    itm.TimeOut = endTime;
                    itm.Changed = DateTime.Now;
                    itm.Object = xml;
                }


                database.SaveChanges();
                //CleanOutTimeOutValues(database);
            }

        }

        public void Delete(string name)
        {
            using (var database = new DataContext())
            {
                var lst = (from ce in database.CachedEntries where ce.Name == name select ce).ToList();
                if (lst.Count > 0)
                {
                    database.CachedEntries.RemoveRange(lst);
                }
                database.SaveChanges();
            }
        }

        public List<string> GetKeys()
        {
            throw new NotImplementedException();
        }
    }
}
