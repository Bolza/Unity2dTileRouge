using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

/**	
 * Generates a tiled level based on level number
 */

public class BoardManager : MonoBehaviour {

	[Serializable]
	public class Count {
		public int maximum;
		public int minimum;
		public Count(int min, int max) {
			minimum = min;
			maximum = max;
		}
	};

	public int columns = 8;
	public int rows = 8;
	public Count wallCount = new Count (5, 9);
	public Count foodCount = new Count (1, 5);
	public GameObject exit;
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] outerWallTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;

	private Transform boardHolder;
	private List<Vector3> gridPositions = new List<Vector3>();

	// Use this for initialization
	void InitList () {
		gridPositions.Clear ();
		for (int x = 1; x < columns-1; x++) {
			for (int y = 1; y < rows-1; y++) {
				gridPositions.Add (new Vector3 (x, y, 0f));
			}
		}
	}

	void BoardSetup() {
		boardHolder = new GameObject ("Board").transform;
		for (int x = -1; x < columns+1; x++) {
			for (int y = -1; y < rows+1; y++) {
				GameObject toInst = floorTiles [Random.Range (0, floorTiles.Length)];
				if (x < 0 || x == columns || y < 0 || y == rows) {
					toInst = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
				}

				GameObject instance = Instantiate (toInst, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
				instance.transform.SetParent (boardHolder);
			}
		}
	}

	Vector3 RandomPosition() {
		int randomIndex = Random.Range (0, gridPositions.Count);
		Vector3 randomPosition = gridPositions [randomIndex];
		gridPositions.RemoveAt (randomIndex);
		return randomPosition;
	}

	void LayoutObjects(GameObject[] tileArray, int min, int max) {
		int objectCount = Random.Range (min, max + 1);
		for (int i = 0; i < objectCount; i++) {
			Vector3 pos = RandomPosition ();
			GameObject tile = tileArray [Random.Range (0, tileArray.Length)];
			Instantiate (tile, pos, Quaternion.identity);
//			instance.transform.SetParent (boardHolder);
		}
	}

	public void SetupScene(int level) {
		BoardSetup ();
		InitList ();
		LayoutObjects (wallTiles, wallCount.minimum, wallCount.maximum);
		LayoutObjects (foodTiles, foodCount.minimum, foodCount.maximum);
		int enemyCount = (int)Mathf.Log (level, 2f);
		LayoutObjects (enemyTiles, enemyCount, enemyCount);
		Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
