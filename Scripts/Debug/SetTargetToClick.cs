using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTargetToClick : MonoBehaviour
{
	public LayerMask mask;

	Agent[] ais;

	Camera cam;


	public void Start()
	{
		cam = Camera.main;
		ais = FindObjectsOfType<Agent>();
		useGUILayout = false;
	}


	/// <summary>Update is called once per frame</summary>
	void Update()
	{
		if (Input.GetMouseButtonDown(0) && cam != null)
		{
			UpdateTargetPosition();
		}
	}


	public void UpdateTargetPosition()
	{
		Vector3 newPosition = Vector3.zero;
		bool positionFound = false;

		// Fire a ray through the scene at the mouse position and place the target where it hits
		RaycastHit hit;
		if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, mask))
		{
			newPosition = hit.point;
			positionFound = true;
		}

		if (positionFound)
		{
			print($"Hit Position {newPosition}");
			for (int i = 0; i < ais.Length; i++)
			{
				if (ais[i] != null) ais[i].MoveTo(newPosition);
			}
		}
	}
}
