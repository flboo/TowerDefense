using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;
public class GameBoard : MonoBehaviour
{
    [SerializeField]
    private Transform ground = default;
    [SerializeField]
    private GameTile tilePrefab = default;
    Vector2Int size;
    GameTile[] tiles;
    Queue<GameTile> searchFrontier = new Queue<GameTile>();
    GameTileContentFactory contentFactory;

    public void Initialize(Vector2Int size, GameTileContentFactory content)
    {
        contentFactory = content;
        this.size = size;
        this.ground.localScale = new Vector3(size.x, size.y, 1f);

        tiles = new GameTile[size.x * size.y];
        Vector2 offset = new Vector2((size.x - 1) * 0.5f, (size.y - 1) * 0.5f);
        for (int y = 0, i = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++, i++)
            {
                GameTile tile = tiles[i] = Instantiate(tilePrefab);
                tile.transform.SetParent(transform, false);
                tile.transform.localPosition = new Vector3(x - offset.x, 0f, y - offset.y);
                if (x > 0)
                {
                    GameTile.MakeEastWestNeighbors(tile, tiles[i - 1]);
                }
                if (y > 0)
                {
                    GameTile.MakeNorthSouthNeighbors(tile, tiles[i - size.x]);//妙啊   此思想我之前做塔防的时候咋就没想到呀
                }
                tile.Isalternative = (x & 1) == 0;
                if ((y & 1) == 0)
                {
                    tile.Isalternative = !tile.Isalternative;
                }
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
            }
        }
        // FindPaths();
        ToggleDestination(tiles[tiles.Length / 2]);
    }

    public void ToggleDestination(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.Destination)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
            FindPaths();
        }
        else
        {
            tile.Content = contentFactory.Get(GameTileContentType.Destination);
            FindPaths();
        }
    }

    bool FindPaths()
    {
        foreach (GameTile tile in tiles)
        {
            if (tile.Content.Type == GameTileContentType.Destination)
            {
                tile.BecomeDestination();
                searchFrontier.Enqueue(tile);
            }
            else
            {
                tile.ClearPath();
            }
        }
        if (searchFrontier.Count == 0)
        {
            return false;
        }
        while (searchFrontier.Count > 0)
        {
            GameTile tile1 = searchFrontier.Dequeue();
            if (tile1 != null)
            {
                if (tile1.Isalternative)
                {
                    searchFrontier.Enqueue(tile1.GrowPathNorth());
                    searchFrontier.Enqueue(tile1.GrowPathSouth());
                    searchFrontier.Enqueue(tile1.GrowPathEast());
                    searchFrontier.Enqueue(tile1.GrowPathWest());
                }
                else
                {
                    searchFrontier.Enqueue(tile1.GrowPathWest());
                    searchFrontier.Enqueue(tile1.GrowPathEast());
                    searchFrontier.Enqueue(tile1.GrowPathSouth());
                    searchFrontier.Enqueue(tile1.GrowPathNorth());
                }
            }
        }
        foreach (var tile in tiles)
        {
            tile.ShowPath();
        }
        return true;
    }

    public GameTile GetTile(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            int x = (int)(hit.point.x + size.x * 0.5f);
            int y = (int)(hit.point.z + size.y * 0.5f);
            if (x >= 0 && x < size.x && y >= 0 && y < size.y)
            {
                return tiles[x + y * size.x];
            }
        }
        return null;
    }


}