using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Utility class for processing the data matrices.
/// </summary>
public static class DataProcessing
{
    /// <summary>
    /// List of 8-directional offsets for neighboring positions.
    /// </summary>
    public static List<Vector2Int> Directions = new List<Vector2Int>
    {
        new Vector2Int( 0, 1), // N
        new Vector2Int( 1, 1), // NE
        new Vector2Int( 1, 0), // E
        new Vector2Int(-1, 1), // SE
        new Vector2Int(-1, 0), // S
        new Vector2Int(-1,-1), // SW
        new Vector2Int( 0,-1), // W
        new Vector2Int( 1,-1)  // NW
    };

    /// <summary>
    /// Finds local maximum positions in the data matrix.
    /// </summary>
    /// <param name="dataMatrix">The data matrix to search.</param>
    /// <param name="xCoord">The X-coordinate of the starting position in the world.</param>
    /// <param name="zCoord">The Z-coordinate of the starting position in the world.</param>
    /// <returns>List of local maxima positions.</returns>
    public static List<Vector2Int> FindLocalMaximum(float[,] dataMatrix, int xCoord, int zCoord)
    {
        List<Vector2Int> maximas = new List<Vector2Int>();
        for (int x = 0; x < dataMatrix.GetLength(0); x++)
        {
            for (int y = 0; y < dataMatrix.GetLength(1); y++)
            {
                float noiseVal = dataMatrix[x, y];
                if (CheckNeighbours(dataMatrix, x, y, (neighbourNoise) => neighbourNoise < noiseVal))
                {
                    maximas.Add(new Vector2Int(xCoord + x, zCoord + y));
                }
            }
        }
        return maximas;
    }

    /// <summary>
    /// Checks if all neighboring positions satisfy the success condition.
    /// </summary>
    /// <param name="dataMatrix">The data matrix to check neighbors against.</param>
    /// <param name="x">The X-coordinate of the current position in the matrix.</param>
    /// <param name="y">The Y-coordinate of the current position in the matrix.</param>
    /// <param name="successCondition">The success condition function to be satisfied by neighboring positions.</param>
    /// <returns>True if all neighbors satisfy the success condition, otherwise false.</returns>
    private static bool CheckNeighbours(float[,] dataMatrix, int x, int y, Func<float, bool> successCondition)
    {
        foreach (var dir in Directions)
        {
            var newX = x + dir.x;
            var newY = y + dir.y;

            if (newX < 0 || newX >= dataMatrix.GetLength(0) || newY < 0 || newY >= dataMatrix.GetLength(1))
            {
                continue;
            }

            if (!successCondition(dataMatrix[newX, newY]))
            {
                return false;
            }
        }
        return true;
    }
}
