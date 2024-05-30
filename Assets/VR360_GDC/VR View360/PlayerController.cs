using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	Animation anim;
	bool isPlaying = true;
	float timeDown;
	bool firstClick = true;

	AudioClip voice;

	void Start()
	{
		if (GetComponent<Animation>()) anim = GetComponent<Animation>();
		if (!GetComponent<BoxCollider>()) gameObject.AddComponent<BoxCollider>();
		isPlaying = true;
		
	}

	private void CheckClickObject()
	{
		var hit = new RaycastHit();
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit))
		{
			if (hit.transform == transform)
			{
				if (isPlaying)
				{
					isPlaying = false;

					//ShowFAQ();
				}
			}
		}
	}

	private void OnMouseDown()
	{
		timeDown = Time.time;
	}

	private void OnMouseUp()
	{
		if (Time.time - timeDown < 0.4f)
		{
			CheckClickObject();
		}
	}
}
