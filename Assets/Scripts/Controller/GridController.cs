using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController
{
    public GridModel Model { get; private set; }

    public GridController(int width, int height)
    {
        Model = new GridModel(width, height);
    }

    public void OnCellClicked(int x, int y)
    {
        Debug.Log($"���� �� ������ [{x}, {y}]");
    }
}
