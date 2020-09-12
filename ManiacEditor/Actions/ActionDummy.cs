namespace ManiacEditor.Actions
{
    public class ActionDummy : IAction
    {
        public string Description => string.Empty;

        public ActionDummy() { }
        public void Undo() { }
        public IAction Redo() { return this; }
    }
}
