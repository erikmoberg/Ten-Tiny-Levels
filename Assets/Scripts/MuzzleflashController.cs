using UnityEngine;
using System.Collections;

public class MuzzleflashController : MonoBehaviour {

	public float timeToLiveSeconds = 0.05f;

	// Use this for initialization
	void Start () {
		Destroy (gameObject, this.timeToLiveSeconds);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
