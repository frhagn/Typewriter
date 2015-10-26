using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Typewriter.Generation.Controllers
{
    /// <summary>
    /// Same as Queue except Dequeue function blocks until there is an object to return.
    /// Note: This class does not need to be synchronized
    /// </summary>
    public class BlockingQueue<T> : ICollection
    {


        private Queue<T> _base;

        private bool _open;
        /// <summary>
        /// Create new BlockingQueue.
        /// </summary>
        /// <param name="col">The System.Collections.ICollection to copy elements from</param>
        //public BlockingQueue(ICollection<T> col)
        //{
        //    _base = new Queue<T>(col);
        //    _open = true;
        //}



        /// <summary>
        /// Create new BlockingQueue.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the queue can contain</param>

        public BlockingQueue(int capacity)
        {
            _base = new Queue<T>(capacity);
            _open = true;
        }



        /// <summary>
        /// Create new BlockingQueue.
        /// </summary>

        public BlockingQueue()
        {
            _base = new Queue<T>();
            _open = true;
        }



        /// <summary>
        /// BlockingQueue Destructor (Close queue, resume any waiting thread).
        /// </summary>

        ~BlockingQueue()
        {
            Close();
        }



        /// <summary>
        /// Remove all objects from the Queue.
        /// </summary>


        public void Clear()
        {

            lock (_SyncRoot)
            {
                _base.Clear();

            }
        }



        /// <summary>
        /// Remove all objects from the Queue, resume all dequeue threads.
        /// </summary>


        public void Close()
        {

            lock (_SyncRoot)
            {
                if (_open)
                {
                    _open = false;

                    _base.Clear();

                    // resume any waiting threads
                    Monitor.PulseAll(_SyncRoot);
                }

            }
        }



        /// <summary>
        /// Removes and returns the object at the beginning of the Queue.
        /// </summary>
        /// <returns>Object in queue.</returns>

        public T Dequeue()
        {

            return Dequeue(Timeout.Infinite);
        }



        /// <summary>
        /// Removes and returns the object at the beginning of the Queue.
        /// </summary>
        /// <param name="timeout">time to wait before returning</param>
        /// <returns>Object in queue.</returns>

        public T Dequeue(TimeSpan timeout)
        {

            return Dequeue(timeout.Milliseconds);
        }



        /// <summary>
        /// Removes and returns the object at the beginning of the Queue.
        /// </summary>
        /// <param name="timeout">time to wait before returning (in milliseconds)</param>
        /// <returns>Object in queue.</returns>

        public T Dequeue(int timeout)
        {

            lock (_SyncRoot)
            {

                while (_open && (_base.Count == 0))
                {


                    if (!Monitor.Wait(_SyncRoot, timeout))
                    {
                        throw new InvalidOperationException("Timeout");

                    }
                }


                if (_open)
                {
                    return _base.Dequeue();

                }

                throw new InvalidOperationException("Queue Closed");

            }
        }


        public T Peek()
        {


            lock (_SyncRoot)
            {

                while (_open && (_base.Count == 0))
                {

                    if (!Monitor.Wait(_SyncRoot, Timeout.Infinite))
                    {
                        throw new InvalidOperationException("Timeout");

                    }
                }


                if (_open)
                {
                    return _base.Peek();

                }
                throw new InvalidOperationException("Queue Closed");

            }
        }



        /// <summary>
        /// Adds an object to the end of the Queue.
        /// </summary>
        /// <param name="obj">Object to put in queue</param>


        public void Enqueue(T obj)
        {

            lock (_SyncRoot)
            {
                _base.Enqueue(obj);

                Monitor.Pulse(_SyncRoot);

            }
        }


        public bool EnqueueIfNotContains(T obj)
        {

            bool added = false;


            lock (_SyncRoot)
            {
                if (!_base.Contains(obj))
                {
                    _base.Enqueue(obj);
                    added = true;
                }


                Monitor.Pulse(_SyncRoot);

            }

            return added;

        }

        public bool EnqueueIfNotContains(T obj, IEqualityComparer<T> comparer)
        {

            bool added = false;


            lock (_SyncRoot)
            {
                if (!_base.Contains(obj, comparer))
                {
                    _base.Enqueue(obj);
                    added = true;
                    
                }


                Monitor.Pulse(_SyncRoot);

            }

            return added;

        }



        /// <summary>
        /// Open Queue.
        /// </summary>


        public void Open()
        {

            lock (_SyncRoot)
            {
                _open = true;

            }
        }



        /// <summary>
        /// Gets flag indicating if queue has been closed.
        /// </summary>

        public bool Closed
        {
            get { return !_open; }
        }



        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();

        }

        public int Count
        {
            get { return _base.Count; }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }


        private object _SyncRoot = new object();
        public object SyncRoot
        {
            get { return _SyncRoot; }
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return _base.GetEnumerator();
        }
    }
}
