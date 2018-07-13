using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour {

	[HideInInspector]
	public GameObject[,] grid = new GameObject[20,20];
	public GameObject cube;
	public GameObject empty;
	private List<GameObject> enemies = new List<GameObject>();
	public GameObject enemyPrefab;
	public GameObject playerPrefab;
	public GameObject goalPrefab;
	[HideInInspector]
	public GameObject player;
	[HideInInspector]
	public intVector2 playerCoords;
	private bool playerSpawned = false;
	[HideInInspector]
	public GameObject goal;
	[HideInInspector]
	public intVector2 goalCoords;
	private bool goalSpawned = false;
	public Transform terrain;
	[HideInInspector]
	public float totalEnemies = 2;
	private float enemyCount;
	private intVector2 pathStart;
	private bool pathComplete = false;
	private List<Cube> emptyList = new List<Cube>();
	private int emptyCount = 0;
	// Use this for initialization
	void Start () {
		StartLevel ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void MakeCube(int i, int j){
		GameObject cubeInst = (GameObject)Instantiate (cube, new Vector3 (i + 0.5f, 0.5f, j + 0.5f), Quaternion.identity, terrain);
		grid [i, j] = cubeInst;
		Cube cubeScript = cubeInst.GetComponent<Cube> ();
		cubeScript.cubeData = new CubeData(Type.Cube, i, j);
	}
	void MakeEmpty(int i, int j){
		GameObject emptyInst = (GameObject)Instantiate (cube, new Vector3 (i + 0.5f, 0.5f, j + 0.5f), Quaternion.identity,terrain);
		grid [i, j] = emptyInst;
		Cube cubeScript = emptyInst.GetComponent<Cube> ();
		cubeScript.cubeData = new CubeData(Type.Empty, i, j);
		emptyInst.transform.tag = "Untagged";
		emptyCount++;
	}
	void StartLevel(){
		//Primary iteration: Setting up blocks
		enemyCount = totalEnemies;
		for (int i = 0; i < grid.GetLength(0); i++) {
			for (int j = 0; j < grid.GetLength(1); j++) {
				float randy = Random.Range (0f, 10f);
				if (j == 0 || j == 19 || i == 0 || i == 19) {
					MakeCube (i, j);

				} else if (randy > 7f) {

					MakeCube (i, j);

				} else {
					MakeEmpty (i, j);
				}
			}
		}
		//Second iteration: Filling Holes
		for (int i = 0; i < grid.GetLength (0); i++) {
			for (int j = 0; j < grid.GetLength (1); j++) {
				if (grid[i,j].tag != "cube") {
					if (grid [i + 1, j].tag != "cube") {
					} else if (grid [i-1, j].tag != "cube") {
					} else if (grid [i, j+1].tag != "cube") {
					}else if (grid [i, j-1].tag != "cube") {
					}else {
						GameObject.Destroy (grid [i, j]);
						MakeCube (i, j);
					}

				}
			}
		}

		//Fourth iteration: spawning player
		for (int i = 0; i < grid.GetLength (0); i++) {
			for (int j = 0; j < grid.GetLength (1); j++) {
				if (grid [i, j].tag != "cube") {
					if (playerSpawned == false) {
						GameObject plr = (GameObject)Instantiate (playerPrefab, grid [i, j].transform.position, Quaternion.identity);
						player = plr;
						playerSpawned = true;
						plr.GetComponent<Player> ().vox = this;
						grid [i, j].GetComponent<Cube> ().cubeData.containsPlayer = true;
						playerCoords.Set (i, j);
						break;

					}
				}
			}
		}
		//Fifth iteration: spawning goal
		for (int i = grid.GetLength(0) - 1; i > -1; i--) {
			for (int j = grid.GetLength(1) - 1; j > -1; j--) {
				if (grid [i, j].tag != "cube") {
					if (goalSpawned == false) {
						GameObject gl = (GameObject)Instantiate (goalPrefab, grid [i, j].transform.position, Quaternion.identity);
						goal = gl;
						goalSpawned = true;
						Cube goalCube = grid [i, j].GetComponent<Cube> ();
						goalCube.cubeData.containsGoal = true;
						goalCoords.Set (i, j);
						pathStart.Set (i, j);
						emptyList.Add (goalCube);
						break;

					}
				}
			}
		}
		//Iteration 5: ensure path to goal;

		while (pathComplete == false) {
			emptyList.Clear ();
			RadiateFromGoal (goalCoords.x,goalCoords.y);
			if (pathComplete == false) {
				bool segmentCarved = false;
				while (segmentCarved == false) {
					int leftOrDown;
					if (pathStart.x == playerCoords.x) {
						leftOrDown = 1;
					} else if (pathStart.y == playerCoords.y) {
						leftOrDown = 0;
					} else {
						leftOrDown = Random.Range (0, 2);
					}
					if (leftOrDown == 0) {
						if (grid [pathStart.x - 1, pathStart.y].gameObject.tag == "cube") {
							GameObject.Destroy (grid [pathStart.x - 1, pathStart.y]);
							MakeEmpty (pathStart.x - 1, pathStart.y);

							pathStart.Set (pathStart.x - 1, pathStart.y);
							segmentCarved = true;
							RadiateFromGoal (pathStart.x, pathStart.y);
						} else {
							pathStart.Set (pathStart.x - 1, pathStart.y);
						}

					} else if (leftOrDown == 1) {
						if (grid [pathStart.x, pathStart.y - 1].gameObject.tag == "cube") {
							GameObject.Destroy (grid [pathStart.x, pathStart.y - 1]);
							MakeEmpty (pathStart.x, pathStart.y - 1);
							pathStart.Set (pathStart.x, pathStart.y - 1);
							segmentCarved = true;
							RadiateFromGoal (pathStart.x, pathStart.y);
						} else {
							pathStart.Set (pathStart.x, pathStart.y - 1);
						}

					}
				}
			}
		}

		//Third iteration: Spawning enemies
		while(enemyCount > 0){
			for (int i = 0; i < grid.GetLength (0); i++) {
				for (int j = 0; j < grid.GetLength (1); j++) {
					if (grid [i, j].tag != "cube") {
						float randy = Random.Range (0f, 10f);
						if (enemyCount > 0) {
							if (randy > 9.5f) {
								GameObject enemy = (GameObject)Instantiate (enemyPrefab, grid [i, j].transform.position, Quaternion.identity);
								Enemy enemyScript = enemy.GetComponent<Enemy> ();
								enemyScript.vox = this;
								enemyScript.current = grid [i, j];
								enemyScript.currentCoords = new intVector2 (i, j);
								enemyCount -= 1;
								enemies.Add (enemy);
							}
						}
					}
				}
			}
		}
	}
	private bool RadiateFromGoal(int i, int j){
		int x = i;
		int y = j;
		if (grid [x + 1, y].tag != "cube") {
			Cube nextCube = grid [x + 1, y].GetComponent<Cube> ();
			if (!emptyList.Contains (nextCube)) {
				emptyList.Add (nextCube);
				if (nextCube.cubeData.containsPlayer) {
					pathComplete = true;
					return true;
				} else {
					RadiateFromGoal (nextCube.cubeData.coords.x, nextCube.cubeData.coords.y);
				}
			}
		}
		if (grid [x - 1, y].tag != "cube") {
			Cube nextCube = grid [x - 1, y].GetComponent<Cube> ();
			if (!emptyList.Contains (nextCube)) {
				emptyList.Add (nextCube);
				if (nextCube.cubeData.containsPlayer) {
					pathComplete = true;
					return true;
				}else {
					RadiateFromGoal (nextCube.cubeData.coords.x, nextCube.cubeData.coords.y);
				}
			}
		}
		if (grid [x, y + 1].tag != "cube") {
			Cube nextCube = grid [x, y + 1].GetComponent<Cube> ();
			if (!emptyList.Contains (nextCube)) {
				emptyList.Add (nextCube);
				if (nextCube.cubeData.containsPlayer) {
					pathComplete = true;
					return true;
				}else {
					RadiateFromGoal (nextCube.cubeData.coords.x, nextCube.cubeData.coords.y);
				}
			}
		}
		if (grid [x, y - 1].tag != "cube") {
			Cube nextCube = grid [x, y - 1].GetComponent<Cube> ();
			if (!emptyList.Contains (nextCube)) {
				emptyList.Add (nextCube);
				if (nextCube.cubeData.containsPlayer) {
					pathComplete = true;
					return true;
				}else {
					RadiateFromGoal (nextCube.cubeData.coords.x, nextCube.cubeData.coords.y);
				}
			}
		}
		return false;
	}
	public void RoundWon (){
		ClearAll ();
		totalEnemies += 1;
		StartLevel ();
	}
	public void RoundLost(){
		ClearAll ();
		StartLevel ();
	}
	private void ClearAll (){
		ClearTiles ();
		Destroy (player);
		playerSpawned = false;
		Destroy (goal);
		goalSpawned = false;
		pathComplete = false;
		foreach (GameObject obj in enemies) {
			Destroy (obj);
		}
		enemyCount = 5;
	}
	private void ClearTiles(){
		for (int i = 0; i < grid.GetLength (0); i++) {
			for (int j = 0; j < grid.GetLength (1); j++) {
				if (grid [i, j] != null) {
					Destroy (grid [i, j]);
					grid [i, j] = null;
				}
			}
		}

	}
}
