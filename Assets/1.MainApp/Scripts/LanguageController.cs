using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainApp
{
    public class LanguageController : MonoBehaviour
    {
        public static LanguageController Instance { get; private set; }

		public LanguageType _languageType { get; private set; }


        public int GetIdLanguage()
        {
            return (int)_languageType;
        }

		public bool IsVietNamese
		{
			get
			{
				return _languageType == LanguageType.VN;
			}
		}

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
			else
			{
				Destroy(gameObject);
				return;
			}
        }
	}


    public enum LanguageType
    {
		VN = 1,
		EN = 0,
    }
}


