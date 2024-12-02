using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Helper class for the voxel data.
/// </summary>
public static class VoxelHelper

{
    /// <summary>
    /// The directions of the faces of a voxel.
    /// </summary>
    private static Direction[] directions =
    {
        Direction.backwards,
        Direction.down,
        Direction.foreward,
        Direction.left,
        Direction.right,
        Direction.up
    };

    /// <summary>
    /// Gets the mesh data for a voxel.
    /// </summary>
    /// <param name="chunk"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="meshData"></param>
    /// <param name="VoxelType"></param>
    /// <returns></returns>
    public static MeshData GetMeshData
        (ChunkData chunk, int x, int y, int z, MeshData meshData, VoxelType VoxelType)
    {
        if (VoxelType == VoxelType.Air || VoxelType == VoxelType.Nothing)
            return meshData;

        foreach (Direction direction in directions)
        {
            var neighbourVoxelCoordinates = new Vector3Int(x, y, z) + direction.GetVector();
            var neighbourVoxelType = Chunk.ObtainVoxelFromChunkCoordinates(chunk, neighbourVoxelCoordinates);

            if (neighbourVoxelType != VoxelType.Nothing &&
                VoxelDataManager.voxelTextureDataDictionary[neighbourVoxelType].isSolid == false)
            {

                if (VoxelType == VoxelType.Water)
                {
                    if (neighbourVoxelType == VoxelType.Air)
                        meshData.waterMesh = GetFaceDataIn(direction, chunk, x, y, z, meshData.waterMesh, VoxelType);
                }
                else
                {
                    meshData = GetFaceDataIn(direction, chunk, x, y, z, meshData, VoxelType);
                }

            }
        }

        return meshData;
    }

    /// <summary>
    /// Generates face data for the specified direction in the chunk.
    /// </summary>
    /// <param name="direction">The direction of the face.</param>
    /// <param name="chunk">The chunk data.</param>
    /// <param name="x">The x-coordinate of the voxels.</param>
    /// <param name="y">The y-coordinate of the voxels.</param>
    /// <param name="z">The z-coordinate of the voxel.</param>
    /// <param name="meshData">The existing mesh data.</param>
    /// <param name="VoxelType">The type of the voxel.</param>
    /// <returns>The updated mesh data.</returns>
    public static MeshData GetFaceDataIn(Direction direction, ChunkData chunk, int x, int y, int z, MeshData meshData,
        VoxelType VoxelType)
    {
        GetFaceVertices(direction, x, y, z, meshData, VoxelType);
        meshData.AddQuadTriangles(VoxelDataManager.voxelTextureDataDictionary[VoxelType].generatesCollider);
        meshData.uv.AddRange(FaceUVs(direction, VoxelType));

        return meshData;
    }

    /// <summary>
    /// Adds vertices for a face in the specified direction to the mesh data.
    /// </summary>
    /// <param name="direction">The direction of the face.</param>
    /// <param name="x">The x-coordinate of the voxel.</param>
    /// <param name="y">The y-coordinate of the voxel.</param>
    /// <param name="z">The z-coordinate of the voxel.</param>
    /// <param name="meshData">The mesh data to add vertices to.</param>
    /// <param name="VoxelType">The type of the voxel.</param>
    public static void GetFaceVertices(Direction direction, int x, int y, int z, MeshData meshData, VoxelType voxelType)
    {
        var generatesCollider = VoxelDataManager.voxelTextureDataDictionary[voxelType].generatesCollider;
        // Order of vertices matters for the normals and how we render the mesh
        switch (direction)
        {
            case Direction.backwards:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                break;
            case Direction.foreward:
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                break;
            case Direction.left:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                break;
            case Direction.right:
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                break;
            case Direction.down:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatesCollider);
                break;
            case Direction.up:
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatesCollider);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Calculates UV coordinates for a face based on the voxel type and direction.
    /// </summary>
    /// <param name="direction">The direction of the face.</param>
    /// <param name="VoxelType">The type of the voxel.</param>
    /// <returns>An array of UV coordinates.</returns>
    public static Vector2[] FaceUVs(Direction direction, VoxelType voxelType)
    {
        Vector2[] UVs = new Vector2[4];
        var tilePos = TexturePosition(direction, voxelType);

        UVs[0] = new Vector2(
            VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.tileSizeX - VoxelDataManager.textureOffset,
            VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.textureOffset);

        UVs[1] = new Vector2(
            VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.tileSizeX - VoxelDataManager.textureOffset,
            VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.tileSizeY - VoxelDataManager.textureOffset);

        UVs[2] = new Vector2(VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.textureOffset,
            VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.tileSizeY - VoxelDataManager.textureOffset);

        UVs[3] = new Vector2(VoxelDataManager.tileSizeX * tilePos.x + VoxelDataManager.textureOffset,
            VoxelDataManager.tileSizeY * tilePos.y + VoxelDataManager.textureOffset);

        return UVs;
    }

    /// <summary>
    /// Determines the position of the texture for a given voxel type and direction.
    /// </summary>
    /// <param name="direction">The direction of the face.</param>
    /// <param name="VoxelType">The type of the voxel.</param>
    /// <returns>The position of the texture in the texture atlas.</returns>
    public static Vector2Int TexturePosition(Direction direction, VoxelType voxelType)
    {
        return direction switch
        {
            Direction.up => VoxelDataManager.voxelTextureDataDictionary[voxelType].up,
            Direction.down => VoxelDataManager.voxelTextureDataDictionary[voxelType].down,
            _ => VoxelDataManager.voxelTextureDataDictionary[voxelType].side
        };
    }
}

