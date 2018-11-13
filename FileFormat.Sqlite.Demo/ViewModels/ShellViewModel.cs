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
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Windows.Threading;
using System.Windows;
using System.IO;

namespace FileFormat.Sqlite.Demo.ViewModels
{
    public sealed class ShellViewModel : ReactiveObject, ISupportsActivation, INodeManager, IDataManager
    {
        public ShellViewModel()
        {
            FilePath = new BehaviorSubject<string>(@"C:\Users\super\Desktop\Test.mrpd");
            this.WhenActivated(d =>
            {
                (LoadFileCommand = ReactiveCommand.Create(LoadFile)).DisposeWith(d);
                (UpCommand = ReactiveCommand.Create(Up, this.WhenAnyValue(s => s.SelectedNodeIndex).Select(i => i > 0))).DisposeWith(d);
                (CreateNodeCommand = ReactiveCommand.Create(CreateNode)).DisposeWith(d);
                (_deleteNodeCommand = ReactiveCommand.Create<string>(DeleteNode)).DisposeWith(d);
                (_enterNodeCommand = ReactiveCommand.Create<string>(EnterNode)).DisposeWith(d);
                (_startRenameNodeCommand = ReactiveCommand.Create<string>(StartRenameNode)).DisposeWith(d);
                (_endRenameNodeCommand = ReactiveCommand.Create<(string, string)>(EndRenameNode)).DisposeWith(d);
                (_deleteDataCommand = ReactiveCommand.Create<string>(DeleteData)).DisposeWith(d);
                NodeNameList.Connect().ObserveOnDispatcher(DispatcherPriority.Background).Bind(out _nodeNames).Subscribe().DisposeWith(d);
                NodeItemViewModelCache.Connect().Sort(SortExpressionComparer<ItemViewModelBase>.Ascending(i => i.Name)).Concat(DataItemViewModelCache.Connect().Sort(SortExpressionComparer<ItemViewModelBase>.Ascending(i => i.Name))).Bind(out _itemViewModels).Subscribe().DisposeWith(d);
                this.WhenAnyValue(s => s.SelectedNodeIndex).Skip(1).Subscribe(SelectedNodeIndexChanged).DisposeWith(d);
                FilePath.Subscribe(FilePathChanged).DisposeWith(d);
                FilePath.Select(f => f != null).ToProperty(this, s => s.HasFile, out _hasFile).DisposeWith(d);
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
            SelectedNodeIndex += 1;
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
            var item = new RenamingNodeItemViewModel(name, this);
            UpdateNodeCache(cache => cache.AddOrUpdate(item));
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
            else if(NodeItemViewModelCache.Keys.Contains(newName))
            {
                await this.ShowProgressAsync("警告", "与其他名称重复", 500);
                var renamingNodeItem = NodeItemViewModelCache.Items.First(i => i is RenamingNodeItemViewModel) as RenamingNodeItemViewModel;
                renamingNodeItem.NewName = newName;
                renamingNodeItem.Focus();
                SelectedItemIndex = ItemViewModels.IndexOf(renamingNodeItem);
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
            var newNode = new RenamingNodeItemViewModel(name, this);
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

        private BehaviorSubject<string> FilePath { get; }

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
            if (!File.Exists(filePath))
                return;
            await this.ShowProgressAsync("请等待", "正在加载文件", async () =>
              {
                  Connection?.Dispose();
                  var fileConnection = new FileConnection(filePath);
                  Connection = await fileConnection.ConnectRootNodeAsync();
                  NodeNameList.Edit(cache =>
                   {
                       cache.Clear();
                       cache.Add(RootName);
                   });
                  if (SelectedNodeIndex == -1)
                      SelectedNodeIndex = 0;
              });
        }

        private async void SelectedNodeIndexChanged(int index)
        {
            if (index == -1)
            {
                SelectedNodeIndex = 0;
                return;
            }

            int lastIndex = NodeNameList.Count - 1;
            if (index < lastIndex)
            {
                NodeNameList.RemoveRange(index + 1, lastIndex - index);
            }

            await Connection.MoveToAsync(NodeNameList.Items.Skip(1));

            NodeItemViewModelCache.Edit(async cache =>
            {
                cache.Clear();
                var names = await Connection.GetChildrenNodeNamesAsync();
                cache.AddOrUpdate(names.Select(n => new NodeItemViewModel(n, this)));
            });

            DataItemViewModelCache.Edit(async cache =>
            {
                cache.Clear();
                var names = await Connection.GetChildrenDataNamesAsync();
                cache.AddOrUpdate(names.Select(n => new DataItemViewModel(n, this)));
            });
        }

        private void Up()
        {
            SelectedNodeIndex -= 1;
        }
    }
}
