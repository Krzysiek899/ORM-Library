public class EntityIterator
{
    private EntityCollection Collection;
    private Iterator IterationState; // stan iteracji (typ to raczej nie bedzie Iterator)

    public EntityIterator()
    {
        Collection = new EntityCollection();
    }
}