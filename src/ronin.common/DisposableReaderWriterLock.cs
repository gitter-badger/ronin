using System;
using System.Threading;

namespace Ronin.Common
{
    public class DisposableReaderWriterLock
    {
        private readonly ReaderWriterLockSlim lockSlim;

        public DisposableReaderWriterLock(LockRecursionPolicy recursionPolicy)
        {
            this.lockSlim = new ReaderWriterLockSlim(recursionPolicy);
        }

        public DisposableReaderWriterLock()
            : this(LockRecursionPolicy.NoRecursion)
        {
        }

        public LockReleaser AquireReadLock()
        {
            return new LockReleaser(this.lockSlim, true);
        }

        public LockReleaser AquireWriteLock()
        {
            return new LockReleaser(this.lockSlim, false);
        }

        public UpgreadableLockReleaser AquireUpgreadableReadLock()
        {
            return new UpgreadableLockReleaser(this.lockSlim);
        }
    }

    public class LockReleaser : IDisposable
    {
        private readonly ReaderWriterLockSlim lockSlim;
        private readonly bool isRead;

        public LockReleaser(ReaderWriterLockSlim lockSlim, bool isRead)
        {
            this.lockSlim = lockSlim;
            this.isRead = isRead;

            if (isRead)
                this.lockSlim.EnterReadLock();
            else
                this.lockSlim.EnterWriteLock();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposing || this.lockSlim == null) return;
            if (isRead)
                this.lockSlim.ExitReadLock();
            else
                this.lockSlim.ExitWriteLock();
        }
    }

    public class UpgreadableLockReleaser : IDisposable
    {
        private readonly ReaderWriterLockSlim lockSlim;
        public UpgreadableLockReleaser(ReaderWriterLockSlim lockSlim)
        {
            this.lockSlim = lockSlim;
            this.lockSlim.EnterUpgradeableReadLock();
        }

        public LockReleaser AquireReadLock()
        {
            return new LockReleaser(this.lockSlim, true);
        }

        public LockReleaser AquireWriteLock()
        {
            return new LockReleaser(this.lockSlim, false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposing || this.lockSlim == null) return;
            this.lockSlim.ExitUpgradeableReadLock();
        }
    }
}
