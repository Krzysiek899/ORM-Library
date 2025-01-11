using System;
using System.Collections.Generic;
using System.Linq;
using ORMLibrary.Context;
using ORMLibrary.Mapping;

namespace ORMLibrary.Validation
{
    public class EntityValidator
    {
        private readonly DatabaseContext _context;

        public EntityValidator(DatabaseContext context)
        {
            _context = context;
        }

        // public bool Validate()
        // {
        //     // var tables = _context.GetTables();
        //     var errors = new List<string>();

        //     foreach (var table in tables)
        //     {
        //         foreach (var foreignKey in table.ForeignKeys)
        //         {
        //             var referenceTable = tables.FirstOrDefault(t => t.TableName == foreignKey.ReferenceTable);
        //             if (referenceTable == null)
        //             {
        //                 errors.Add($"Foreign key '{foreignKey.Name}' in table '{table.TableName}' references non-existent table '{foreignKey.ReferenceTable}'.");
        //             }
        //         }
        //     }

        //     if (errors.Any())
        //     {
        //         foreach (var error in errors)
        //         {
        //             Console.WriteLine(error);
        //         }
        //         return false;
        //     }

        //     return true;
        // }
    }
}