using System.Reactive.Disposables;
using Microsoft.Win32;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System;
using System.Linq;
using DynamicData;
using System.Reactive.Subjects;
using Splat;
using System.IO;
using FileFormat.Sqlite.Demo.Interfaces;

namespace FileFormat.Sqlite.Demo.ViewModels
{
    public sealed class ShellViewModel : ReactiveObject, ISupportsActivation, INodeManager, IDataManager
    {
        public ShellViewModel()
        {
            this.WhenActivated(d =>
            {
                (LoadFileCommand = ReactiveCommand.Create(LoadFile)).DisposeWith(d);
                (UpCommand = ReactiveCommand.Create(Up, this.WhenAnyValue(s => s.SelectedNodeIndex).Select(i => i > 0))).DisposeWith(d);
                (CreateNodeCommand = ReactiveCommand.Create(CreateNode)).DisposeWith(d);
                NodeNameList.Connect().Bind(out _nodeNames).Subscribe().DisposeWith(d);
                ItemViewModelList.Connect().Bind(out _itemViewModels).Subscribe().DisposeWith(d);
                this.WhenAnyValue(s => s.SelectedNodeIndex).Skip(1).CombineLatest(IsUpdating, (int index, bool isUpdating) => new { index, isUpdating }).Where(x => !x.isUpdating).Select(x => x.index).Subscribe(SelectedNodeIndexChanged).DisposeWith(d);
                this.WhenAnyValue(s => s.SelectedItemIndex).Skip(1).Subscribe(SelectedItemIndexChanged).DisposeWith(d);
                FilePath.Subscribe(FilePathChanged).DisposeWith(d);
                (_hasFile = FilePath.Select(f => f != null).ToProperty(this, s => s.HasFile)).DisposeWith(d);
                IsUpdating.OnNext(false);
            });
        }

        #region 实现ISupportsActivation

        ViewModelActivator ISupportsActivation.Activator { get; } = new ViewModelActivator();

        #endregion

        #region 实现INodeManager

        async void INodeManager.DeleteNode(string name)
        {
            await Connection.DeleteNodeAsync(name);
            ItemViewModelList.RemoveAt(ItemViewModelList.Items.Select((item, index) => new { item, index }).First(x => x.item.Name == name && x.item is NodeItemViewModel).index);
        }

        void INodeManager.EnterNode(string name)
        {
            NodeNameList.Add(name);
            UpdateSelectedNodeIndex(i => i + 1);
        }

        #endregion

        #region 实现IDataManager

        async void IDataManager.DeleteData(string name)
        {
            await Connection.DeleteDataAsync(name);
            ItemViewModelList.RemoveAt(ItemViewModelList.Items.Select((item, index) => new { item, index }).First(x => x.item.Name == name && x.item is DataItemViewModel).index);
        }

        #endregion

        private string RootName => "...";

        public ReactiveCommand LoadFileCommand { get; set; }

        public ReactiveCommand CreateFileCommand { get; set; }

        public ReactiveCommand UpCommand { get; set; }

        public ReactiveCommand CreateNodeCommand { get; set; }

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

        private Subject<string> FilePath { get; } = new Subject<string>();

        private ObservableAsPropertyHelper<bool> _hasFile;

        public bool HasFile => _hasFile.Value;

        private NodeConnection Connection { get; set; }

        private void LoadFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { CheckFileExists = true, Multiselect = false, Filter = "数据文件|*.mrpd" };
            if(openFileDialog.ShowDialog().Value)
            {
                FilePath.OnNext(openFileDialog.FileName);
            }
        }

        private void CreateFile()
        {
            
        }

        private async void CreateNode()
        {
            string newName = "NewNode";
            char separator = '_';
            string nodeName = $"{newName}{separator}1";
            ItemViewModelList.Insert(ItemViewModelList.Items.Select((item, index) => new { item, index }).Where(x => x.item is NodeItemViewModel).FirstOrDefault(x =>
            {
                if (string.Compare(x.item.Name, nodeName) < 0)
                    return false;
                if(x.item.Name == nodeName)
                {
                    nodeName = $"{newName}{separator}{int.Parse(nodeName.Split(separator)[1]) + 1}";
                    return false;
                }
                return true;
            })?.index ?? ItemViewModelList.Count, new NodeItemViewModel(nodeName, this));
            await Connection.CreateNodeAsync(nodeName, false);
        }

        private async void FilePathChanged(string filePath)
        {
            UpdateSelectedNodeIndex(i => -1);
            Connection?.Dispose();
            Connection = await new FileConnection(filePath).ConnectRootNodeAsync();
            NodeNameList.Add(RootName);
            UpdateSelectedNodeIndex(i => 0);
        }

        private async void SelectedNodeIndexChanged(int index)
        {
            if (index == -1)
            {
                NodeNameList.Clear();
                ItemViewModelList.Clear();
                return;
            }

            int lastIndex = NodeNameList.Count - 1;
            if (index < lastIndex)
            {
                NodeNameList.Edit(list =>
                {
                    for (int i = lastIndex; i > index; i--)
                        list.RemoveAt(i);
                });
            }

            await Connection.MoveToAsync(NodeNameList.Items.Skip(1));
            ItemViewModelList.Edit(async list =>
            {
                list.Clear();
                foreach (var nodeName in (await Connection.GetChildrenNodeNamesAsync()).OrderBy(n => n))
                    list.Add(new NodeItemViewModel(nodeName, this));
                foreach (var dataName in (await Connection.GetChildrenDataNamesAsync()).OrderBy(n => n))
                    list.Add(new DataItemViewModel(dataName, this));
            });
        }

        private async void SelectedItemIndexChanged(int index)
        {
            if (index == -1)
            {
                Image?.Dispose();
                Image = null;
                return;
            }
            var selectedItem = ItemViewModelList.Items.ElementAt(index);
            if (selectedItem is NodeItemViewModel)
            {
                Image?.Dispose();
                Image = null;
            }
            else
            {
                using (var stream = new MemoryStream(await Connection.ReadDataAsync(selectedItem.Name)))
                {
                    Image?.Dispose();
                    Image = await BitmapLoader.Current.Load(stream, null, null);
                }
            }
        }

        private void UpdateSelectedNodeIndex(Func<int, int> updateAction)
        {
            IsUpdating.OnNext(true);
            SelectedNodeIndex = updateAction.Invoke(SelectedNodeIndex);
            IsUpdating.OnNext(false);
        }

        private void Up()
        {
            UpdateSelectedNodeIndex(i => i - 1);
        }
    }
}
