using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace FileFormat.Sqlite.Demo.ViewModels
{
    public abstract class ItemViewModelBase : ReactiveObject
    {
        protected ItemViewModelBase(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
