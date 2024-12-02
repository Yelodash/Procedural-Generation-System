using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// data structure representing tree data.
/// </summary>
public class TreeData
{
    /// <summary>
    /// List of positions where trees are located.
    /// </summary>
    public List<Vector2Int> treePositions = new List<Vector2Int>();

    /// <summary>
    /// List of solid leaf positions for trees.
    /// </summary>
    public List<Vector3Int> treeLeafesSolid = new List<Vector3Int>();
}
