using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Loader;

namespace System.Reflection
{
    public static class TypeUtils
    {
        public static Type GetEntityTypeSafelyByName(string name)
        {
            return GetTypes(t => t.HasCustomeAttribute<TableAttribute>(false)&&t.Name.Equals(name))
                .FirstOrDefault();
        }

        public static IEnumerable<Type> GetEntityTypes()
        {
            return GetTypes(t => t.HasCustomeAttribute<TableAttribute>(false));
        }


        public static IEnumerable<Type> GetTypes(Func<Type, bool> func = null)
        {
            var types = AssemblyLoadContext.Default.Assemblies
                            .SelectMany(assm => assm.GetTypes());
            if (func != null)
                types = types.Where(func);

            return types;
        }
    }
}
