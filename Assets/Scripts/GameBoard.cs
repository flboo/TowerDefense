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
    [SerializeField]
    private Texture2D gridTextture = default;
    Vector2Int size;
    GameTile[] tiles;
    Queue<GameTile> searchFrontier = new Queue<GameTile>();
    List<GameTile> spawnPoint = new List<GameTile>();
    GameTileContentFactory contentFactory;
    bool showPaths = false, showGrid = false;
    public int SpawnPointCount => spawnPoint.Count;
    public bool ShowPaths
    {
        get => showPaths;
        set
        {
            showPaths = value;
            if (showPaths)
            {
                foreach (GameTile tile in tiles)
                {
                    tile.ShowPath();
                }
            }
            else
            {
                foreach (GameTile tile in tiles)
                {
                    tile.HidePath();
                }
            }

        }
    }

    public bool ShowGrid
    {
        get => showGrid;
        set
        {
            showGrid = value;
            Material m = this.ground.GetComponent<MeshRenderer>().material;
            if (showGrid)
            {
                m.mainTexture = gridTextture;
                m.SetTextureScale("_MainTex", size);
            }
            else
            {
                m.mainTexture = null;
            }
        }
    }

    public void Initialize(Vector2Int size, GameTileContentFactory contentFactory)
    {
        this.contentFactory = contentFactory;
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
                tile.Content = this.contentFactory.Get(GameTileContentType.Empty);
            }
        }
        ToggleDestination(tiles[tiles.Length / 2]);
        ToggleSpawnPoint(tiles[0]);
    }

    public GameTile GetSpwanPoint(int index)
    {
        return spawnPoint[index];
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
            if (!FindPaths())
            {
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
        }
    }

    public void ToggleSpawnPoint(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.SpawnPoint)
        {
            if (spawnPoint.Count > 1)
            {
                spawnPoint.Remove(tile);
            }
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
        }
        else if (tile.Content.Type == GameTileContentType.Empty)
        {
            spawnPoint.Add(tile);
            tile.Content = contentFactory.Get(GameTileContentType.SpawnPoint);
        }
    }

    public void ToggleWall(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.Wall)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
            FindPaths();
        }
        else if (tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Wall);
            if (!FindPaths())
            {
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
        }
    }

    public void ToggleTower(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.Tower)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Empty);
            FindPaths();
        }
        else if (tile.Content.Type == GameTileContentType.Tower)
        {
            tile.Content = contentFactory.Get(GameTileContentType.Tower);
            if (!FindPaths())
            {
                tile.Content = contentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
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
        foreach (GameTile tile in tiles)
        {
            if (!tile.HasPath)
            {
                return false;
            }
        }
        if (showPaths)
        {
            foreach (GameTile tile in tiles)
            {
                tile.ShowPath();
            }
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