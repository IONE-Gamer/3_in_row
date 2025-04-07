using System;

[Serializable]
public class GridModel
{
    // Размеры сетки
    public int Width { get; private set; }
    public int Height { get; private set; }
    public CellModel[,] Cells { get; private set; }
    
    public GridModel(int width, int height)
    {
        Width = width;
        Height = height;
        Cells = new CellModel[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cells[x, y] = new CellModel(x, y);
            }
        }
    }
}
