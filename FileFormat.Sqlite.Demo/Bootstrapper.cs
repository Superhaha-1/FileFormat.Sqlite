using System.Windows;
using Prism.Mef;
using System.ComponentModel.Composition.Hosting;
using Splat;
using FileFormat.Sqlite.Demo.Views;
using System;
using System.Reflection;
using System.Reactive.Linq;
using ReactiveUI;

namespace FileFormat.Sqlite.Demo
{
    /// <summary>
    /// 启动器
    /// </summary>
    public class Bootstrapper : MefBootstrapper
    {
        /// <summary>
        /// 构造Shell
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject CreateShell()
        {
            return Container.GetExportedValue<Shell>();
        }

        /// <summary>
        /// 初始化Shell
        /// </summary>
        protected override void InitializeShell()
        {
            (Shell as Window).Show();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Locator.CurrentMutable.InitializeSplat();
            Locator.CurrentMutable.InitializeReactiveUI();
            Locator.CurrentMutable.RegisterLazySingleton(() => new MefViewLocator(Container), typeof(IViewLocator));
        }

        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();
            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(GetType().Assembly));
            //    string m_name = GetType().Assembly.GetName().Name;
            //    foreach (var n in XElement.Load("CatalogInfo.xml").Elements("Application").First(m => m.Attribute("Name").Value == m_name).Elements("Assembly"))
            //    {
            //        var assemblyCatalog = new AssemblyCatalog(string.Format(@"DirectoryModules\{0}.dll", n.Attribute("Name").Value));
            //        AggregateCatalog.Catalogs.Add(assemblyCatalog);
            //        SerializationHelper.Add(assemblyCatalog.Assembly);
            //    }
        }
    }
}
