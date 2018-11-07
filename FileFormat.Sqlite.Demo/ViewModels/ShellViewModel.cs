using System.Reactive.Disposables;
using Microsoft.Win32;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System;
using System.Linq;
using DynamicData;
using System.Reactive.Subjects;
using FileFormat.Sqlite.Demo.Interfaces;
using System.Windows.Input;
using DynamicData.Binding;

namespace FileFormat.Sqlite.Demo.ViewModels
{
    public sealed class ShellViewModel : ReactiveObject, ISupportsActivation, INodeManager, IDataManager
    {
        public ShellViewModel()
        {
            this.WhenActivated(d =>
            {
                (IsUpdating = new Subject<bool>()).DisposeWith(d);
                (FilePath = new Subject<string>()).DisposeWith(d);
                (LoadFileCommand = ReactiveCommand.Create(LoadFile)).DisposeWith(d);
                (UpCommand = ReactiveCommand.Create(Up, this.WhenAnyValue(s => s.SelectedNodeIndex).Select(i => i > 0))).DisposeWith(d);
                (CreateNodeCommand = ReactiveCommand.Create(CreateNode)).DisposeWith(d);
                (_deleteNodeCommand = ReactiveCommand.Create<string>(DeleteNode)).DisposeWith(d);
                (_enterNodeCommand = ReactiveCommand.Create<string>(EnterNode)).DisposeWith(d);
                (_startRenameNodeCommand = ReactiveCommand.Create<string>(StartRenameNode)).DisposeWith(d);
                (_endRenameNodeCommand = ReactiveCommand.Create<(string, string)>(EndRenameNode)).DisposeWith(d);
                (_deleteDataCommand = ReactiveCommand.Create<string>(DeleteData)).DisposeWith(d);
                NodeNameList.Connect().Bind(out _nodeNames).Subscribe().DisposeWith(d);
                NodeItemViewModelCache.Connect().Sort(SortExpressionComparer<ItemViewModelBase>.Ascending(i => i.Name)).Concat(DataItemViewModelCache.Connect().Sort(SortExpressionComparer<ItemViewModelBase>.Ascending(i => i.Name))).Bind(out _itemViewModels).Subscribe().DisposeWith(d);
                this.WhenAnyValue(s => s.SelectedNodeIndex).Skip(1).CombineLatest(IsUpdating, (index, isUpdating) => (index: index, isUpdating: isUpdating)).Where(x => !x.isUpdating).Select(x => x.index).Subscribe(SelectedNodeIndexChanged).DisposeWith(d);
                FilePath.Subscribe(FilePathChanged).DisposeWith(d);
                (_hasFile = FilePath.Select(f => f != null).ToProperty(this, s => s.HasFile)).DisposeWith(d);
                IsUpdating.OnNext(false);
            });
        }

        #region 实现ISupportsActivation

        ViewModelActivator ISupportsActivation.Activator { get; } = new ViewModelActivator();

        #endregion

        #region 实现INodeManager

        #region DeleteNodeCommand

        private ReactiveCommand _deleteNodeCommand;

        ICommand INodeManager.DeleteNodeCommand => _deleteNodeCommand;

        private async void DeleteNode(string name)
        {
            await Connection.DeleteNodeAsync(name);
            NodeItemViewModelCache.RemoveKey(name);
        }

        #endregion

        #region EnterNodeCommand

        private ReactiveCommand _enterNodeCommand;

        ICommand INodeManager.EnterNodeCommand => _enterNodeCommand;

        private void EnterNode(string name)
        {
            NodeNameList.Add(name);
            UpdateSelectedNodeIndex(i => i + 1);
        }

        #endregion

        private void UpdateNodeCache(Action<ISourceUpdater<ItemViewModelBase, string>> updateAction)
        {
            int selectedItemIndex = SelectedItemIndex;
            NodeItemViewModelCache.Edit(updateAction);
            SelectedItemIndex = selectedItemIndex;
        }

        #region StartRenameNodeCommand

        private ReactiveCommand _startRenameNodeCommand;

        ICommand INodeManager.StartRenameNodeCommand => _startRenameNodeCommand;

        private void StartRenameNode(string name)
        {
            UpdateNodeCache(cache => cache.AddOrUpdate(new RenamingNodeItemViewModel(name, this)));
        }

        #endregion

        #region EndRenameNodeCommand

        private ReactiveCommand _endRenameNodeCommand;

        ICommand INodeManager.EndRenameNodeCommand => _endRenameNodeCommand;

        private async void EndRenameNode((string oldName, string newName) changedName)
        {
            var oldName = changedName.oldName;
            var newName = changedName.newName;
            var item = new NodeItemViewModel(newName, this);
            if (oldName == newName)
            {
                UpdateNodeCache(cache =>
                {
                    cache.AddOrUpdate(item);
                });
                return;
            }
            NodeItemViewModelCache.Edit(cache =>
            {
                cache.RemoveKey(oldName);
                cache.AddOrUpdate(item);
            });
            await Connection.RenameNodeAsync(oldName, newName);
            SelectedItemIndex = ItemViewModels.IndexOf(item);
        }

        #endregion

        #endregion

        #region 实现IDataManager

        #region DeleteDataCommand

        private ReactiveCommand _deleteDataCommand;

        ICommand IDataManager.DeleteDataCommand => _deleteDataCommand;

        private async void DeleteData(string name)
        {
            await Connection.DeleteDataAsync(name);
            DataItemViewModelCache.RemoveKey(name);
        }

        #endregion

        #endregion

        private string RootName => "...";

        public ReactiveCommand LoadFileCommand { get; private set; }

        public ReactiveCommand CreateFileCommand { get; private set; }

        public ReactiveCommand UpCommand { get; private set; }

        #region CreateNodeCommand

        public ReactiveCommand CreateNodeCommand { get; private set; }

        private async void CreateNode()
        {
            string newName = "NewNode_";
            string name = null;
            var names = NodeItemViewModelCache.Keys.Where(k => k.StartsWith(newName)).ToArray();
            for (int i = 1; true; i++)
            {
                name = $"{newName}{i}";
                if (!names.Contains(name))
                    break;
            }
            await Connection.CreateNodeAsync(name, false);
            var newNode = new NodeItemViewModel(name, this);
            NodeItemViewModelCache.AddOrUpdate(newNode);
            SelectedItemIndex = ItemViewModels.IndexOf(newNode);
        }

        #endregion

        private SourceList<string> NodeNameList { get; } = new SourceList<string>();

        private ReadOnlyObservableCollection<string> _nodeNames;

        public ReadOnlyObservableCollection<string> NodeNames => _nodeNames;

        private SourceCache<ItemViewModelBase, string> NodeItemViewModelCache { get; } = new SourceCache<ItemViewModelBase, string>(n => n.Name);

        private SourceCache<ItemViewModelBase, string> DataItemViewModelCache { get; } = new SourceCache<ItemViewModelBase, string>(d => d.Name);

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

        private Subject<bool> IsUpdating { get; set; }

        private Subject<string> FilePath { get; set; }

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
                NodeItemViewModelCache.Clear();
                DataItemViewModelCache.Clear();
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

            NodeItemViewModelCache.Edit(async cache =>
            {
                cache.Clear();
                cache.AddOrUpdate((await Connection.GetChildrenNodeNamesAsync()).Select(n => new NodeItemViewModel(n, this)));
            });

            DataItemViewModelCache.Edit(async cache =>
            {
                cache.Clear();
                cache.AddOrUpdate((await Connection.GetChildrenDataNamesAsync()).Select(n => new DataItemViewModel(n, this)));
            });
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
