using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TYMDLL
{
    internal class GroupQueue<T>
    {
        private readonly Queue<T> _queue;
        private readonly int _maxSize;
        private readonly object _enqueueLock = new object();
        private readonly object _dequeueLock = new object();
        private bool _isCompleted = false;

        public GroupQueue(int maxSize)
        {
            if (maxSize <= 0)
                throw new ArgumentException("Queue size must be greater than zero.", nameof(maxSize));

            _maxSize = maxSize;
            _queue = new Queue<T>(maxSize);
        }

        public void Enqueue(T item)
        {
            lock (_enqueueLock)
            {
                while (_queue.Count >= _maxSize && !_isCompleted)
                {
                    Monitor.Wait(_enqueueLock);
                }

                if (_isCompleted)
                    throw new InvalidOperationException("Queue has been marked as complete.");

                lock (_queue) // 只锁定队列操作
                {
                    _queue.Enqueue(item);
                }

                // 通知可能正在等待的消费者
                lock (_dequeueLock)
                {
                    Monitor.Pulse(_dequeueLock);
                }
            }
        }

        public bool TryDequeue(out T item)
        {
            lock (_dequeueLock)
            {
                while (_queue.Count == 0 && !_isCompleted)
                {
                    Monitor.Wait(_dequeueLock);
                }

                if (_queue.Count == 0 && _isCompleted)
                {
                    item = default;
                    return false;
                }

                lock (_queue) // 只锁定队列操作
                {
                    item = _queue.Dequeue();
                }

                // 通知可能正在等待的生产者
                lock (_enqueueLock)
                {
                    Monitor.Pulse(_enqueueLock);
                }

                return true;
            }
        }

        public void Complete()
        {
            lock (_enqueueLock)
            {
                lock (_dequeueLock)
                {
                    _isCompleted = true;
                    Monitor.PulseAll(_dequeueLock);
                }
                Monitor.PulseAll(_enqueueLock);
            }
        }

        public int Count
        {
            get
            {
                lock (_queue)
                {
                    return _queue.Count;
                }
            }
        }
    }
}
