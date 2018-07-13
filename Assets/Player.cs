using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	private Rigidbody rb;
	public float speed;
	[HideInInspector]
	public Voxel vox;
	private char priorityMove = '0';
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.A)) {
			priorityMove = 'a';
		} else if (Input.GetKey (KeyCode.D)) {
			priorityMove = 'd';
		} else if (Input.GetKey (KeyCode.S)) {
			priorityMove = 's';
		} else if (Input.GetKey (KeyCode.W)) {
			priorityMove = 'w';
		} else {
			priorityMove = '0';
		}
		switch (priorityMove) {
		case 'a':
			rb.velocity = Vector3.left * speed;
			break;
		case 'd':
			rb.velocity = Vector3.right * speed;
			break;
		case 's':
			rb.velocity = Vector3.back * speed;
			break;
		case 'w':
			rb.velocity = Vector3.forward * speed;
			break;
		case '0':
			rb.velocity = Vector3.zero;
			break;
		}
	}
	void OnCollisionEnter(Collision other){
		if (other.collider.tag == "Enemy") {
			vox.RoundLost ();

		}
		if (other.collider.tag == "Goal") {
			vox.RoundWon ();

		}
	}
}

