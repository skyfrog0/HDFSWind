using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HDFSwindow.HDFSCore
{
    public class AtomicInt
    {
        private volatile int _count = 0;

        public int Count
        {
            get { return _count; }
        }

        public bool IsEmpty
        { 
            get { return 0 == _count; } 
        }

        public AtomicInt()
        {
        }

        public AtomicInt(int initial)
        {
            _count = initial;
        }

        public void Increment()
        {
            Interlocked.Increment(ref _count);
        }

        public void Decrement()
        {
            Interlocked.Decrement(ref _count);
        }

    }
}
