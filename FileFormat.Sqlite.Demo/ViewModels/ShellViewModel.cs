using System.Reactive.Disposables;
using Microsoft.Win32;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace FileFormat.Sqlite.Demo.ViewModels
{
    public sealed class ShellViewModel : ReactiveObject, ISupportsActivation
    {
        public ShellViewModel()
        {
            this.WhenActivated(d =>
            {
                (LoadFileCommand = ReactiveCommand.Create(LoadFile)).DisposeWith(d);
            });
        }

        #region 实现ISupportsActivation

        ViewModelActivator ISupportsActivation.Activator { get; } = new ViewModelActivator();

        #endregion

        private string RootName => "Root";

        public ReactiveCommand LoadFileCommand { get; set; }

        public ObservableCollection<string> NodeNames { get; } = new ObservableCollection<string>();

        private string _selectedNodeName;

        public string SelectedNodeName
        {
            get
            {
                return _selectedNodeName;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref _selectedNodeName, value);
            }
        }

        private FileConnection Connection { get; set; }

        private void LoadFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { CheckFileExists = true, Multiselect = false, Filter = "数据文件|*.mrpd" };
            if(openFileDialog.ShowDialog().Value)
            {
                Connection = new FileConnection(openFileDialog.FileName);
                NodeNames.Clear();
                NodeNames.Add(RootName);
            }
        }
    }
}
