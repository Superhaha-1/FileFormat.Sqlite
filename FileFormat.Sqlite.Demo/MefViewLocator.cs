using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace FileFormat.Sqlite.Demo
{
    public sealed class MefViewLocator : IViewLocator
    {
        public MefViewLocator(CompositionContainer container)
        {
            Container = container;
            GetExportMethodInfo = typeof(CompositionContainer).GetMethod(nameof(CompositionContainer.GetExportedValue), new Type[] { typeof(string) });
        }

        private MethodInfo GetExportMethodInfo { get; }

        private CompositionContainer Container { get; }

        private Type IViewForType { get; } = typeof(IViewFor<>);

        private Dictionary<Type, MethodInfo> GetExportMethodInfoDictionary { get; } = new Dictionary<Type, MethodInfo>();

        public IViewFor ResolveView<T>(T viewModel, string contract = null) where T : class
        {
            var type = viewModel.GetType();
            if (!GetExportMethodInfoDictionary.TryGetValue(type, out var methodInfo))
            {
                GetExportMethodInfoDictionary.Add(type, methodInfo = GetExportMethodInfo.MakeGenericMethod(IViewForType.MakeGenericType(type)));
            }
            return methodInfo.Invoke(Container, new object[] { contract }) as IViewFor;
        }
    }
}
