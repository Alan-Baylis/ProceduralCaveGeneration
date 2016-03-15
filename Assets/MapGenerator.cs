using UnityEngine;
using System.Collections;
using System;

public class MapGenerator : MonoBehaviour
{
	//How many iterations of the smoothing function will run?
	public int smoothingIterations;

	public int width;
	public int height;

	public string seed;
	public bool useRandomSeed;

	[Range(0,100)]
	public int fillCriteria;

	int[,] map;

	void Start()
	{
		GenerateMap();
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			GenerateMap();
		}
	}

	void GenerateMap()
	{
		map = new int[width, height];
		RandomFillMap();

		for(int i = 0; i < smoothingIterations; i++)
		{
			SmoothMap();
		}

		MeshGenerator meshGen = GetComponent<MeshGenerator>();
		meshGen.GenerateMesh(map, 1);
	}

	void RandomFillMap()
	{
		if (useRandomSeed)
		{
			//Random value for ensuring a 'random' appearance of the terrain
			seed = DateTime.Now.Ticks.ToString();
		}

		//Use a HashCode for a reocurring, but 'random' look 
		System.Random pseudoRandom = new System.Random(seed.GetHashCode());

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				//At the edges of the terrain? If so, add a wall
				if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
				{
					map[x, y] = 1;
				}
				else
				{
					//Generate a random number, if it's over the specified random cap, add a wall
					map[x, y] = (pseudoRandom.Next(0, 100) < fillCriteria) ? 1 : 0;
				}
			}
		}
	}

	void SmoothMap()
	{
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				//How many cubes are surrounding this cube?
				int neighbourWallTiles = GetSurroundingWallCount(x, y);

				if (neighbourWallTiles > 4)
				{
					map[x, y] = 1;
				}
				else if (neighbourWallTiles < 4)
				{
					map[x, y] = 0;
				}
			}
		}
	}

	int GetSurroundingWallCount(int gridX, int gridY)
	{
		int wallCount = 0;
		//Scan all of the surrounding neighbours to the current cube
		for(int neighbourX = gridX -1; neighbourX <= gridX + 1; neighbourX++)
		{
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
			{
				//Ensure we aren't scanning off the terrain's bounds (0 - width-1 and 0 - height-1)
				if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
				{
					//Don't scan the cube against itself
					if (neighbourX != gridX || neighbourY != gridY)
					{
						wallCount += map[neighbourX, neighbourY];
					}
				}
				else
				{
					wallCount++;
				}
			}
		}
		return wallCount;
	}

	//void OnDrawGizmos()
	//{
	//	Vector3 cubeSize = Vector3.one;
	//	if (map != null)
	//	{
	//		for (int x = 0; x < width; x++)
	//		{
	//			for (int y = 0; y < height; y++)
	//			{
	//				Gizmos.color = (map[x, y] == 1) ? Color.black : Color.white;
	//				Vector3 pos = new Vector3(-width / 2 + x, 0, -height/2 + y);
	//				Gizmos.DrawCube(pos,cubeSize);
	//			}
	//		}
	//	}
	//}
}