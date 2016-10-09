using System.Collections;
using UnityEngine;

public class UIWheel : MonoBehaviour
{
	public GameObject Wheel;
	public GameObject player;
	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		Wheel.transform.position = player.transform.position;
	}
}
