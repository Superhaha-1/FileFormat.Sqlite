using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FileFormat.Sqlite.Models
{
    internal sealed class Node
    {
        [Key]
        public int Key { get; set; }

        public string Name { get; set; }

        public DateTime LastWriteTime { get; set; }

        public Node Parent { get; set; }

        public IList<Node> ChildrenNodes { get; set; }

        public IList<Data> ChildrenDatas { get; set; }

        public Node()
        {

        }

        //public Node(string name, int nodeKey)
        //{
        //    Name = name;
        //    NodeKey = nodeKey;
        //}
    }
}
