using System.Linq.Expressions;


public class EntityTypeBuilder<T>
{
    public EntityConfiguration Config { get; private set; }

    public EntityTypeBuilder()
    {
        Config = new EntityConfiguration(typeof(T));
    }

    // Relacja "HasMany" (jeden do wielu)
    public ReferenceCollectionBuilder<T, TRelated> HasMany<TRelated>(Expression<Func<T, IEnumerable<TRelated>>> navigationExpression)
    {
        var builder = new ReferenceCollectionBuilder<T, TRelated>(navigationExpression);
        Config.AddCollectionNavigation(builder);
        return builder;
    }

    // Relacja "WithOne" (jeden do jednego)
    public ReferenceBuilder<T, TRelated> WithOne<TRelated>(Expression<Func<T, TRelated>> navigationExpression)
    {
        var builder = new ReferenceBuilder<T, TRelated>(navigationExpression);
        Config.AddReferenceNavigation(builder);
        return builder;
    }

    // Relacja "Many-to-Many"
    public ManyToManyBuilder<T, TRelated> HasManyToMany<TRelated>(Expression<Func<T, IEnumerable<TRelated>>> navigationExpression)
    {
        var builder = new ManyToManyBuilder<T, TRelated>(navigationExpression);
        Config.AddManyToManyNavigation(builder);
        return builder;
    }

    // Definiowanie klucza obcego
    public EntityTypeBuilder<T> HasForeignKey<TKey>(Expression<Func<T, TKey>> foreignKeyExpression)
    {
        Config.SetForeignKey(foreignKeyExpression);
        return this;
    }
}