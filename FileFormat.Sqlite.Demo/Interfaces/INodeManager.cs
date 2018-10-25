namespace FileFormat.Sqlite.Demo.Interfaces
{
    public interface INodeManager
    {
        void DeleteNode(string name);

        void EnterNode(string name);
    }
}
