using System;
using UnityEngine;
public class Block : MonoBehaviour
{
    [HideInInspector] public int ID;
    [HideInInspector] public float X, Y, NY;
    [HideInInspector] public bool CheckForMatch;
    private Board _board;
    
    public void Initialize(int x, int y, int blockId, Board board)
    {
        _board = board;
        ID = blockId; 
        X = x;
        Y = NY = y;
    }
    
    public bool IsFalling() => Math.Abs(Y - NY) > float.Epsilon;
    public bool IsNeighbour(Block block) => Mathf.Sqrt(Mathf.Pow(X - block.X, 2) + Mathf.Pow(Y - block.Y, 2)) <= 1f + float.Epsilon;
    
    private void Update()
    {
        if (NY < Y) Y -= Time.deltaTime * 7;
        else Y = NY; 
        transform.position = new Vector3(X, Y);
    }
    private void OnMouseDown() { _board.OnSelect(this); }
    public void SwapPlace(Block block)
    {
        (X, block.X) = (block.X, X);
        (Y, block.Y) = (block.Y, Y);
        (NY, block.NY) = (block.NY, NY);
        (_board.Grid[(int) X, (int) Y], _board.Grid[(int) block.X, (int) block.Y]) = (_board.Grid[(int) block.X, (int) block.Y], _board.Grid[(int) X, (int) Y]);
        block.CheckForMatch = CheckForMatch = true;
    }
}