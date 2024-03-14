using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MainApp.VirtualFriend
{
	public class SoapController : MonoBehaviour
	{
		[SerializeField] float startTimeBtwSpawns;
		[SerializeField] Camera cameraVirtualFriend;
		[Space]
		[SerializeField] int maxBubblesInit;
		[SerializeField] Transform bubbleContain;
		[SerializeField] GameObject bubblePref;
		bool isWashing;

		List<GameObject> lstBubbles;
		Vector3 startPos;
		Vector3 bubbleScale;
		float distanceFromCmr;
		float timeBtwSpawns;

		int countBubbles;

		public System.Action onWashDone;
		public System.Action onStartWash;
		bool canWash;

		void Start()
		{
			startPos = transform.position;
			
			distanceFromCmr = Vector3.Distance(transform.position, cameraVirtualFriend.transform.position);
			InitBubbles();
		}

		private void InitBubbles()
		{
			lstBubbles = new List<GameObject>();
			bubbleScale = bubblePref.transform.localScale;

			for (int i = 0; i < maxBubblesInit; i++)
			{
				var bubble = Instantiate(bubblePref, bubbleContain);
				bubble.transform.localEulerAngles = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
				bubble.transform.localScale = Vector3.zero;

				lstBubbles.Add(bubble);
			}

			bubbleContain.gameObject.SetActive(false);
		}

		private void Update()
		{
			if (Input.GetMouseButton(0))
			{
				MoveSoapToWash();
			}
		}

		private void MoveSoapToWash()
		{
			var pos = Input.mousePosition;
			pos.z = distanceFromCmr;

			if (canWash)
			{
				transform.position = cameraVirtualFriend.ScreenToWorldPoint(pos);
				if (timeBtwSpawns <= 0 && countBubbles < maxBubblesInit)
				{
					var bubble = lstBubbles[countBubbles];

					RaycastHit hit;
					Ray ray = cameraVirtualFriend.ScreenPointToRay(pos);

					if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 9))
					{
						if (hit.collider.gameObject.GetComponent<MyPetController>())
						{
							bubble.transform.position = hit.point;
							TweenControl.GetInstance().Scale(bubble, bubbleScale, 0.25f);

							timeBtwSpawns = startTimeBtwSpawns;
							countBubbles++;
						}
					}
				}
				else
				{
					timeBtwSpawns -= Time.deltaTime;
				}
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.GetComponent<MyPetController>() == null) return;
			isWashing = true;
			onStartWash?.Invoke();
		}

		private void OnMouseDown()
		{
			canWash = true;
			bubbleContain.gameObject.SetActive(true);
		}

		private void OnMouseUp()
		{
			canWash = false;

			var count = countBubbles;
			countBubbles = 0;

			TweenControl.GetInstance().Move(transform, startPos, 0.4f, ()=>
			{
				if (!isWashing) return;
				isWashing = false;

				TweenControl.GetInstance().DelayCall(this.transform, 1, () =>
				{
					for (int i = 0; i < count + 1; i++)
					{
						var bubble = lstBubbles[i];
						var temp = i;
						TweenControl.GetInstance().Scale(bubble, Vector3.zero, 0.25f, ()=>
						{
							if (temp == count) bubbleContain.gameObject.SetActive(false);
						}, EaseType.Linear, temp * 0.02f);
					}
				});
				onWashDone?.Invoke();
			});
		}
	}
}
