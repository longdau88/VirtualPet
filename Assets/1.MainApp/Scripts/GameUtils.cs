using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DG.Tweening;
using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game.Utils
{
	//TUOC
	
    public static class GameUtils
    {
		public static string SpecialCharacters = "?,.!:;'";

		public static void CopyToClipboard(this string copy)
		{
			TextEditor te = new TextEditor();
			te.text = copy;
			te.SelectAll();
			te.Copy();

#if UNITY_ANDROID && !UNITY_EDITOR
			ShowAndroidToastMessage("Sao chép văn bản thành công.");
#else
            ShowPopupCopiedMessage();
#endif
        }

        public static void ShowAndroidToastMessage(string message)
		{
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

			if (unityActivity != null)
			{
				AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
				unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
				{
					AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
					toastObject.Call("show");
				}));
			}
		}

        public static void ShowPopupCopiedMessage()
		{
            var popup = UIPopup.GetPopup("PopupCopied");
            popup.Show();
        }

		public static string Duplicate(string p_str, int p_nTimes)
		{
			string result = "";
			for (int i = 0; i < p_nTimes; i++)
			{
				result += p_str;
			}

			return result;
		}

		public static string DeepTrace(this object p_obj, int p_level = 0)
		{
			string baseStr = Duplicate("\t", p_level);

			if (p_obj == null)
			{
				return "null";
			}

			if (p_obj.GetType().IsPrimitive)
			{
				return p_obj.ToString();
			}

			var list1 = p_obj as IList;
			if (list1 != null)
			{
				IList list = list1;
				string str = baseStr + "[";
				for (int i = 0; i < list.Count; i++)
				{
					str += (i > 0 ? ",\n" : "\n") + baseStr + "\t" + DeepTrace(list[i], p_level + 1);
				}

				return str + "]";
			}

			var dictionary = p_obj as IDictionary;
			if (dictionary != null)
			{
				IDictionary dict = dictionary;
				string str = baseStr + "{";
				bool first = true;

				foreach (DictionaryEntry item in dict)
				{
					str += (first ? "\n" : ",\n") + baseStr + "\t"
						   + (item.Key + ":" + DeepTrace(item.Value, p_level + 1));
					first = false;
				}

				return str + "}";
			}

			return baseStr + p_obj;
		}

		public static GameObject SetCanvas(this GameObject obj, bool isShow)
        {
            if (obj == null) return obj;
            var cv = obj.GetComponent<CanvasGroup>();
            if (cv == null)
                cv = obj.AddComponent<CanvasGroup>();

            if (isShow && !obj.activeSelf)
                obj.SetActive(true);

            cv.alpha = isShow ? 1 : 0;
            cv.blocksRaycasts = isShow;

            return obj;
        }

        public static GameObject SetAlpha(this GameObject obj, float alpha, bool stillClick = true)
        {
            var cv = obj.GetComponent<CanvasGroup>();
            if (cv == null)
                cv = obj.AddComponent<CanvasGroup>();

            if (stillClick && alpha == 0)
                alpha = 0.005f;

            cv.alpha = alpha;
            return obj;
        }

        public static GameObject SetRaycast(this GameObject obj, bool raycast)
        {
            var cv = obj.GetComponent<CanvasGroup>();
            if (cv == null)
                cv = obj.AddComponent<CanvasGroup>();

            cv.blocksRaycasts = raycast;
            return obj;
        }

        public static GameObject SetInteracable(this GameObject obj, bool interacable)
        {
            var cv = obj.GetComponent<CanvasGroup>();
            if (cv == null)
                cv = obj.AddComponent<CanvasGroup>();
            cv.interactable = interacable;

            return obj;
        }

        public static float RandomRange(float min, float max = 100)
        {
            return Random.Range(min, max);
        }

        public static int RandomRange(int min, int max = 100)
        {
            return Random.Range(min, max);
        }

        public static string ReplaceColor(this string txt, string color)
		{
            return txt.Replace("[[", "<color=#" + color + ">").Replace("]]", "</color>");
        }

        public static string ReplaceSpace(this string txt)
        {
            return txt.Replace(" ", "");
        }

        public static Transform ResetTrans(this Transform trans, bool resetPos = true)
        {
            if (resetPos)
                trans.localPosition = Vector3.zero;
            trans.localEulerAngles = Vector3.zero;
            trans.localScale = Vector3.one;
            return trans;
        }

        public static RectTransform ResetTrans(this RectTransform trans)
        {
            trans.anchoredPosition = Vector3.zero;
            trans.localEulerAngles = Vector3.zero;
            trans.localScale = Vector3.one;
            return trans;
        }

        public static void DelayCall(this Transform trans, float timeDelay, Action action)
        {
            TweenControl.GetInstance().DelayCall(trans, timeDelay, () =>
            {
                if (action != null)
                    action();
            });
        }

        public static void DelayCall(this GameObject trans, float timeDelay, Action action)
        {
            TweenControl.GetInstance().DelayCall(trans.transform, timeDelay, () =>
            {
                if (action != null)
                    action();
            });
        }
     
        public static string StringToken(this string txt, string token)
        {
            return txt.Replace("{N}", token);
        }

        public static string ReplaceNewLine(this string txt)
        {
            if (string.IsNullOrEmpty(txt)) return txt;

            return txt.Replace("\\n", "\n");
        }

        public static string ReplaceDoubleSpace(this string txt)
        {
            txt = txt.Replace("  ", " ");
            return txt.Replace("  ", " ");
        }

        /// <summary>
        /// for normal text color only code = FFFFFFFF
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="color"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="replaceNewLine"></param>
        /// <returns></returns>
        public static string SetColorText(this string txt, string color,
            string begin = "[[", string end = "]]", bool replaceNewLine = true)
        {
            txt = txt.Replace(begin, "<color=#" + color + ">");
            txt = txt.Replace(end, "</color>");
            if (replaceNewLine)
                txt = txt.ReplaceNewLine();
            return txt;
        }

        /// <summary>
        /// for TEXDraw color only code = FFFFFFFF
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="color"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="replaceNewLine"></param>
        /// <returns></returns>
        public static string SetColorTEXDraw(this string txt, string color = "blue",
            string begin = "[[", string end = "]]", bool replaceNewLine = true)
        {
            txt = txt.Replace(begin, "\\color[" + color + "]{");
            txt = txt.Replace(end, "}");
            if (replaceNewLine)
                txt = txt.ReplaceNewLine();
            return txt;
        }

        public static string SetColorWithUnderTEXDraw(this string txt, string color = "blue",
        string begin = "[[", string end = "]]", bool replaceNewLine = true)
        {
            txt = txt.Replace(begin, "\\color[" + color + "]\\under ");
            txt = txt.Replace(end, "}");
            if (replaceNewLine)
                txt = txt.ReplaceNewLine();
            return txt;
        }

        public static string ReplaceFraction(this string txt, string[] begin,
            string[] end, string color = "blue", bool setColor = true, bool replaceLine = true)
        {
            var lst = txt.Split(begin, StringSplitOptions.None).ToList();
            var result = "";
            for (int i = 0; i < lst.Count; i++)
            {
                if (lst[i].Contains(end[0]))
                {
                    var lstTemp = lst[i].Split(end, StringSplitOptions.None).ToList();
                    if (lstTemp.Count > 2)
                    {
                        Debug.LogError("Some thing wrong when parse string");
                    }
                    else
                    {
                        result += lstTemp[0].ParseFraction();
                        result += lstTemp[1];
                    }
                }
                else
                {
                    result += lst[i];
                }
            }
            if (setColor)
                txt = SetColorTEXDraw(color, replaceNewLine: replaceLine);
            return result;
        }

        /// <summary>
        /// txt = content to replace
        /// begin = key begin
        /// end = key end
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="actionReplace"></param>
        /// <returns></returns>
        public static string ReplaceSpecialKey(this string txt, string[] begin,
            string[] end, Func<string, string> actionReplace)
        {
            var result = "";

            var lst = txt.Split(begin, StringSplitOptions.None).ToList();
            for (int i = 0; i < lst.Count; i++)
            {
                if (lst[i].Contains(end[0]))
                {
                    var lstTemp = lst[i].Split(end, StringSplitOptions.None).ToList();
                    if (lstTemp.Count > 2)
                    {
                        Debug.LogError("Some thing wrong when parse string");
                    }
                    else
                    {
                        result += actionReplace(lstTemp[0]);
                        result += lstTemp[1];
                    }
                }
                else
                {
                    result += lst[i];
                }
            }
            return result;
        }

        /// <summary>
        /// txt = 2/4/5
        /// 2 = so nguyen
        /// 4 = tu so
        /// 5 = mau so
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string ParseFraction(this string txt)
        {
            txt = txt.ReplaceSpace();
            var lst = txt.Split('/').ToList();
            if (lst.Count == 1) return txt;

            var number = "";
            var numer = lst[0];
            var denomi = lst[1];
            if (lst.Count > 2)
            {
                number = lst[0];
                numer = lst[1];
                denomi = lst[2];
            }
            txt = number + "\\frac{" + numer + "}{" + denomi + "}";
            return txt;
        }

        public static RectTransform ForceUpdateLayout(this RectTransform rt)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
            return rt;
        }

        public static string UpperFirstLetter(this string content)
        {
            if (string.IsNullOrEmpty(content)) return content;

            content = content.First().ToString().ToUpper() + content.Substring(1);
            return content;
        }

        public static string ReplaceAtIndex(this string content, int idChar, char txtReplace)
        {
            if (string.IsNullOrEmpty(content)) return content;

            var lstChar = content.ToCharArray();
            lstChar[idChar] = txtReplace;
            content = new string(lstChar);
            return content;
        }

        public static string ReplaceAtIndex(this string content, int idChar, string txtReplace)
        {
            if (string.IsNullOrEmpty(content)) return content;

            var str1 = content.Substring(0, idChar);
            var str2 = content.Substring(idChar + 1);
            //var lstChar = content.ToCharArray();
            //lstChar[idChar] = txtReplace;
            //content = new string(lstChar);
            content = str1 + txtReplace + str2;
            return content;
        }

        public static string ConvertOperatorToLetter(this string content)
        {
            if (string.IsNullOrEmpty(content)) return content;

            content = content.Replace("-", "minus");
            content = content.Replace("+", "plus");
            content = content.Replace(" = ", " equals ");
            return content;
        }

        public static string ConverNumberToLetter(this string content)
        {
            if (string.IsNullOrEmpty(content)) return content;

            content = content.Replace("10", "ten");
            content = content.Replace("9", "nine");
            content = content.Replace("8", "eight");
            content = content.Replace("7", "seven");
            content = content.Replace("6", "six");
            content = content.Replace("5", "five");
            content = content.Replace("4", "four");
            content = content.Replace("3", "three");
            content = content.Replace("2", "two");
            content = content.Replace("1", "one");
            content = content.Replace("0", "zero");
            return content;
        }

        public static string ConvertToUnSign(this string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        /*public static T GetRandomSingle<T>(this List<T> lstObj)
        {
            //return RandomSingleObject<T>.GetRandomSingle(lstObj);
        }*/

        public static string ReplaceUnderTxt(this string content, string keyReplace = @"\u", bool needAddSpace = false)
        {
            if (needAddSpace)
            {
                content = content.Replace(@"\u", @"\under ");
            }
            else
            {
                content = content.Replace(@"\u", @"\under");
            }
            return content;
        }

        public static string ConvertUnitLength(this string content)
        {
            //temp dung dc thi dung ko thi copy edit
            content = content.Replace("mm", "millimetres");
            content = content.Replace("cm", "centimetres");
            content = content.Replace("dm", "decimetres");
            content = content.Replace(" m ", " metres.");
            return content;
        }
        
        public static string ReplaceMaxTxtToDot(this string content, int maxCount = 15)
        {
            if (content.Length > maxCount)
            {
                content = content.Substring(0, maxCount) + "...";
            }
            return content;
        }

		public static string NumberToWords(int number)
		{
			if (number == 0)
				return "zero";

			if (number < 0)
				return "minus " + NumberToWords(Math.Abs(number));

			string words = "";

			if ((number / 1000000) > 0)
			{
				words += NumberToWords(number / 1000000) + " million ";
				number %= 1000000;
			}

			if ((number / 1000) > 0)
			{
				words += NumberToWords(number / 1000) + " thousand ";
				number %= 1000;
			}

			if ((number / 100) > 0)
			{
				words += NumberToWords(number / 100) + " hundred ";
				number %= 100;
			}

			if (number > 0)
			{
				if (words != "")
					words += "and ";

				var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
				var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

				if (number < 20)
					words += unitsMap[number];
				else
				{
					words += tensMap[number / 10];
					if ((number % 10) > 0)
						words += " " + unitsMap[number % 10];
				}
			}

			return words.ReplaceDoubleSpace();
		}

		public static int ScoreToNumStars(this int score)
		{
			var numStars = (int)Math.Round((float)score / (float)2, 0, MidpointRounding.AwayFromZero);
			return numStars;
		}

		public static void SetRatioImage(Vector2 limitedSize, Image image, Sprite sprite)
		{
			image.rectTransform.sizeDelta = limitedSize;
			var imgRatio = image.GetComponent<AspectRatioFitter>() ? image.GetComponent<AspectRatioFitter>() : image.gameObject.AddComponent<AspectRatioFitter>();
			var ratio = (float)sprite.rect.width / (float)sprite.rect.height;

			if (sprite.rect.width < sprite.rect.height)
			{
				imgRatio.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
				imgRatio.aspectRatio = ratio;
			}
			else
			{
				imgRatio.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
				imgRatio.aspectRatio = ratio;
				imgRatio.aspectRatio = ratio;
			}
		}

		public static int LineCount(this Text txt)
        {
            //1 dong = can giua, > 1 = can trai
            TextGenerationSettings settings = txt.GetGenerationSettings(txt.rectTransform.rect.size);
            TextGenerator generator = new TextGenerator();
            generator.Populate(txt.text, settings);

            return generator.lineCount;
        }

        public static void SetAnchor(this RectTransform source, AnchorPresets allign, float offsetX = 0, float offsetY = 0)
        {

            switch (allign)
            {
                case (AnchorPresets.TopLeft):
                    {
                        source.anchorMin = new Vector2(0, 1);
                        source.anchorMax = new Vector2(0, 1);
                        break;
                    }
                case (AnchorPresets.TopCenter):
                    {
                        source.anchorMin = new Vector2(0.5f, 1);
                        source.anchorMax = new Vector2(0.5f, 1);
                        break;
                    }
                case (AnchorPresets.TopRight):
                    {
                        source.anchorMin = new Vector2(1, 1);
                        source.anchorMax = new Vector2(1, 1);
                        break;
                    }

                case (AnchorPresets.MiddleLeft):
                    {
                        source.anchorMin = new Vector2(0, 0.5f);
                        source.anchorMax = new Vector2(0, 0.5f);
                        break;
                    }
                case (AnchorPresets.MiddleCenter):
                    {
                        source.anchorMin = new Vector2(0.5f, 0.5f);
                        source.anchorMax = new Vector2(0.5f, 0.5f);
                        break;
                    }
                case (AnchorPresets.MiddleRight):
                    {
                        source.anchorMin = new Vector2(1, 0.5f);
                        source.anchorMax = new Vector2(1, 0.5f);
                        break;
                    }

                case (AnchorPresets.BottomLeft):
                    {
                        source.anchorMin = new Vector2(0, 0);
                        source.anchorMax = new Vector2(0, 0);
                        break;
                    }
                case (AnchorPresets.BottonCenter):
                    {
                        source.anchorMin = new Vector2(0.5f, 0);
                        source.anchorMax = new Vector2(0.5f, 0);
                        break;
                    }
                case (AnchorPresets.BottomRight):
                    {
                        source.anchorMin = new Vector2(1, 0);
                        source.anchorMax = new Vector2(1, 0);
                        break;
                    }

                case (AnchorPresets.HorStretchTop):
                    {
                        source.anchorMin = new Vector2(0, 1);
                        source.anchorMax = new Vector2(1, 1);
                        break;
                    }
                case (AnchorPresets.HorStretchMiddle):
                    {
                        source.anchorMin = new Vector2(0, 0.5f);
                        source.anchorMax = new Vector2(1, 0.5f);
                        break;
                    }
                case (AnchorPresets.HorStretchBottom):
                    {
                        source.anchorMin = new Vector2(0, 0);
                        source.anchorMax = new Vector2(1, 0);
                        break;
                    }

                case (AnchorPresets.VertStretchLeft):
                    {
                        source.anchorMin = new Vector2(0, 0);
                        source.anchorMax = new Vector2(0, 1);
                        break;
                    }
                case (AnchorPresets.VertStretchCenter):
                    {
                        source.anchorMin = new Vector2(0.5f, 0);
                        source.anchorMax = new Vector2(0.5f, 1);
                        break;
                    }
                case (AnchorPresets.VertStretchRight):
                    {
                        source.anchorMin = new Vector2(1, 0);
                        source.anchorMax = new Vector2(1, 1);
                        break;
                    }

                case (AnchorPresets.StretchAll):
                    {
                        source.anchorMin = new Vector2(0, 0);
                        source.anchorMax = new Vector2(1, 1);
                        break;
                    }
            }
            source.anchoredPosition = new Vector3(offsetX, offsetY, 0);
        }

        public static void SetPivot(this RectTransform source, PivotPresets allign, float offsetX = 0, float offsetY = 0)
        {

            switch (allign)
            {
                case (PivotPresets.TopLeft):
                    {
                        source.pivot = new Vector2(0, 1);
                        break;
                    }
                case (PivotPresets.TopCenter):
                    {
                        source.pivot = new Vector2(0.5f, 1);
                        break;
                    }
                case (PivotPresets.TopRight):
                    {
                        source.pivot = new Vector2(1, 1);
                        break;
                    }

                case (PivotPresets.MiddleLeft):
                    {
                        source.pivot = new Vector2(0, 0.5f);
                        break;
                    }
                case (PivotPresets.MiddleCenter):
                    {
                        source.pivot = new Vector2(0.5f, 0.5f);
                        break;
                    }
                case (PivotPresets.MiddleRight):
                    {
                        source.pivot = new Vector2(1, 0.5f);
                        break;
                    }

                case (PivotPresets.BottomLeft):
                    {
                        source.pivot = new Vector2(0, 0);
                        break;
                    }
                case (PivotPresets.BottonCenter):
                    {
                        source.pivot = new Vector2(0.5f, 0);
                        break;
                    }
                case (PivotPresets.BottomRight):
                    {
                        source.pivot = new Vector2(1, 0);
                        break;
                    }

                case (PivotPresets.HorStretchTop):
                    {
                        source.pivot = new Vector2(0, 1);
                        break;
                    }
                case (PivotPresets.HorStretchMiddle):
                    {
                        source.pivot = new Vector2(0, 0.5f);
                        break;
                    }
                case (PivotPresets.HorStretchBottom):
                    {
                        source.pivot = new Vector2(0, 0);
                        break;
                    }

                case (PivotPresets.VertStretchLeft):
                    {
                        source.pivot = new Vector2(0, 0);
                        break;
                    }
                case (PivotPresets.VertStretchCenter):
                    {
                        source.pivot = new Vector2(0.5f, 0);
                        break;
                    }
                case (PivotPresets.VertStretchRight):
                    {
                        source.pivot = new Vector2(1, 0);
                        break;
                    }

                case (PivotPresets.StretchAll):
                    {
                        source.pivot = new Vector2(0, 0);
                        break;
                    }
            }
            source.anchoredPosition = new Vector3(offsetX, offsetY, 0);
        }

        public static int ToInt(this object val)
        {
            int ot = 0;
            if (!int.TryParse(val.ToString(), out ot))
                return 0;
            return int.Parse(val.ToString());
        }

        public static bool ToBool(this object val)
        {
            string value = val.ToString().ToLower();
            return value.Equals("true");
        }

        public static string ConvertHtml(this string val)
        {
            return val.ReplaceHtmlTag();
        }

		public static string ReplaceFraction(this string val)
		{
			return val.Replace("/frac", "\frac");
		}

        public static string ReplaceHtmlTag(this string val)
        {
            var matches = Regex.Matches(val, "<.*?>");
            foreach (Match match in matches)
            {
                var rep = match.Groups[0].Captures[0].Value;
                val = val.Replace(rep, "");
            }
            return val;
        }

        public static string RemoveHTMLTagsCharArray(this string html)
        {
            char[] charArray = new char[html.Length];
            int index = 0;
            bool isInside = false;

            for (int i = 0; i < html.Length; i++)
            {
                char left = html[i];

                if (left == '<')
                {
                    isInside = true;
                    continue;
                }

                if (left == '>')
                {
                    isInside = false;
                    continue;
                }

                if (!isInside)
                {
                    charArray[index] = left;
                    index++;
                }
            }

            return new string(charArray, 0, index);
        }

        public static string CheckToDownLine(this string val)
		{
			var maxWord = 14;
			var arrVal = val.Split(' ');
			int count = arrVal.Length;
			if (count > maxWord)
			{
				val = val.Replace(arrVal[maxWord -2] + " " + arrVal[maxWord - 1] + " ", arrVal[maxWord - 2] + " " + arrVal[maxWord - 1] + "\n");
			}
			return val;
		}

        public static string CheckElementQuest(this string val, ref int countRepeat)
        {
            var tempCount = -1;
			var quest = "";
            val = val.Replace("&lt;", "<").Replace("&gt;", ">");
            var matches = Regex.Matches(val, "<.*?>");
			if(matches.Count <= 0)
			{
				quest = val;
			}
            foreach (Match match in matches)
            {
                var rep = match.Groups[0].Captures[0].Value;
                if (rep.Contains("repeat"))
                {
                    var temp = rep.Replace("repeat", "");
                    tempCount = int.Parse(temp.ReplaceSpace().Replace("<", "").Replace(">", ""));
                    quest = val.Replace(rep, "");
                }
				else
				{
					quest = val;
				}
            }
            countRepeat = tempCount;
            return quest;
        }

        public static float SnapTo(this float a, float snap)
        {
            var floor = Mathf.FloorToInt(a);
            var half = a - floor;
            if (half >= 0.5f)
                return floor + 0.5f;
            else
                return floor;
        }

		public static string TranslateMonth(this string month)
		{
			var result = "";

			switch (month)
			{
				case "January":
					result = "Tháng Một";
					break;
				case "February":
					result = "Tháng Hai";
					break;
				case "March":
					result = "Tháng Ba";
					break;
				case "April":
					result = "Tháng Tư";
					break;
				case "May":
					result = "Tháng Năm";
					break;
				case "June":
					result = "Tháng Sáu";
					break;
				case "July":
					result = "Tháng Bảy";
					break;
				case "August":
					result = "Tháng Tám";
					break;
				case "September":
					result = "Tháng Chín";
					break;
				case "October":
					result = "Tháng Mười";
					break;
				case "November":
					result = "Tháng Mười một";
					break;
				case "December":
					result = "Tháng Mười hai";
					break;
			}
			return result;
		}

		public static string ReplaceCharByString(this string content, int index, string replaceBy)
		{
			StringBuilder sb = new StringBuilder(content);
			//sb[index] = replaceBy;
			return content;
		}

		public static double DateTimeSecondToDouble(this DateTime theDate, bool isLocalTime = true)
		{
			DateTime origin = new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
			TimeSpan diff = /*(isLocalTime? theDate : theDate.ToLocalTime())*/ theDate - origin;
			return Math.Floor(diff.TotalSeconds);
		}

        public static double DateTimeToDouble(this DateTime theDate, bool isLocalTime = true)
		{
            DateTime origin = new DateTime(2000, 1, 1);
            TimeSpan diff = /*(isLocalTime? theDate : theDate.ToLocalTime())*/ theDate - origin;
            return Math.Floor(diff.TotalSeconds);
        }

		public static DateTime DoubleToDateTimeSecond(this double Second)
		{
			DateTime origin = new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
			return origin.AddSeconds(Second);
		}

		public static TimeSpan DeltaTimeSpan(this double Second)
		{
			DateTime origin = new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
			var date = origin.AddSeconds(Second);
			return date - origin;
		}

		public static string ReplaceTimeSpan(this string time, bool isVie)
		{
			var arrTime = time.Split('.');

			if (arrTime.Length <= 1) return time;

			if (isVie)
			{
				return time = arrTime[0] + " ngày";
			}
			else
			{
				var day = int.Parse(arrTime[0]);
				if (day == 1)
					return time = arrTime[0] + " day";
				else
					return time = arrTime[0] + " days";
			}
		}

        public static string TimeRemainToNowByDay(this string time, bool isVietnamese)
		{
            var now = Convert.ToDateTime(DateTime.Now.ToShortTimeString()).DateTimeSecondToDouble(false);
            var timeTarget = Convert.ToDateTime(time).DateTimeSecondToDouble(false);

            var deltaTime = timeTarget - now;

            time = deltaTime.DeltaTimeSpan().ToString();

            var arrTime = time.Split('.');

            if (arrTime.Length <= 1)
			{
                arrTime = time.Split(':');
                if (arrTime[0].Contains("-"))
                {
                    return "Đã quá hạn";
                }
                else return "Dưới 1 ngày";
			}

            if (isVietnamese)
            {
                return time ="Còn " + arrTime[0] + " ngày";
            }
            else
            {
                var day = int.Parse(arrTime[0]);
                if (day == 1)
                    return time = arrTime[0] + " day remain";
                else
                    return time = arrTime[0] + " days remain";
            }
        }

		public static string RemoveSpecialCharacters(this string stringCheck)
		{
			for (int i = 0; i < stringCheck.Length; i++)
			{
				var temp = i;
				if (SpecialCharacters.Contains(stringCheck[temp]))
					stringCheck = stringCheck.Replace(stringCheck[temp].ToString(), "");
			}
			return stringCheck;
		}

		public static string ConvertNumberToString(this int number)
		{
			var value = number.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("de"));
			return value;
		}

        public static string ConvertNumberToString(this float number)
		{
            var value = number.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("de"));
            return value;
		}

        public static float DeltaTimeToMinutes(DateTime startTime)
        {
            var length = DateTime.Now - startTime;
            return (float)length.TotalMinutes;
        }
    }


    public enum AnchorPresets
    {
        TopLeft,
        TopCenter,
        TopRight,

        MiddleLeft,
        MiddleCenter,
        MiddleRight,

        BottomLeft,
        BottonCenter,
        BottomRight,
        BottomStretch,

        VertStretchLeft,
        VertStretchRight,
        VertStretchCenter,

        HorStretchTop,
        HorStretchMiddle,
        HorStretchBottom,

        StretchAll
    }

    public enum PivotPresets
    {
        TopLeft,
        TopCenter,
        TopRight,

        MiddleLeft,
        MiddleCenter,
        MiddleRight,

        BottomLeft,
        BottonCenter,
        BottomRight,
        BottomStretch,

        VertStretchLeft,
        VertStretchRight,
        VertStretchCenter,

        HorStretchTop,
        HorStretchMiddle,
        HorStretchBottom,

        StretchAll
    }
}
