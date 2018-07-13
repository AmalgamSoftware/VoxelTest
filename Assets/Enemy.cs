using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct intVector2{
	public int x;
	public int y;
	public intVector2(int x,int y){
		this.x = x;
		this.y = y;
	}
	public void Set(int x, int y){
		this.x = x;
		this.y = y;
	}
}

public class Enemy : MonoBehaviour {

	[HideInInspector]
	public Voxel vox;
	private int phase = 0;
	[HideInInspector]
	public GameObject current, target;
	[HideInInspector]
	public intVector2 currentCoords, targetCoords;
	private Transform tf;
	private Vector3 direction;
	bool successfulSearch = false;
	// Use this for initialization

	void Start () {
		tf = transform;
	}
	
	// Update is called once per frame
	void Update () {
		//PHASE 1
		if (phase == 0) {
			while (successfulSearch == false) {
				int choice = Random.Range (0, 4);
				//Debug.Log (choice);
			
		
				switch (choice) {
				case 0:
					if (vox.grid [currentCoords.x + 1, currentCoords.y].tag != "cube") {
						target = vox.grid [currentCoords.x + 1, currentCoords.y];
						successfulSearch = true;
						phase = 1;
					}
					break;
				case 1:
					if (vox.grid [currentCoords.x - 1, currentCoords.y].tag != "cube") {
						target = vox.grid [currentCoords.x - 1, currentCoords.y];
						successfulSearch = true;
						phase = 1;
					}
					break;
				case 2:
					if (vox.grid [currentCoords.x, currentCoords.y + 1].tag != "cube") {
						target = vox.grid [currentCoords.x, currentCoords.y + 1];
						successfulSearch = true;
						phase = 1;
					}
					break;
				case 3:
					if (vox.grid [currentCoords.x, currentCoords.y -1].tag != "cube") {
						target = vox.grid [currentCoords.x, currentCoords.y -1];
						successfulSearch = true;
						phase = 1;
					}
					break;
				}
			}
				
		} 
		
		//PHASE 2
		if(phase == 1){
			
			direction = (target.transform.position - current.transform.position).normalized;
			phase = 2;
		}
		//PHASE 3
		if (phase == 2){
			
			tf.Translate(direction * 2.2f * Time.deltaTime);
			if (Vector3.Distance(tf.position,target.transform.position) < 0.04f) {
				tf.position = target.transform.position;
				phase = 0;
				successfulSearch = false;
				current = target;
				currentCoords = new intVector2 ((int)tf.position.x, (int)tf.position.z);
				target = null;


			}
		}

	}
}
