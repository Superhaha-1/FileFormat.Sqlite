using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace FileFormat.Sqlite.Demo.ViewModels
{
    public abstract class ItemViewModelBase : ReactiveObject, ISupportsActivation
    {
        public static ItemViewModelBase Empty { get; } = null;

        protected ItemViewModelBase(string name)
        {
            Name = name;
        }

        #region 实现ISupportsActivation

        ViewModelActivator ISupportsActivation.Activator { get; } = new ViewModelActivator();

        #endregion

        public string Name { get; }
    }
}
