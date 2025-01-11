using System.Collections;

namespace ORMLibrary.Iterator
{
    public abstract class IterableCollection
    {
        public abstract IEnumerator GetEnumerator();
    }
}