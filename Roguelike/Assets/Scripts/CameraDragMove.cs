using UnityEngine;
using System.Collections;

public class CameraDragMove : MonoBehaviour
{
	public GameObject poi; // The point of interest
	public float easing = 0.05f;

	private Vector3 ResetCamera;
	private Vector3 Origin;
	private Vector3 Diference;
	private Vector3 destination;

	private bool Drag = false;

	void Start()
	{
		ResetCamera = Camera.main.transform.position;
	}
	void LateUpdate()
	{
		if (Input.GetMouseButton(0))
		{
			Diference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
			if (Drag == false)
			{
				Drag = true;
				Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			}
		}
		else
		{
			FollowPointOfInterest();
			Drag = false;
		}
		if (Drag == true)
		{
			Camera.main.transform.position = Origin - Diference;
		}
	}

	private void FollowPointOfInterest()
	{
		destination = poi.transform.position;

		
		// Interpolate from the current Camera position toward destination
		destination = Vector3.Lerp(transform.position, destination, easing);
		// Retain a destination.z of camZ
		destination.z = Camera.main.transform.position.z;
		// Set the camera to the destination
		transform.position = destination;
	}
}