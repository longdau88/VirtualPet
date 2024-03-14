using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game.Utils;
using Game;

namespace MainApp.VirtualFriend
{
	public class MyPetController : MonoBehaviour
	{
		[SerializeField] VirtualPetManager manager;
		[SerializeField] Animator anim;
		[Space]
		[SerializeField] GameObject brush;
		[SerializeField] float timeBreakTeeth;
		[Header("Sound")]
		[SerializeField] AudioClip soundEat;
		[SerializeField] AudioClip soundBeAttaked;
		[SerializeField] AudioClip soundHearing;
		[SerializeField] AudioClip soundHungry;
		[SerializeField] AudioClip soundAsleep;
		[SerializeField] AudioClip soundRefuse;
		[SerializeField] AudioClip soundSleep;

		BoxCollider thisCollider;
		MyPetData data;

		int countBeAttack;
		float deltaTimeBeAttack;
		bool isBeAttacked;
		bool canAttack = true;

		void Start()
		{
			thisCollider = GetComponent<BoxCollider>();
			if (brush != null)
				brush.SetActive(false);
		}

		public void SetIdle()
		{
			anim.SetBool("isNormal", true);
		}

		public void SetDirty(bool isTrue)
		{
			anim.SetBool("isDirty", isTrue);
		}

		public void SetHungry(bool isTrue)
		{
			anim.SetBool("isHungry", isTrue);
		}

		public void SetEat()
		{
			manager.SetFreeToAttack(false);
            GameAudio.Instance.PlayClip(SourceType.SOUND_FX, soundEat, false, () =>
            {
                manager.SetFreeToAttack(true);
            });
            anim.SetTrigger("eat");
		}

		public void SetBreakTeeth()
		{
			brush.SetActive(true);
			TweenControl.GetInstance().DelayCall(this.transform, timeBreakTeeth, () =>
			{
				brush.SetActive(false);
			});
			anim.SetTrigger("breakteeth");
		}

		public void SetAsleep(bool isTrue)
		{
			if (isTrue)
			{
				manager.SetFreeToAttack(false);
				manager.SetFreeToRepeat(false);

				GameAudio.Instance.PlayClip(SourceType.SOUND_FX, soundAsleep, true, () =>
				{
					manager.SetFreeToAttack(true);
					manager.SetFreeToRepeat(true);
				});
			}
			
			anim.SetBool("isAsleep", isTrue);
		}

		public void SetGotoSleep()
		{
			anim.SetTrigger("sleep");
		}

		public void PlaySoundSleep()
		{
			GameAudio.Instance.PlayClip(SourceType.SOUND_FX, soundSleep, false);
		}

		public void SetRefuse()
		{
			manager.SetFreeToAttack(false);
			manager.SetFreeToRepeat(false);

            GameAudio.Instance.PlayClip(SourceType.SOUND_FX, soundRefuse, true, () =>
            {
                manager.SetFreeToAttack(true);
                manager.SetFreeToRepeat(true);
            });
            anim.SetTrigger("refuse");
		}

		public void SetAnimBeAttacked()
		{
			GameAudio.Instance.PlayClip(SourceType.SOUND_FX, soundBeAttaked, true);
			anim.SetTrigger("attack");
		}

		public void SetBoolAngry(bool isTrue)
		{
			anim.SetBool("isAngry", isTrue);
		}

		public void SetAnimHearing()
		{
			anim.SetTrigger("hear");
		}

		public void SetAnimTalk()
		{
			anim.SetTrigger("talk");
		}

		public void SetRelax()
		{
			anim.SetTrigger("relax");
		}

		private void OnMouseDown()
		{
			if (manager.isFreeToAttack && !VirtualPetManager.Instance.isShowPopup)
			{
				canAttack = false;
				isBeAttacked = true;
				manager.SetFreeToRepeat(false);
				countBeAttack++;
				SetAnimBeAttacked();
			}
		}

		private void OnMouseUp()
		{
			if (!isBeAttacked) return;
			isBeAttacked = false;

			if (OnAttackDone != null) StopCoroutine(OnAttackDone);
			OnAttackDone = StartCoroutine(IOnAttackDone());
		}

		Coroutine OnAttackDone;
		IEnumerator IOnAttackDone()
		{
			yield return new WaitForSeconds(1.5f);

			if (!isBeAttacked)
			{
				if (countBeAttack >= 2) SetBoolAngry(true);
				else
				{
					SetBoolAngry(false);
				}
				countBeAttack = 0;

				yield return new WaitForSeconds(1.5f);

				if (!isBeAttacked) manager.SetFreeToRepeat(true);

			}
		}
	}
}