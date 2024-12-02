using UnityEngine;
using System;
/// <summary>
/// Extension methods for the Direction enumeration.
/// </summary>
public static class DirectionExtensions
{
    /// <summary>
    /// Gets the vector corresponding to the direction.
    /// </summary>
    /// <param name="direction">The direction.</param>
    /// <returns>The corresponding vector.</returns>
    public static Vector3Int GetVector(this Direction direction)
    {
        return direction switch
        {
            Direction.up => Vector3Int.up,
            Direction.down => Vector3Int.down,
            Direction.right => Vector3Int.right,
            Direction.left => Vector3Int.left,
            Direction.foreward => Vector3Int.forward,
            Direction.backwards => Vector3Int.back,
            _ => throw new Exception("Invalid input direction")
        };
    }
}



