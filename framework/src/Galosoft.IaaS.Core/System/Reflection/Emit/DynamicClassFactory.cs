using System;
using System.Collections.Generic;
using System.Reflection;

namespace System.Reflection.Emit
{
    public class DynamicClassFactory
    {
        private ModuleBuilder _dynamicModule;
        public static DynamicClassFactory Instance = new DynamicClassFactory();
        private Dictionary<string, Type> _classes;
        private DynamicClassFactory()
        {
            var dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicAssembly"), AssemblyBuilderAccess.RunAndCollect);
            _dynamicModule = dynamicAssembly.DefineDynamicModule("DynamicModule");
            _classes = new Dictionary<string, Type>();
        }

        public Type GetOrCreateClass(string name, params KeyValuePair<string, Type>[] properties)
        {
            if (_classes.TryGetValue(name, out var type))
                return type;

            var newType = CreateClass(name, properties);
            _classes.Add(name, newType);
            return newType;
        }

        private Type CreateClass(string name, params KeyValuePair<string, Type>[] properties)
        {
            var dynamicType = _dynamicModule.DefineType(name, TypeAttributes.Class);
            dynamicType.SetParent(typeof(object));

            foreach (var property in properties)
            {
                var propertyName = property.Key;
                var propertyType = property.Value;
                var prop = dynamicType.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
                var field = dynamicType.DefineField($"_{propertyName.ToLower()}", propertyType, FieldAttributes.Private | FieldAttributes.HasDefault);

                var mbGet = dynamicType.DefineMethod($"get_{propertyName}", MethodAttributes.Public | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
                var genGet = mbGet.GetILGenerator();
                genGet.Emit(OpCodes.Ldarg_0);
                genGet.Emit(OpCodes.Ldfld, field);
                genGet.Emit(OpCodes.Ret);
                prop.SetGetMethod(mbGet);

                var mbSet = dynamicType.DefineMethod($"set_{propertyName}", MethodAttributes.Public | MethodAttributes.HideBySig, null, new[] { propertyType });
                var genSet = mbSet.GetILGenerator();
                genSet.Emit(OpCodes.Ldarg_0);
                genSet.Emit(OpCodes.Ldarg_1);
                genSet.Emit(OpCodes.Stfld, field);
                genSet.Emit(OpCodes.Ret);
                prop.SetSetMethod(mbSet);
            }

            return dynamicType.CreateType();
        }
    }
}
