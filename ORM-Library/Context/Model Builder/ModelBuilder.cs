public class ModelBuilder
{
    private readonly List<EntityConfiguration> _entityConfigurations = new List<EntityConfiguration>();

    // Rejestracja konfiguracji encji
    public EntityTypeBuilder<T> Entity<T>()
    {
        var builder = new EntityTypeBuilder<T>();
        _entityConfigurations.Add(builder.Config);
        return builder;
    }

    // Metoda do pobrania konfiguracji
    public List<EntityConfiguration> GetConfigurations() => _entityConfigurations;
}




