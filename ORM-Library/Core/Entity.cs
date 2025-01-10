using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

public abstract class Entity
{
    // Zwraca nazwę klasy (Entity)
    public string GetName()
    {
        return this.GetType().Name;
    }

    // Zwraca listę pól (np. w celu generowania metadanych)
    public List<FieldInfo> GetFieldsList()
    {
        FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        return new List<FieldInfo>(fields);
    }

    // Zwraca właściwości oznaczone jako klucze główne
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

    // Zwraca właściwości oznaczone jako klucze obce (ForeignKey)
    public List<PropertyInfo> GetForeignKeys()
    {
        List<PropertyInfo> foreignKeyProperties = new List<PropertyInfo>();
        PropertyInfo[] properties = this.GetType().GetProperties();

        foreach (var property in properties)
        {
            var foreignKeyAttribute = property.GetCustomAttribute<ForeignKeyAttribute>();
            if (foreignKeyAttribute != null)
            {
                foreignKeyProperties.Add(property);
            }
        }

        return foreignKeyProperties;
    }

    // Zwraca relacje w postaci słownika {nazwa właściwości -> typ powiązanej encji}
    public Dictionary<string, Type> GetRelationships()
    {
        var relationships = new Dictionary<string, Type>();

        var foreignKeys = GetForeignKeys();

        foreach (var foreignKey in foreignKeys)
        {
            var foreignKeyAttribute = foreignKey.GetCustomAttribute<ForeignKeyAttribute>();
            if (foreignKeyAttribute != null)
            {
                // Relacja wiele do 1 (klucz obcy -> encja)
                relationships.Add(foreignKey.Name, foreignKey.PropertyType);
            }
        }

        // Sprawdzamy kolekcje, relacje 1:N i N:M
        var properties = this.GetType().GetProperties();
        foreach (var property in properties)
        {
            // Relacja 1:N (np. jeden użytkownik ma wielu członków zespołu)
            if (typeof(IEnumerable<>).IsAssignableFrom(property.PropertyType.GetGenericTypeDefinition()))
            {
                var collectionType = property.PropertyType.GetGenericArguments()[0];
                relationships.Add(property.Name, collectionType);
            }

            // Relacje 1:1
            else if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
            {
                relationships.Add(property.Name, property.PropertyType);
            }
        }

        return relationships;
    }
}
