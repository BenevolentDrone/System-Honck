using System;
using System.Collections.Generic;
using UnityEngine;

public class TileField : MonoBehaviour
{
    private const int xShift = 16;
    
    private static readonly int yMask = IntPow(2, xShift) - 1;
    
    [SerializeField]
    private Vector2 tileSize;
    public Vector2 TileSize { get { return tileSize; } }
    
    private Dictionary<int, Tile> tiles = new Dictionary<int, Tile>();
    
    public void AddTile(Tile tile)
    {
        tiles[tile.Coordinates] = tile;
    }
    
    public Tile GetTile(int coordinates)
    {
        if (!tiles.ContainsKey(coordinates))
            return null;
        
        return tiles[coordinates];
    }
    
    public Tile GetTile(int coordinates, int xOffset, int yOffset)
    {
        int x, y;
        
        GetIntCoordinates(coordinates, out x, out y);
        
        x += xOffset;
        
        y += yOffset;
        
        int newIntCoordinates = ToIntCoordinates(x, y);
        
        if (!tiles.ContainsKey(newIntCoordinates))
            return null;
        
        return tiles[newIntCoordinates];
    }
    
    public Tile GetTile(Vector2 coordinates)
    {
        int intCoordinates = ToIntCoordinates(coordinates);
        
        if (!tiles.ContainsKey(intCoordinates))
            return null;
        
        return tiles[intCoordinates];
    }

    public void GetTilesInRange(Vector2 coordinates, float range, List<Tile> result)
    {
        result.Clear();

        int centerX, centerY;

        GetCellIntCoordinates(coordinates, out centerX, out centerY);

        int rangeInCellsX = Math.Max(Round(range / tileSize.x), 1) + 1;
        int rangeInCellsY = Math.Max(Round(range / tileSize.y), 1) + 1;

        for (int x = -rangeInCellsX; x <= rangeInCellsX; x++)
            for (int y = -rangeInCellsY; y <= rangeInCellsY; y++)
            {
                Vector2 tileCoordinates = new Vector2(tileSize.x * (centerX + x), tileSize.y * (centerY + y));

                if ((tileCoordinates - coordinates).sqrMagnitude < range * range)
                {
                    int tileIntCoordinates = ToIntCoordinates(centerX + x, centerY + y);

                    if (tiles.ContainsKey(tileIntCoordinates))
                        result.Add(tiles[tileIntCoordinates]);
                }
            }
    }
    
    public int ToIntCoordinates(Vector2 coordinates)
    {
        int x = (int)((coordinates.x + tileSize.x / 2f) / tileSize.x);
        
        int y = (int)((coordinates.y + tileSize.y / 2f) / tileSize.y);
        
        return (x << xShift) | y;
    }

    public int ToIntCoordinates(int x, int y)
    {
        return (x << xShift) | y;
    }
    
    public void GetIntCoordinates(int intCoordinates, out int x, out int y)
    {
        x = intCoordinates >> xShift;
        
        y = intCoordinates & yMask;
    }
    
    public void GetCellIntCoordinates(Vector2 coordinates, out int x, out int y)
    {
        x = (int)((coordinates.x + tileSize.x / 2f) / tileSize.x);

        y = (int)((coordinates.y + tileSize.y / 2f) / tileSize.y);
    }

    public Vector2 GetCellCoordinates(Vector2 coordinates)
    {
        int x = (int)((coordinates.x + tileSize.x / 2f) / tileSize.x);

        int y = (int)((coordinates.y + tileSize.y / 2f) / tileSize.y);

        return new Vector2(tileSize.x * x, tileSize.y * y);
    }
    
    public Vector2 ToVector2Coordinates(int intCoordinates)
    {
        int x = intCoordinates >> xShift;
        
        int y = intCoordinates & yMask;
        
        return new Vector2(tileSize.x * x, tileSize.y * y);
    }
    
    public Vector2 ToVector2Coordinates(int x, int y)
    {
        return new Vector2(tileSize.x * x, tileSize.y * y);
    }
    
    private static int IntPow(int x, uint pow)
    {
        int ret = 1;
        while (pow != 0)
        {
            if ((pow & 1) == 1)
                ret *= x;
            x *= x;
            pow >>= 1;
        }
        return ret;
    }

    private static int Round(float x)
    {
        return (int)(x + 0.5f);
    }
}