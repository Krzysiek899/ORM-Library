using System.Reflection;
using System.ComponentModel.DataAnnotations;

public abstract class Entity{

    public string GetName(){
        return this.GetType().Name;
    }

    public List<FieldInfo> GetFieldsList()
    {
        FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

        return fields.ToList();
    }

     public List<PropertyInfo> GetPrimaryKey()
    {
        List<PropertyInfo> primaryKeyProperties = new List<PropertyInfo>();

        PropertyInfo[] properties = this.GetType().GetProperties();

        foreach (var property in properties)
        {
            if (Attribute.IsDefined(property, typeof(KeyAttribute)))
            {
                primaryKeyProperties.Add(property);  
            }
        }

        return primaryKeyProperties;
    }



    



}