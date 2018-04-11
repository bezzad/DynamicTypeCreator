using System;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicTypeFactory
{
    internal class DynamicAssemblyBuilder
    {
        internal static string AssemblyName { get; } = $"DynamicAssembly_{Guid.NewGuid():N}";

        public static DynamicAssembly GlobalDynamicAssembly { get; } = new DynamicAssembly();

        internal sealed class DynamicAssembly
        {
            private readonly AssemblyBuilder _assemblyBuilder;
            public ModuleBuilder ModuleBuilder { get; }

            public DynamicAssembly()
            { 
                var assemblyName = new AssemblyName(AssemblyName);
                _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
                ModuleBuilder = _assemblyBuilder.DefineDynamicModule(assemblyName.Name);
            }


            public void Save(string asmName = null)
            {
                _assemblyBuilder.Save(asmName ?? AssemblyName);
            }
        }
    }
}