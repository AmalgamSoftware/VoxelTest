using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

	public Gradient grad;
	public Material mat;
	private int counter = 0;
	private int counterRate = 1;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		counter += counterRate;
		if (counter > 100) {
			counter = 0;
		}
		mat.color = grad.Evaluate (counter / 100.0f);
	}
}
