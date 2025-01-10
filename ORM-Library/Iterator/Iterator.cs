using System.Collections;

public abstract class Iterator : IEnumerator
{
    object IEnumerator.Current => Current();

    public abstract object Key();
    public abstract object Current();
    public abstract bool MoveNext();
    public abstract void Reset();

}