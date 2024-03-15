using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

namespace MainApp.VirtualFriend
{
	public class PopupPermissionAR : MonoBehaviour
	{
		[SerializeField] Text txtContent;
		[SerializeField] Button btnConfirm;
		[SerializeField] Text txtAgree;


		public void InitPopup(System.Action onConfirm)
		{
			txtContent.text = LabelLanguage.ContentPermissionAR[LanguageController.Instance.GetIdLanguage()];
			txtAgree.text = LabelLanguage.Continue[LanguageController.Instance.GetIdLanguage()];

			btnConfirm.onClick.RemoveAllListeners();
			btnConfirm.onClick.AddListener(() =>
			{
				VirtualPetManager.Instance.HidePopupPermissionAR();

#if UNITY_IOS
				Application.RequestUserAuthorization(UserAuthorization.WebCam);
#endif

#if UNITY_ANDROID
				Permission.RequestUserPermission(Permission.Camera);
#endif
				onConfirm?.Invoke();
			});
		}
	}
}
