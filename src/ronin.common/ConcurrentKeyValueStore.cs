using System.Collections.Generic;

namespace Ronin.Common
{
    public class ConcurrentKeyValueStore<TKey, TContent>
    {
        protected readonly IDictionary<TKey, TContent> Repository;
        protected readonly DisposableReaderWriterLock ReaderWriterLock;

        public ConcurrentKeyValueStore()
        {
            this.Repository = new Dictionary<TKey, TContent>();
            this.ReaderWriterLock = new DisposableReaderWriterLock();
        }

        public ICollection<TKey> Keys => this.Repository.Keys;

        public void AddOrUpdate(TKey key, TContent content)
        {
            using (this.ReaderWriterLock.AquireWriteLock())
            {
                if (this.Repository.ContainsKey(key))
                {
                    this.Repository[key] = content;
                }
                else
                {
                    this.Repository.Add(key, content);
                }
            }
        }

        public TContent GetOrAdd(TKey key, TContent content)
        {
            using (var context = this.ReaderWriterLock.AquireUpgreadableReadLock())
            {
                TContent existingContent;
                if (this.Repository.TryGetValue(key, out existingContent))
                {
                    return existingContent;
                }
                else
                {
                    using (context.AquireWriteLock())
                    {
                        if (this.Repository.TryGetValue(key, out existingContent))
                        {
                            return existingContent;
                        }
                        else
                        {
                            this.Repository.Add(key, content);
                            return content;
                        }
                    }
                }
            }
        }

        public bool TryGet(TKey key, out TContent content)
        {
            using (this.ReaderWriterLock.AquireReadLock())
            {
                return this.Repository.TryGetValue(key, out content);
            }
        }

        public IEnumerable<TContent> GetAll()
        {
            using (this.ReaderWriterLock.AquireReadLock())
            {
                return this.Repository.Values;
            }
        }

        public void Add(TKey key, TContent content)
        {
            using (this.ReaderWriterLock.AquireWriteLock())
            {
                this.Repository.Add(key, content);
            }
        }

        public void Set(TKey key, TContent content)
        {
            using (this.ReaderWriterLock.AquireWriteLock())
            {
                if (this.Repository.ContainsKey(key))
                {
                    this.Repository[key] = content;
                }
                else
                {
                    this.Repository.Add(key, content);
                }
            }
        }
    }
}
