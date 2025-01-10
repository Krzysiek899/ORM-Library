public class EntityIterator : Iterator
{
    private EntityCollection _collection;

    private int _possition = -1;

    private bool _reverse = false;

    public EntityIterator(EntityCollection collection, bool reverse)
    {   
        this._collection = collection;
        this._reverse = reverse;

        if (reverse)
        {
            this._possition = collection.getItems().Count;
        }
    }

    public override object Current()
    {
        return this._collection.getItems()[_possition];
    }

    public override object Key()
    {
        return this._possition;
    } 

    public override bool MoveNext()
    {
        int updatedPossition = this._possition + (this._reverse ? -1 : 1);

        if (updatedPossition >= 0 && updatedPossition < this._collection.getItems().Count)
        {
            this._possition = updatedPossition;
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void Reset()
    {
        this._possition = this._reverse ? this._collection.getItems().Count - 1 : 0;
    }



}