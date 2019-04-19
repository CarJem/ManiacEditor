namespace ManiacEditor.Actions
{
    public interface IAction
    {
        void Undo();
        IAction Redo();

        string Description { get; }
    }
}
