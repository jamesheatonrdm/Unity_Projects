using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk {

	public Material cubeMaterial;
	public Block[,,] chunkData;
	public GameObject chunk;
	private UInt16[] _voxels = new ushort[16 * 16 * 16];

	public UInt16 this[int x, int y, int z]
	{
		get { return _voxels[x * 16 * 16 + y * 16 + z]; }
		set { _voxels[x * 16 * 16 + y * 16 + z] = value; }
	}

	void BuildChunk()
	{
		chunkData = new Block[World.chunkSize * 4, World.chunkSize * 4, World.chunkSize * 4];
		//create blocks
		for (float z = 0; z < World.chunkSize; z += 0.25f)
			for (float y = 0; y < World.chunkSize; y += 0.25f)
				for (float x = 0; x < World.chunkSize; x += 0.25f)
				{
					var voxelType = this[(int)x * 4, (int)y * 4, (int)z * 4];
					Vector3 pos = new Vector3(x, y, z);
					if(voxelType == 1)
						chunkData[(int) (x * 4), (int) (y * 4), (int) (z * 4)] = new Block(Block.BlockType.DIRT, pos,
											chunk.gameObject, this);
					else
						chunkData[(int)(x * 4), (int)(y * 4), (int)(z * 4)] = new Block(Block.BlockType.AIR, pos,
											chunk.gameObject, this);
				}
	}

	public void DrawChunk()
    {
		for (float z = 0; z < World.chunkSize; z += 0.25f)
			for (float y = 0; y < World.chunkSize; y += 0.25f)
				for (float x = 0; x < World.chunkSize; x += 0.25f)
				{
					try
                    {
						chunkData[(int)(x * 4), (int)(y * 4), (int)(z * 4)].Draw();
					}
					catch (NullReferenceException e)
                    {
						Vector3 pos = new Vector3(x, y, z);
						chunkData[(int)(x * 4), (int)(y * 4), (int)(z * 4)] = new Block(Block.BlockType.AIR, pos,
											chunk.gameObject, this);
						chunkData[(int)(x * 4), (int)(y * 4), (int)(z * 4)].Draw();

					}
					
				}
		CombineQuads();
	}

	// Use this for initialization
	// Use this for initialization
	public Chunk(Vector3 position, Material c)
	{

		chunk = new GameObject(World.BuildChunkName(position));
		chunk.transform.position = position;
		cubeMaterial = c;
		chunkData = new Block[World.chunkSize * 4, World.chunkSize * 4, World.chunkSize * 4];
		//BuildChunk();
	}

	// Update is called once per frame
	void Update () {
		
	}

	void CombineQuads()
	{
		//1. Combine all children meshes
		MeshFilter[] meshFilters = chunk.GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		int i = 0;
		while (i < meshFilters.Length)
		{
			combine[i].mesh = meshFilters[i].sharedMesh;
			combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
			i++;
		}

		//2. Create a new mesh on the parent object
		MeshFilter mf = (MeshFilter)chunk.gameObject.AddComponent(typeof(MeshFilter));
		mf.mesh = new Mesh();

		//3. Add combined meshes on children as the parent's mesh
		mf.mesh.CombineMeshes(combine);

		//4. Create a renderer for the parent
		MeshRenderer renderer = chunk.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		renderer.material = cubeMaterial;

		//5. Delete all uncombined children
		foreach (Transform quad in chunk.transform)
		{
			GameObject.Destroy(quad.gameObject);
		}

	}

	public void debug()
    {
		for (int x = 0; x < chunkData.GetLength(0); x++)
        {
			for (int y = 0; y < chunkData.GetLength(1); y++)
            {
				for (int z = 0; z < chunkData.GetLength(2); z++)
                {
					Debug.Log(chunkData[x, y, z]);
                }
            }
        }
    }
}
