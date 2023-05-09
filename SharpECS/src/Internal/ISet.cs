using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpECS.Internal
{
    internal interface ISet<T>
    {
        public void Add(T item);

        void Remove(T item);

        bool Contains(T item);
    }
}
