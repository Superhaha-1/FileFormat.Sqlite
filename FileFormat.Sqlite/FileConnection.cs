using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FileFormat.Sqlite
{
    public sealed class FileConnection
    {
        public FileConnection(string filePath)
        {
            FilePath = filePath;
            if (!File.Exists(filePath))
            {
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FileFormat.Sqlite.template.db"))
                {
                    using (var fileStream = new FileStream(filePath, FileMode.CreateNew))
                    {
                        stream.CopyTo(fileStream);
                    }
                }
            }
        }

        private string FilePath { get; }

        private string RootName => string.Empty;

        /// <summary>
        /// 连接至根节点
        /// </summary>
        /// <returns></returns>
        public async Task<NodeConnection> ConnectRootNodeAsync()
        {
            var context = new FileFormatContext(FilePath);
            var node = await context.GetRootNodeAsync();
            return new NodeConnection(context, node);
        }

        /// <summary>
        /// 连接至节点
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public async Task<NodeConnection> ConnectNodeAsync(IEnumerable<string> names)
        {
            var connection = await ConnectRootNodeAsync();
            try
            {
                foreach (var name in names)
                    await connection.MoveDownToAsync(name);
                return connection;
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }

        /// <summary>
        /// 连接至节点
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public async Task<NodeConnection> ConnectNodeAsync(string fullName)
        {
            var connection = await ConnectRootNodeAsync();
            try
            {
                string[] names = fullName.GetChidrenNames();
                foreach (var name in names)
                    await connection.MoveDownToAsync(name);
                return connection;
            }
            catch
            {
                connection.Dispose();
                throw;
            }
        }

        /// <summary>
        /// 新建节点
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public async Task CreateNodeAsync(string fullName)
        {
            string[] names = fullName.GetChidrenNames();
            using (var connection = await ConnectRootNodeAsync())
            {
                foreach(var name in names)
                    await connection.CreateNodeAsync(name);
            }
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public async Task DeleteNodeAsync(string fullName)
        {
            string[] names = fullName.GetChidrenNames();
            using (var connection = await ConnectRootNodeAsync())
            {
                foreach (var name in names.Take(names.Length - 1))
                    await connection.MoveDownToAsync(name);
                await connection.DeleteNodeAsync(names[names.Length - 1]);
            }
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task SaveDataAsync(string fullName, byte[] value)
        {
            string[] names = fullName.GetChidrenNames();
            using (var connection = await ConnectRootNodeAsync())
            {
                foreach (var name in names.Take(names.Length - 1))
                    await connection.CreateNodeAsync(name);
                await connection.SaveDataAsync(names[names.Length - 1], value);
            }
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public async Task<byte[]> ReadDataAsync(string fullName)
        {
            string[] names = fullName.GetChidrenNames();
            using (var connection = await ConnectRootNodeAsync())
            {
                foreach (var name in names.Take(names.Length - 1))
                    await connection.MoveDownToAsync(name);
                return await connection.ReadDataAsync(names[names.Length - 1]);
            }
        }

        public async Task DeleteDataAsync(string fullName)
        {
            string[] names = fullName.GetChidrenNames();
            using (var connection = await ConnectRootNodeAsync())
            {
                foreach (var name in names.Take(names.Length - 1))
                    await connection.MoveDownToAsync(name);
                await connection.DeleteDataAsync(names[names.Length - 1]);
            }
        }
    }
}
