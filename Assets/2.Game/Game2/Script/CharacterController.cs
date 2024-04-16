using UnityEngine;

namespace Game.FlappyEddie
{
	public class CharacterController : MonoBehaviour
	{
		public System.Action OnDead;
		public System.Action OnCollect;

		[SerializeField] float speedJump;
		[SerializeField] SpineControl animation;

		[Space]
		[SerializeField] AudioClip soundJump;
		[SerializeField] AudioClip soundCollect;
		[SerializeField] AudioClip soundTrigger;
		[SerializeField] AudioClip soundDied;

		[Space]
		[SerializeField] string ANIM_FLY = "Fly";
		[SerializeField] string ANIM_JUMP = "Tad-Eddie Flappy";
		[SerializeField] string ANIM_DEAD = "Died";
		[SerializeField] float timeAnimJump;

		bool isDead;

		Rigidbody2D rb;
		Vector2 startPos;
		Vector2 startRotation;

		// Start is called before the first frame update
		void Start()
		{
			rb = GetComponent<Rigidbody2D>();
			startPos = transform.position;
			startRotation = transform.localEulerAngles;

			SetAnimFly();
		}

		public void SetJump()
		{
			rb.velocity = new Vector2(0, speedJump);
			SetAnimJump();

			GameAudio.Instance.PlayClip(SourceType.SOUND_FX, soundJump, false);

		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (isDead) return;

			var item = collision.gameObject.GetComponent<ItemTrigger>();
			if (item == null) return;

			if (item.ItemType == ItemType.Dead)
			{
				isDead = true;
				Debug.Log("Dead");

				SetAnimDead();

				GameAudio.Instance.PlayClip(SourceType.SOUND_FX, soundTrigger);

				OnDead?.Invoke();
			}
			else if (item.ItemType == ItemType.Coin)
			{
				GameAudio.Instance.PlayClip(SourceType.SOUND_FX, soundCollect, false);
				OnCollect?.Invoke();
			}
		}

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (isDead) return;

			var item = collision.gameObject.GetComponent<ItemTrigger>();
			if (item == null) return;

			if (item.ItemType == ItemType.Dead)
			{
				isDead = true;
				Debug.Log("Dead");

				SetAnimDead();

				GameAudio.Instance.PlayClip(SourceType.SOUND_FX, soundTrigger);

				OnDead?.Invoke();
			}
		}

		public void PlaySoundDead(System.Action onDone)
		{
			GameAudio.Instance.PlayClip(SourceType.SOUND_FX, soundDied, true, () =>
			{
				onDone?.Invoke();
			});
		}

		private void SetAnimJump()
		{
			TweenControl.GetInstance().KillDelayCallNew(this.transform);
			animation.SetAnimation(ANIM_JUMP, false);

			TweenControl.GetInstance().DelayCallNew(this.transform, timeAnimJump, () =>
			{
				SetAnimFly();
			});
		}

		private void SetAnimFly()
		{
			animation.SetAnimation(ANIM_FLY, true);
		}

		private void SetAnimDead()
		{
			animation.SetAnimation(ANIM_DEAD, false);
		}

		public void ResetCharacter()
		{
			isDead = false;
			transform.localEulerAngles = startRotation;
			transform.position = startPos;
			SetAnimFly();
		}

		public void SetPhysics(bool isPhysics)
		{
			if (rb == null) rb = GetComponent<Rigidbody2D>();
			rb.bodyType = isPhysics ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
		}
	}
}
