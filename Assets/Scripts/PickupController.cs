using UnityEngine;
using System.Collections;

public class PickupController : MonoBehaviour {

	private Vector3 vector = new Vector3();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.vector.Set(0, 0, 45);
		transform.Rotate(this.vector * Time.deltaTime);
	}
}
