using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FileFormat.Sqlite.Models
{
    internal sealed class Node
    {
        [Key]
        public int Key { get; set; }

        public string Name { get; set; }

        public int? NodeKey { get; set; }

        public IList<Node> ChildrenNodes { get; set; }

        public IList<Data> ChildrenDatas { get; set; }

        private Node()
        {

        }

        public Node(string name, int nodeKey)
        {
            Name = name;
            NodeKey = nodeKey;
        }
    }
}
