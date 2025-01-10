using System.Collections;
using System.Data;
using System.Collections.Generic;

public class EntityCollection : IterableCollection
{
    private List<DataRow> _collection = new List<DataRow>();
    private bool _direction = false;

    public void ReverseDirection()
    {
        _direction = !_direction;
    }

    public List<DataRow> GetItems()
    {
        return _collection;
    }

    public void AddItem(DataRow item)
    {
        _collection.Add(item);
    }

    public override IEnumerator GetEnumerator()
    {
        return new EntityIterator(this, _direction);
    }
}
