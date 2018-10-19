using FileFormat.Sqlite.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FileFormat.Sqlite
{
    public sealed class NodeConnection : IDisposable
    {
        internal NodeConnection(FileFormatContext context, Node node)
        {
            Context = context;
            Node = node;
        }

        private FileFormatContext Context { get; }

        private Node Node { get; set; }

        #region IDisposable Support

        private bool disposedValue = false; // 要检测冗余调用

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                Context.Dispose();

                // TODO: 将大型字段设置为 null。
                Node = null;

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        ~NodeConnection()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }

        #endregion

        private async Task<Data> GetDataAsync(string name)
        {
            name.VerifyName();
            return await Context
                .Entry(Node)
                .Collection(n => n.ChildrenDatas)
                .Query()
                .FirstOrDefaultAsync(d => d.Name == name);
        }

        private async Task<Node> GetNodeAsync(string name)
        {
            name.VerifyName();
            return await Context
                .Entry(Node)
                .Collection(n => n.ChildrenNodes)
                .Query()
                .FirstOrDefaultAsync(n => n.Name == name);
        }

        public async Task<byte[]> ReadDataAsync(string name)
        {
            var data = await GetDataAsync(name);
            if (data == null)
                throw new Exception($"不存在名为{name}的数据");
            return data.Value;
        }

        public async Task SaveDataAsync(string name, byte[] value)
        {
            var data = await GetDataAsync(name);
            if (data == null)
                await Context
                    .Datas
                    .AddAsync(new Data(name, value, Node.Key));
            else
                data.Value = value;
            await Context.SaveChangesAsync();
        }

        public async Task<string> MoveUpAsync()
        {
            if (Node.NodeKey == null)
                throw new Exception("已经是根节点了");
            Node = await Context
                .Nodes
                .FindAsync(Node.NodeKey.Value);
            return Node.Name;
        }

        public async Task MoveUpToAsync(string name)
        {
            name.VerifyName();
            while (await MoveUpAsync() != name)
            {

            }
        }

        public async Task MoveUpToRoot()
        {
            Node = await Context.GetRootNodeAsync();
        }

        public async Task MoveDownToAsync(string name)
        {
            Node = await GetNodeAsync(name) ?? throw new Exception($"不存在名为{name}的Node");
        }

        public async Task<string[]> GetChildrenNodeNamesAsync()
        {
            return await Context
                .Entry(Node)
                .Collection(n => n.ChildrenNodes)
                .Query()
                .Select(n => n.Name)
                .ToArrayAsync();
        }

        public async Task<string[]> GetChildrenDataNamesAsync()
        {
            return await Context
                .Entry(Node)
                .Collection(n => n.ChildrenDatas)
                .Query()
                .Select(d => d.Name)
                .ToArrayAsync();
        }

        public async Task DeleteNodeAsync(string name)
        {
            var node = await GetNodeAsync(name);
            if (node == null)
                throw new Exception($"不存在名为{name}的Node");
            Context
                .Nodes
                .Remove(node);
            await Context.SaveChangesAsync();
        }

        public async Task CreateNodeAsync(string name)
        {
            var node = await GetNodeAsync(name);
            if (node == null)
            {
                await Context.Nodes.AddAsync(new Node(name, Node.Key));
                await Context.SaveChangesAsync();
            }
        }

        public async Task DeleteDataAsync(string name)
        {
            var data = await GetDataAsync(name);
            if(data==null)
                throw new Exception($"不存在名为{name}的Data");
            Context
                .Datas
                .Remove(data);
            await Context.SaveChangesAsync();
        }
    }
}
