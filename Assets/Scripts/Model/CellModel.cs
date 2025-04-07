using System;

[Serializable]
public class CellModel
{
    public int X { get; }
    public int Y { get; }

    public CellModel(int x, int y)
    {
        X = x;
        Y = y;
    }
}
