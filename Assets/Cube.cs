using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type{Cube,Empty};
public struct CubeData{
	public bool containsPlayer;
	public bool containsGoal;
	public Type type;
	public intVector2 coords;
	public CubeData (Type typ, int coord1, int coord2){
		type = typ;
		coords.x = coord1;
		coords.y = coord2;
		containsPlayer = false;
		containsGoal = false;
	}
}
public class Cube : MonoBehaviour {

	public CubeData cubeData;

	// Use this for initialization
	void Start () {
		if (cubeData.type == Type.Empty) {
			GetComponent<MeshRenderer> ().enabled = false;
			GetComponent<BoxCollider> ().enabled = false;

		}
	}

}
