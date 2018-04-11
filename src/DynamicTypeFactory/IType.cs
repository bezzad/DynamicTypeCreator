using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DynamicTypeFactory
{
    public interface IType
    {
        IType Map<T>(T source);
    }


    public class Pype : IType
    {
        protected Dictionary<string, PropertyInfo> Properties = new Dictionary<string, PropertyInfo>();

        public IType Map<T>(T source)
        {
            var srcFullName = typeof(T).FullName;
            foreach (var prop in GetType().GetProperties())
            {
                var key = $"{srcFullName}_{prop.Name}";
                if (!Properties.TryGetValue(key, out var dtProp))
                    Properties[key] = dtProp = typeof(T).GetProperties().FirstOrDefault(p => p.JsPropertyName() == prop.Name);

                prop.SetValue(this, dtProp?.GetValue(source) ?? prop.PropertyType.GetDefault());
            }

            return this;
        }
    }

}