using FileFormat.Sqlite.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FileFormat.Sqlite
{
    internal class FileFormatContext : DbContext
    {
        public DbSet<Parameter> Parameters { get; set; }

        public DbSet<Node> Nodes { get; set; }

        public DbSet<Data> Datas { get; set; }

        public FileFormatContext()
        {
            
        }

        public FileFormatContext(string path)
        {
            if (!File.Exists(path))
                throw new Exception("文件不存在");
            Path = path;
        }

        private string Path { get; } = "template.db";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={Path};Cache=shared");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Node>()
                .HasMany(n => n.ChildrenNodes)
                .WithOne(n => n.Parent)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Node>()
                .HasMany(n => n.ChildrenDatas)
                .WithOne(d => d.Parent)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Cascade);
        }

        /// <summary>
        /// 获取根节点
        /// </summary>
        /// <returns></returns>
        public async Task<Node> GetRootNodeAsync()
        {
            var node = await Task.Run(() => Nodes.Find(1));
            if (node == null)
            {
                node = new Node() { LastWriteTime = DateTime.Now };
                await Nodes.AddAsync(node);
                await SaveChangesAsync();
            }
            return node;
        }

        public async Task UpdateLastWriteTimeAsync(Node node, DateTime lastWriteTime)
        {
            if (node == null)
                return;
            node.LastWriteTime = lastWriteTime;
            var parent = node.Parent
            var parent = await Entry(node).Reference(n => n.Parent).Query().FirstOrDefaultAsync();
            await UpdateLastWriteTimeAsync(parent, lastWriteTime);
        }

    }
}
