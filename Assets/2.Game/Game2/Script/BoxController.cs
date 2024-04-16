using UnityEngine;

namespace Game.FlappyEddie
{
	public class BoxController : MonoBehaviour
	{
		[SerializeField] float minPosY;
		[SerializeField] float maxPosY;
		Vector2 startPos;
		float targetXMove;

		RectTransform rect;

		public bool isMoving { get; private set; }

		public void InitBox(Vector2 startPos, Vector2 target)
		{
			this.startPos = startPos;
			targetXMove = target.x;

			rect = GetComponent<RectTransform>();

			rect.anchoredPosition = startPos;
		}

		public void Move(float speedMove)
		{
			ResetBox();

			var posY = Random.Range(minPosY, maxPosY);

			rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, posY);

			var targetMove = new Vector2(targetXMove, posY);

			var distance = Vector2.Distance(rect.anchoredPosition, targetMove);
			var time = (float)distance / speedMove;

			isMoving = true;
			TweenControl.GetInstance().MoveRect(rect, targetMove, time, ()=> { isMoving = false; });
		}

		public void ResetBox()
		{
			isMoving = false;
			rect.anchoredPosition = startPos;
		}
	}
}