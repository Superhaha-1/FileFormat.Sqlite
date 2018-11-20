using FileFormat.Sqlite.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FileFormat.Sqlite
{
    public sealed class NodeConnection : IDisposable
    {
        internal NodeConnection(FileFormatContext context, Node node)
        {
            Context = context;
            Node = node;
        }

        /// <summary>
        /// 数据库连接
        /// </summary>
        private FileFormatContext Context { get; }

        /// <summary>
        /// 当前节点
        /// </summary>
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

        /// <summary>
        /// 获取数据(没有返回null)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private async Task<Data> GetDataAsync(string name)
        {
            name.ValidateName();
            return await Context.Entry(Node)
                .Collection(n => n.ChildrenDatas)
                .Query()
                .FirstOrDefaultAsync(d => d.Name == name);
        }

        /// <summary>
        /// 获取节点(没有返回null)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private async Task<Node> GetNodeAsync(string name)
        {
            name.ValidateName();
            return await Context.Entry(Node)
                .Collection(n => n.ChildrenNodes)
                .Query()
                .FirstOrDefaultAsync(n => n.Name == name);
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<byte[]> ReadDataAsync(string name)
        {
            var data = await GetDataAsync(name);
            if (data == null)
                throw new Exception($"不存在名为{name}的数据");
            return data.Value;
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task SaveDataAsync(string name, byte[] value)
        {
            value.ValidateData();
            var data = await GetDataAsync(name);
            if (data == null)
            {
                data = new Data() { Name = name, Parent = Node };
                await Context.Datas
                    .AddAsync(data);
            }
            data.Value = value;
            var lastWriteTime = DateTime.Now;
            data.LastWriteTime = lastWriteTime;
            await Context.UpdateLastWriteTimeAsync(Node, lastWriteTime);
            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// 移动至上一个节点
        /// </summary>
        /// <returns></returns>
        public async Task<string> MoveUpAsync()
        {
            var parent = await Context.GetParentAsync(Node);
            Node = parent ?? throw new Exception("已经是根节点了");
            return Node.Name;
        }

        /// <summary>
        /// 移动至指定的节点
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public async Task MoveToAsync(IEnumerable<string> names)
        {
            await MoveUpToRoot();
            foreach (var name in names)
            {
                name.ValidateName();
                await MoveDownToAsync(name);
            }
        }

        /// <summary>
        /// 移动至根节点
        /// </summary>
        /// <returns></returns>
        public async Task MoveUpToRoot()
        {
            Node = await Context.GetRootNodeAsync();
        }

        /// <summary>
        /// 往下移动至指定的节点
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task MoveDownToAsync(string name)
        {
            Node = await GetNodeAsync(name) ?? throw new Exception($"不存在名为{name}的Node");
        }

        /// <summary>
        /// 获取所有子节点的name
        /// </summary>
        /// <returns></returns>
        public async Task<string[]> GetChildrenNodeNamesAsync()
        {
            return await Context.Entry(Node)
                .Collection(n => n.ChildrenNodes)
                .Query()
                .Select(n => n.Name)
                .ToArrayAsync();
        }

        /// <summary>
        /// 获取所有子数据的name
        /// </summary>
        /// <returns></returns>
        public async Task<string[]> GetChildrenDataNamesAsync()
        {
            return await Context.Entry(Node)
                .Collection(n => n.ChildrenDatas)
                .Query()
                .Select(d => d.Name)
                .ToArrayAsync();
        }

        /// <summary>
        /// 重命名指定的节点
        /// </summary>
        /// <param name="name"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public async Task RenameNodeAsync(string name, string newName)
        {
            newName.ValidateName();
            var node = await GetNodeAsync(name);
            if (node == null)
                throw new Exception($"不存在名为{name}的Node");
            node.Name = newName;
            await Context.UpdateLastWriteTimeAsync(node, DateTime.Now);
            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// 删除指定的节点
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task DeleteNodeAsync(string name)
        {
            var node = await GetNodeAsync(name);
            if (node == null)
                throw new Exception($"不存在名为{name}的Node");
            Context.Nodes
                .Remove(node);
            await Context.UpdateLastWriteTimeAsync(Node, DateTime.Now);
            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// 新建节点
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isMove">是否移动至该节点</param>
        /// <returns></returns>
        public async Task CreateNodeAsync(string name, bool isMove = true)
        {
            var node = await GetNodeAsync(name);
            if (node == null)
            {
                node = new Node() { Name = name, Parent = Node };
                await Context.Nodes
                    .AddAsync(node);
                await Context.UpdateLastWriteTimeAsync(node, DateTime.Now);
                await Context.SaveChangesAsync();
            }
            if (isMove)
                Node = node;
        }

        /// <summary>
        /// 删除指定的数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task DeleteDataAsync(string name)
        {
            var data = await GetDataAsync(name);
            if (data == null)
                throw new Exception($"不存在名为{name}的Data");
            Context.Datas
                .Remove(data);
            await Context.UpdateLastWriteTimeAsync(Node, DateTime.Now);
            await Context.SaveChangesAsync();
        }
    }
}
