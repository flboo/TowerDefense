﻿using UnityEngine;

public enum GameTileContentType
{
    Empty,
    Destination,
    Wall,
    SpawnPoint,
}

public enum Direction
{
    North,
    East,
    South,
    West,
}

public enum DirectionChange
{
    None,
    TurnRight,
    TurnLeft,
    TurnAround,
}

public static class DirectionExtension
{
    static Quaternion[] rotations =
    {
        Quaternion.identity,
        Quaternion.Euler(0f,90f,0f),
        Quaternion.Euler(0f,180f,0f),
        Quaternion.Euler(0f,270f,0f)
    };

    private static Vector3[] halfVectors =
    {
        Vector3.forward*0.5f,
        Vector3.right*0.5f,
        Vector3.back*0.5f,
        Vector3.left*0.5f,
    };

    public static Vector3 GetHalfVector(this Direction direction)
    {
        return halfVectors[(int)direction];
    }

    public static Quaternion GetRotation(this Direction direction)
    {
        return rotations[(int)direction];
    }

    public static DirectionChange GetDirectionChangeTo(this Direction current, Direction next)
    {
        if (current == next)
        {
            return DirectionChange.None;
        }
        else if (current + 1 == next || current - 3 == next)
        {
            return DirectionChange.TurnRight;
        }
        else if (current - 1 == next || current + 3 == next)
        {
            return DirectionChange.TurnLeft;
        }
        return DirectionChange.TurnAround;
    }

    public static float GetAngle(this Direction direction)
    {
        return (float)direction * 90f;
    }



}