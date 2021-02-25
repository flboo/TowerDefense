using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameTile : MonoBehaviour
{
    [SerializeField]
    private Transform arrow = default;
    GameTile north, east, south, west, nextOnPath;
    Quaternion northRotation = Quaternion.Euler(90, 0, 0);
    Quaternion eastRotation = Quaternion.Euler(90, 90, 0);
    Quaternion southRotation = Quaternion.Euler(90, 180, 0);
    Quaternion westRotation = Quaternion.Euler(90, 270, 0);
    public bool HasPath => distance != int.MaxValue;
    int distance;
    public GameTile GrowPathNorth() => GrowPathTo(north, Direction.South);
    public GameTile GrowPathSouth() => GrowPathTo(south, Direction.North);
    public GameTile GrowPathEast() => GrowPathTo(east, Direction.West);
    public GameTile GrowPathWest() => GrowPathTo(west, Direction.East);
    GameTileContent content;

    public bool Isalternative { get; set; }
    public GameTile NextTileOnPath => nextOnPath;
    public Vector3 ExitPoint { get; private set; }
    public Direction PathDirection { get; private set; }


    public GameTileContent Content
    {
        get => content;
        set
        {
            Debug.Assert(value != null, "null assigned ti content");
            if (content != null)
            {
                content.Recycle();
            }
            content = value;
            content.transform.localPosition = transform.localPosition;
        }
    }

    public void ClearPath()
    {
        distance = int.MaxValue;
        nextOnPath = null;
    }

    public void BecomeDestination()
    {
        distance = 0;
        nextOnPath = null;
        ExitPoint = transform.localPosition;
    }

    public void ShowPath()
    {
        if (distance == 0)
        {
            arrow.gameObject.SetActive(false);
            return;
        }
        arrow.gameObject.SetActive(true);
        arrow.localRotation =
        nextOnPath == north ? northRotation :
        nextOnPath == east ? eastRotation :
        nextOnPath == south ? southRotation :
        westRotation;
    }

    private GameTile GrowPathTo(GameTile neighbor, Direction direction)
    {
        Debug.Assert(HasPath, "no path");
        if (!HasPath || neighbor == null || neighbor.HasPath)
        {
            return null;
        }
        neighbor.distance = distance + 1;
        neighbor.nextOnPath = this;
        neighbor.ExitPoint = neighbor.transform.localPosition + direction.GetHalfVector();
        neighbor.PathDirection = direction;
        return neighbor.content.Type != GameTileContentType.Wall ? neighbor : null;
    }

    public static void MakeEastWestNeighbors(GameTile east, GameTile west)
    {
        Debug.Assert(west.east == null && east.west == null, "Redefines neighbors");
        west.east = east;
        east.west = west;
    }

    public static void MakeNorthSouthNeighbors(GameTile north, GameTile south)
    {
        Debug.Assert(north.south == null && south.north == null, "Redefines neighbors");
        north.south = south;
        south.north = north;
    }

    public void HidePath()
    {
        arrow.gameObject.SetActive(false);
    }
}