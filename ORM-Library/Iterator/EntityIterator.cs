using System;
using System.Collections;
using System.Data;

namespace ORMLibrary.Iterator
{
    public class EntityIterator : Iterator
    {
        private EntityCollection _collection;
        private int _position = -1;
        private bool _reverse = false;

        public EntityIterator(EntityCollection collection, bool reverse)
        {
            _collection = collection;
            _reverse = reverse;

            if (_reverse)
            {
                _position = _collection.GetItems().Count;
            }
        }

        public override object Current()
        {
            if (_position < 0 || _position >= _collection.GetItems().Count)
            {
                throw new InvalidOperationException("Iterator is out of bounds.");
            }

            return _collection.GetItems()[_position];
        }

        public override object Key()
        {
            return _position;
        }

        public override bool MoveNext()
        {
            int updatedPosition = _position + (_reverse ? -1 : 1);

            if (updatedPosition >= 0 && updatedPosition < _collection.GetItems().Count)
            {
                _position = updatedPosition;
                return true;
            }

            return false;
        }

        public override bool HasNext()
        {
            int updatedPosition = _position + (_reverse ? -1 : 1);
            if ((updatedPosition >= 0 && updatedPosition < _collection.GetItems().Count  && !_reverse))
            {
                return true;
            }

            return false;
        }

        public override void Reset()
        {
            _position = _reverse ? _collection.GetItems().Count - 1 : 0;
        }
    }
}

