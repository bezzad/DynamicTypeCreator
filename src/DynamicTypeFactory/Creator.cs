using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicTypeFactory
{
    public static class Creator
    {
        private static Dictionary<TypePair, Type> MapperHistory { get; } = new Dictionary<TypePair, Type>();

        public static void Bind<TSource, TTarget>(Type model)
        {
            var typePair = TypePair.Create<TSource, TTarget>();
            MapperHistory[typePair] = model;
        }
        public static IType GetMapperType<TSource, TTarget>()
        {
            var typePair = TypePair.Create<TSource, TTarget>();
            if (MapperHistory.TryGetValue(typePair, out var model) == false)
                MapperHistory[typePair] = model = Build<TSource, TTarget>();

            return Activator.CreateInstance(model) as IType;
        }
        public static IType Map<TSource, TTarget>(this TSource source)
        {
            var model = GetMapperType<TSource, TTarget>();
            var result = model.Map(source);
            return result;
        }
        public static List<IType> Map<TSource, TTarget>(this List<TSource> sources)
        {
            return sources.Select(Map<TSource, TTarget>).ToList();
        }

        public static Type Build<TSource, TTarget>()
        {
            // Generate a persist-able single-module assembly.
            var mb = DynamicAssemblyBuilder.GlobalDynamicAssembly.ModuleBuilder;
            var tb = mb.DefineType($"DynamicType_{Guid.NewGuid():N}", TypeAttributes.Public, typeof(Pype));

            //define a parameterless constructor to initialize the field.
            //tb.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, Type.EmptyTypes);

            var properties = 
                from s in typeof(TSource).GetProperties() join t in typeof(TTarget).GetProperties() on s.Name equals t.Name
                where s.PropertyType == t.PropertyType select s;

            // read any properties except which has been `IgnorePropertyAttribute`
            foreach (var prop in properties)
            {
                // fetch name from `DisplayAttribute` if exist
                var propName = prop.JsPropertyName();

                var field = tb.DefineField("_" + propName, prop.PropertyType, FieldAttributes.Private); // Field:  _propName;

                // The last argument of DefineProperty is null, because the property has no parameters.
                var property = tb.DefineProperty(propName, PropertyAttributes.HasDefault, prop.PropertyType, null); // Property:  propName { get; set; }
                
                // The property set and property get methods require a special
                // set of attributes.
                var getSetAttr = MethodAttributes.Public | MethodAttributes.HideBySig;

                // Define the "get" accessors method for Property.
                var getter = tb.DefineMethod($"get_{property.Name}", getSetAttr, prop.PropertyType, Type.EmptyTypes);
                // Define the "set" accessors method for CustomerName.
                var setter = tb.DefineMethod($"set_{property.Name}", getSetAttr, typeof(void), new[] { property.PropertyType });

                var propGetIl = getter.GetILGenerator();
                propGetIl.Emit(OpCodes.Ldarg_0);
                propGetIl.Emit(OpCodes.Ldfld, field);
                propGetIl.Emit(OpCodes.Ret);

                var propSetIl = setter.GetILGenerator();
                propSetIl.Emit(OpCodes.Ldarg_0);
                propSetIl.Emit(OpCodes.Ldarg_1);
                propSetIl.Emit(OpCodes.Stfld, field);
                propSetIl.Emit(OpCodes.Ret);

                // Last, we must map the two methods created above to our PropertyBuilder to 
                // their corresponding behaviors, "get" and "set" respectively. 
                property.SetGetMethod(getter);
                property.SetSetMethod(setter);
            }
            
            return tb.CreateType();
        }

        public static string JsPropertyName(this PropertyInfo prop)
        {
            return prop.Name.ToTitle().Replace(" ", "_").ToLower();
        }
    }
}