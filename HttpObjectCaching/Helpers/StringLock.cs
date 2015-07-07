using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpObjectCaching.Helpers
{
    public class StringLock
    {
        private readonly Dictionary<string, LockObject> keyLocks = new Dictionary<string, LockObject>();
        private readonly object keyLocksLock = new object();

        private StringLock()
        {
            
        }
        private static readonly Lazy<StringLock> lazyStringLock = new Lazy<StringLock>(() => new StringLock());
        public static StringLock Lock
        {
            get { return lazyStringLock.Value; }
        }

        public IDisposable AcquireLock(string key)
        {
            LockObject obj;
            lock (keyLocksLock)
            {
                if (!keyLocks.TryGetValue(key,
                                          out obj))
                {
                    keyLocks[key] = obj = new LockObject(key);
                }
                obj.Withdraw();
            }
            Monitor.Enter(obj);
            return new DisposableToken(this,
                                       obj);
        }

        private void ReturnLock(DisposableToken disposableLock)
        {
            var obj = disposableLock.LockObject;
            lock (keyLocksLock)
            {
                if (obj.Return())
                {
                    keyLocks.Remove(obj.Key);
                }
                Monitor.Exit(obj);
            }
        }

        private class DisposableToken : IDisposable
        {
            private readonly LockObject lockObject;
            private readonly StringLock stringLock;
            private bool disposed;

            public DisposableToken(StringLock stringLock, LockObject lockObject)
            {
                this.stringLock = stringLock;
                this.lockObject = lockObject;
            }

            public LockObject LockObject
            {
                get
                {
                    return lockObject;
                }
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            ~DisposableToken()
            {
                Dispose(false);
            }

            private void Dispose(bool disposing)
            {
                if (disposing && !disposed)
                {
                    stringLock.ReturnLock(this);
                    disposed = true;
                }
            }
        }

        private class LockObject
        {
            private readonly string key;
            private int leaseCount;

            public LockObject(string key)
            {
                this.key = key;
            }

            public string Key
            {
                get
                {
                    return key;
                }
            }

            public void Withdraw()
            {
                Interlocked.Increment(ref leaseCount);
            }

            public bool Return()
            {
                return Interlocked.Decrement(ref leaseCount) == 0;
            }
        }
    }
}
