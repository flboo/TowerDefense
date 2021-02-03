using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    [SerializeField]
    private Transform arrow = default;
    GameTile north, east, south, west, nextOnPath;
    private static
    Quaternion northRotation = Quaternion.Euler(90, 0, 0);
    Quaternion eastRotation = Quaternion.Euler(90, 90, 0);
    Quaternion southRotation = Quaternion.Euler(90, 180, 0);
    Quaternion westRotation = Quaternion.Euler(90, 270, 0);
    public bool HasPath => distance != int.MaxValue;
    int distance;
    public GameTile GrowPathNorth() => GrowPathTo(north);
    public GameTile GrowPathSouth() => GrowPathTo(south);
    public GameTile GrowPathEast() => GrowPathTo(east);
    public GameTile GrowPathWest() => GrowPathTo(west);
    GameTileContent content;

    public bool Isalternative { get; set; }

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

    private GameTile GrowPathTo(GameTile neighbor)
    {
        Debug.Assert(HasPath, "no path");
        if (!HasPath || neighbor == null || neighbor.HasPath)
        {
            return null;
        }
        neighbor.distance = distance + 1;
        neighbor.nextOnPath = this;
        return neighbor;
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





}
