using System;
using UnityEngine;
using System.Collections.Generic;
public class Board : MonoBehaviour
{
    public Block[,] Grid;
    [SerializeField] private int _width = 10;
    [SerializeField] private int _height = 10;
    [SerializeField] private GameObject[] _blocks;
    private bool _isFalling;
    private Block _selectedBlock;
    private void Awake()
    {        
        Grid = new Block[_width, _height * 2];
        PreSpawnBlocks();
    }
    private void PreSpawnBlocks() { for (int i = 0; i < _width * _height; i++) CreateBlock(i % _width, i / _width, UnityEngine.Random.Range(0, _blocks.Length)); }
    private void Update()
    {
        _isFalling = IsAnyBlockIsFalling();
        if (!_isFalling) Spawn();
        MoveBlockDownAndMatch();
    }
    private void Spawn()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0, count = 0; y < _height; y++)
            {
                if (Grid[x, y] || Grid[x, _height + count]) continue;
                CreateBlock(x, _height + count, UnityEngine.Random.Range(0, _blocks.Length));
                count++;
            }
        }
    }
    private void MoveBlockDownAndMatch()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0, count = 0; y < _height * 2; y++)
            {
                Block block = Grid[x, y];
                if (block == null) continue;
                if (block.ID == -1) break;
                
                if (Math.Abs(block.NY - count) > float.Epsilon)
                {
                    Grid[x, y] = null;
                    block.NY = count;
                    block.CheckForMatch = true;
                    Grid[x, count] = block;
                }
                else if (block.CheckForMatch && !_isFalling)
                {
                    SolveMatches(Match(block));
                    block.CheckForMatch = false;
                }
                count++;
            }
        }
    }
    private void CreateBlock(int x, int y, int blockId)
    {
        GameObject obj = Instantiate(_blocks[blockId], new Vector3(x, y), Quaternion.identity) as GameObject;
        Block block = obj.GetComponent<Block>();
        block.Initialize(x, y, blockId, this);
        Grid[x, y] = block;
    }
    private bool IsAnyBlockIsFalling()
    {
        for (int x = 0; x < _width; x++) for (int y = 0; y < _height; y++) if (Grid[x, y] != null && Grid[x, y].IsFalling()) return true;
        return false;
    }
    private List<Block> Match(Block b)
    {
        List<Block> output = new List<Block>(); 
        List<Block> tempMatches = new List<Block>();
        bool linked = false;
        for (int x = 0; x < _width; x++)
        {
            if (x == (int) b.X) linked = true;
            if (Grid[x, (int) b.Y] != null && !Grid[x, (int) b.Y].IsFalling() && Grid[x, (int) b.Y].ID == b.ID)
                tempMatches.Add(Grid[x, (int) b.Y]);
            else if (linked) break;
            else tempMatches.Clear();
        }
        if (tempMatches.Count >= 3) output.AddRange(tempMatches);
        tempMatches.Clear();
        linked = false;
        for (int y = 0; y < _height; y++)
        {
            if (y == (int) b.Y) linked = true;
            if (Grid[(int) b.X, y] && !Grid[(int) b.X, y].IsFalling() && Grid[(int) b.X, y].ID == b.ID)
                tempMatches.Add(Grid[(int) b.X, y]);
            else if (linked) break;
            else tempMatches.Clear();
        }
        if (tempMatches.Count >= 3) output.AddRange(tempMatches);
        return output;
    }
    private void SolveMatches(List<Block> matches) { foreach (Block block in matches) Destroy(block.gameObject); }
    public void OnSelect(Block block)
    {
        if (_isFalling) return;
        if (_selectedBlock != null)
        {
            if (_selectedBlock == block || !_selectedBlock.IsNeighbour(block)) _selectedBlock = null;
            else
            {
                _selectedBlock.SwapPlace(block);
                if (Match(block).Count < 3 && Match(_selectedBlock).Count < 3) _selectedBlock.SwapPlace(block);
                _selectedBlock = null;
            }
        }
        else _selectedBlock = block;
    }
}