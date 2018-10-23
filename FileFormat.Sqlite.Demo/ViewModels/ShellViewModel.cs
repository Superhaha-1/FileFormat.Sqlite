using System.Reactive.Disposables;
using Microsoft.Win32;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System;
using System.Linq;
using DynamicData;
using System.Threading.Tasks;
using System.Reactive.Subjects;
using Splat;
using System.IO;

namespace FileFormat.Sqlite.Demo.ViewModels
{
    public sealed class ShellViewModel : ReactiveObject, ISupportsActivation
    {
        public ShellViewModel()
        {
            (this).WhenActivated(d =>
            {
                (LoadFileCommand = ReactiveCommand.Create(LoadFile)).DisposeWith(d);
                (UpCommand = ReactiveCommand.Create(Up, this.WhenAnyValue(s => s.SelectedNodeIndex).Select(i => i > 0))).DisposeWith(d);
                NodeNameList.Connect().Bind(out _nodeNames).Subscribe().DisposeWith(d);
                ItemViewModelList.Connect().Bind(out _itemViewModels).Subscribe().DisposeWith(d);
                this.WhenAnyValue(s => s.SelectedNodeIndex).Skip(1).CombineLatest(IsUpdating, (int index, bool isUpdating) => new { index, isUpdating }).Where(x => !x.isUpdating).Select(x => x.index).Select(SelectedNodeIndexChanged).Subscribe().DisposeWith(d);
                this.WhenAnyValue(s => s.SelectedItemIndex).Skip(1).Subscribe(SelectedItemIndexChanged).DisposeWith(d);
            });
        }

        #region 实现ISupportsActivation

        ViewModelActivator ISupportsActivation.Activator { get; } = new ViewModelActivator();

        #endregion

        private string RootName => "...";

        public ReactiveCommand LoadFileCommand { get; set; }

        public ReactiveCommand UpCommand { get; set; }

        private SourceList<string> NodeNameList { get; } = new SourceList<string>();

        private ReadOnlyObservableCollection<string> _nodeNames;

        public ReadOnlyObservableCollection<string> NodeNames => _nodeNames;

        private SourceList<ItemViewModelBase> ItemViewModelList { get; } = new SourceList<ItemViewModelBase>();

        private ReadOnlyObservableCollection<ItemViewModelBase> _itemViewModels;

        public ReadOnlyObservableCollection<ItemViewModelBase> ItemViewModels => _itemViewModels;

        private int _selectedNodeIndex = -1;

        public int SelectedNodeIndex
        {
            get
            {
                return _selectedNodeIndex;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref _selectedNodeIndex, value);
            }
        }

        private Subject<bool> IsUpdating { get; } = new Subject<bool>();

        private int _selectedItemIndex = -1;

        public int SelectedItemIndex
        {
            get
            {
                return _selectedItemIndex;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref _selectedItemIndex, value);
            }
        }

        private IBitmap _image;

        public IBitmap Image
        {
            get
            {
                return _image;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref _image, value);
            }
        }

        private FileConnection Connection { get; set; }

        private void LoadFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { CheckFileExists = true, Multiselect = false, Filter = "数据文件|*.mrpd" };
            if(openFileDialog.ShowDialog().Value)
            {

                SelectedNodeIndex = -1;
                Connection = new FileConnection(openFileDialog.FileName);
                NodeNameList.Edit(list =>
                {
                    list.Add(RootName);
                });
                SelectedNodeIndex = 0;
                IsUpdating.OnNext(false);
            }
        }

        private async Task SelectedNodeIndexChanged(int index)
        {
            if (index == -1)
            {
                NodeNameList.Edit(list =>
                {
                    list.Clear();
                });
                ItemViewModelList.Edit(list =>
                {
                    list.Clear();
                });
                return;
            }
            using (var nodeConnection = await Connection.ConnectNodeAsync(NodeNameList.Items.Skip(1).Take(index)))
            {
                ItemViewModelList.Edit(async list =>
                {
                    list.Clear();
                    foreach (var nodeName in await nodeConnection.GetChildrenNodeNamesAsync())
                        list.Add(new NodeItemViewModel(nodeName));
                    foreach (var dataName in await nodeConnection.GetChildrenDataNamesAsync())
                        list.Add(new DataItemViewModel(dataName));
                });
            }
            SelectedItemIndex = -1;

            int lastIndex = NodeNameList.Count - 1;
            if (index < lastIndex)
            {
                NodeNameList.Edit(list =>
                {
                    for (int i = lastIndex; i > index; i--)
                        list.RemoveAt(i);
                });
            }
        }

        private async void SelectedItemIndexChanged(int index)
        {
            if (index == -1)
                return;
            var selectedItem = ItemViewModelList.Items.ElementAt(index);
            if (selectedItem is NodeItemViewModel)
            {
                NodeNameList.Add(selectedItem.Name);
                SelectedNodeIndex++;
                Image?.Dispose();
                Image = null;
            }
            else
            {
                using (var nodeConnection = await Connection.ConnectNodeAsync(NodeNameList.Items.Skip(1)))
                {
                    using (var stream = new MemoryStream(await nodeConnection.ReadDataAsync(selectedItem.Name)))
                    {
                        Image?.Dispose();
                        Image = await BitmapLoader.Current.Load(stream, null, null);
                    }
                }
            }
        }

        private void Up()
        {
            IsUpdating.OnNext(true);
            SelectedNodeIndex--;
            IsUpdating.OnNext(false);
        }
    }
}
