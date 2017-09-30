using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject Player;
	private Vector3 offset;

	// Use this for initialization
	void Start () {
		this.offset = transform.position - this.Player.transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.position = this.Player.transform.position + this.offset;
	}
}
