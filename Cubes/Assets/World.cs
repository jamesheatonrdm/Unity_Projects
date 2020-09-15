using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class World : MonoBehaviour
{

	public Material textureAtlas;
	public static int columnHeight = 1;
	public static int chunkSize = 4;
	public static int worldSize = 2;
	public static Dictionary<string, Chunk> chunks;

	int voxelIndex = 0;
	

	public Dictionary<ChunkId, Chunk> Chunks = new Dictionary<ChunkId, Chunk>();

	Dictionary<Vector3Int, int> voxels = new Dictionary<Vector3Int, int>();

	public string filename = "VoxelGrid.dat";

	public UInt16 this[float x, float y, float z]
	{
		get
		{
			var chunk = Chunks[ChunkId.FromWorldPos(x, y, z)];
			//return chunk[x & 0xF, y & 0xF, z & 0xF];
			return chunk[(int)(x % 4) * 4, (int)(y % 4) * 4, (int)(z % 4) * 4];
		}

		set
		{
			var chunk = Chunks[ChunkId.FromWorldPos(x, y, z)];
			//chunk[x & 0xF, y & 0xF, z & 0xF] = value;
			chunk[(int)(x % 4) * 4, (int)(y % 4) * 4, (int)(z % 4) * 4] = value;
		}
	}

	public static string BuildChunkName(Vector3 v)
	{
		return (int)v.x + "_" +
					 (int)v.y + "_" +
					 (int)v.z;
	}

	IEnumerator BuildWorld()
	{
		foreach(var voxel in voxels)
        {
			var x = (voxel.Key.x / 4f) + .5f;
			var y = (voxel.Key.y / 4f) + .5f;
			var z = (voxel.Key.z / 4f) + .5f;
			UInt16 voxelType = 1;
			try
            {
				this[x, y, z] = voxelType;
				Chunk curChunk = Chunks[ChunkId.FromWorldPos(x, y, z)];
				Vector3 pos = new Vector3((x / 4), y / 4, z / 4);

				curChunk.chunkData[(int)(x % 4) * 4, (int)(y % 4) * 4, (int)(z % 4) * 4] = new Block(Block.BlockType.DIRT, pos,
					curChunk.chunk, curChunk);
            }
			catch (KeyNotFoundException e)
            {
				Vector3 chunkPosition = new Vector3((x / 4), (y / 4), (z / 4));
				Chunk c = new Chunk(chunkPosition, textureAtlas);
				c.chunk.transform.parent = this.transform;
				Chunks.Add(new ChunkId((int)(x / 4), (int)(y / 4), (int)(z / 4)), c);
				c.chunkData[(int)(x % 4) * 4, (int)(y % 4) * 4, (int)(z % 4) * 4] = new Block(Block.BlockType.DIRT, chunkPosition,
					c.chunk, c);

			}
			catch (ArgumentException e)
            {
				Debug.Log(e.Message);
            }
			
		
		}

		foreach (KeyValuePair<ChunkId, Chunk> c in Chunks)
        {
			c.Value.DrawChunk();
			yield return null;
        }

		/*
		for (int z = 0; z < worldSize; z++)
			for (int x = 0; x < worldSize; x++)
				for (int y = 0; y < columnHeight; y++)
				{
					Vector3 chunkPosition = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);
					Chunk c = new Chunk(chunkPosition, textureAtlas);
					c.chunk.transform.parent = this.transform;
					chunks.Add(c.chunk.name, c);

				}

		foreach (KeyValuePair<string, Chunk> c in chunks)
		{
			c.Value.DrawChunk();
			yield return null;
		}*/

	}

	IEnumerator BuildChunkColumn()
	{
		for (int i = 0; i < columnHeight; i++)
		{
			Vector3 chunkPosition = new Vector3(this.transform.position.x,
												i * chunkSize,
												this.transform.position.z);
			Chunk c = new Chunk(chunkPosition, textureAtlas);
			c.chunk.transform.parent = this.transform;
			chunks.Add(c.chunk.name, c);
		}

		foreach (KeyValuePair<string, Chunk> c in chunks)
		{
			c.Value.DrawChunk();
			yield return null;
		}

	}

	// Use this for initialization
	void Start()
	{
		chunks = new Dictionary<string, Chunk>();
		this.transform.position = Vector3.zero;
		this.transform.rotation = Quaternion.identity;
		LoadFromFile();
		//AssignVoxels();
		StartCoroutine(BuildWorld());
	}

	void LoadFromFile()
    {
		StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/" + filename);
		BinaryReader br = new BinaryReader(sr.BaseStream);
		voxels = new Dictionary<Vector3Int, int>();

		int i = 0;
		while (true)
		{
			try
			{
				int x = br.ReadInt32();
				int y = br.ReadInt32();
				int z = br.ReadInt32();
				int v = br.ReadInt32();
				voxels.Add(new Vector3Int(x, y, z), v);
			}
			catch
			{
				break;
			}
			i++;
		}
	}

	void AssignVoxels()
	{
		foreach (var voxel in voxels)
		{
			//Debug.Log("x: " + voxel.Key.x / 4f + ", y: " + voxel.Key.y / 4f + ", z: " + voxel.Key.z / 4f);
			var x = (voxel.Key.x / 4f) + .5f;
			var y = (voxel.Key.y / 4f) + .5f;
			var z = (voxel.Key.z / 4f) + .5f;
			Debug.Log("Floating val: " + x);
			var voxelType = (UInt16) 1;
			try
			{
				this[x, y, z] = voxelType;
			}
			catch (KeyNotFoundException e)
			{
				//var chunkGameObject = new GameObject("Chunk " + (int)(x / 4) + ", " + (int)(y / 4) + ", " + (int)(z / 4));
				//chunkGameObject.transform.position = new Vector3(x, y, z);//transform.parent;
																		  // Add Chunk to GameObject
				//var chunk = chunkGameObject.AddComponent<Chunk>();
				// Add chunk to world at position 0, 0, 0
				try
				{
					var chunk = new Chunk(new Vector3(x, y, z), textureAtlas);
					// _world.Chunks.Add(new ChunkId(x >> 4, y >> 4, z >> 4), chunk);
					this.Chunks.Add(new ChunkId((int)(x / 4), (int)(y / 4), (int)(z / 4)), chunk);
					//_world.Chunks.Add(new ChunkId(x / 16, y /16))
					Debug.Log("Chunk Added");
				}
				catch (ArgumentException ae)
				{
					Debug.Log(ae.Message);
				}
				//chunkGameObject.GetComponent<MeshRenderer>().material = textureAtlas;

				//_world[x, y, z] = voxelType;
			}



			voxelIndex++;
			if (voxelIndex < -100)
			{
				break;
			}
		}
	}

	// Update is called once per frame
	void Update()
	{

	}
}
