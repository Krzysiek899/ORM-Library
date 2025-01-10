using System.Linq.Expressions;

public class EntityConfiguration
{
    public Type EntityType { get; }
    public List<object> NavigationProperties { get; }
    public object ForeignKey { get; private set; }

    public EntityConfiguration(Type entityType)
    {
        EntityType = entityType;
        NavigationProperties = new List<object>();
    }

    public void AddCollectionNavigation(object navigation)
    {
        NavigationProperties.Add(navigation);
    }

    public void AddReferenceNavigation(object navigation)
    {
        NavigationProperties.Add(navigation);
    }

    public void AddManyToManyNavigation(object navigation)
    {
        NavigationProperties.Add(navigation);
    }

    public void SetForeignKey(object foreignKey)
    {
        ForeignKey = foreignKey;
    }
}

// Reference for "One-to-One" or "One-to-Many" relationships
public class ReferenceBuilder<T, TRelated>
{
    private readonly Expression<Func<T, TRelated>> _navigationExpression;

    public ReferenceBuilder(Expression<Func<T, TRelated>> navigationExpression)
    {
        _navigationExpression = navigationExpression;
    }
}

// Reference for "Many-to-Many" relationships
public class ManyToManyBuilder<T, TRelated>
{
    private readonly Expression<Func<T, IEnumerable<TRelated>>> _navigationExpression;

    public ManyToManyBuilder(Expression<Func<T, IEnumerable<TRelated>>> navigationExpression)
    {
        _navigationExpression = navigationExpression;
    }
}

// Reference for "Collection Navigation" (HasMany)
public class ReferenceCollectionBuilder<T, TRelated>
{
    private readonly Expression<Func<T, IEnumerable<TRelated>>> _navigationExpression;

    public ReferenceCollectionBuilder(Expression<Func<T, IEnumerable<TRelated>>> navigationExpression)
    {
        _navigationExpression = navigationExpression;
    }
}