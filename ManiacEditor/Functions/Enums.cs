namespace ManiacEditor
{
    internal enum DuplicateAction
    {
        Info,
        Keep,
        Abort
    }

    public enum FlipDirection : ushort
    {
        Horizontal = 1024,
        Veritcal = 2048
    }

    internal enum ScrollDir : int
    {
        X = 0,
        Y = 1
    }
}
