using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellView : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }
    private GridController controller;

    public void Init(int x, int y, GridController gridController)
    {
        X = x;
        Y = y;
        controller = gridController;
    }

    private void OnMouseDown()
    {
        controller.OnCellClicked(X, Y);
    }
}
