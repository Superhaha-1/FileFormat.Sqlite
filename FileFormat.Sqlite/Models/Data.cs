using System;
using System.ComponentModel.DataAnnotations;

namespace FileFormat.Sqlite.Models
{
    internal class Data
    {
        [Key]
        public int Key { get; set; }

        public string Name { get; set; }

        public DateTime LastWriteTime { get; set; }

        public virtual byte[] Value { get; set; }

        public Node Parent { get; set; }

        public Data()
        {

        }

        //public Data(string name, byte[] value, Node parent)
        //{
        //    Name = name;
        //    Value = value;
        //    Parent = parent;
        //}
    }
}
