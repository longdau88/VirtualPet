using MainApp;
using UnityEngine.Rendering;

public static class StaticConfig
{
	public static readonly string CREATE_APP = "CreateApp";
	public static readonly string GOLD_PLAYER_APP = "GoldPlayerApp";
	public static readonly string SAVE_DATE_TIME_FULL = "SaveDateTimeFull";
	public static readonly string LAST_SCREEN = "LastScreen";
	public static readonly string VALUE_HUNGRY = "ValueHungry";
	public static readonly string VALUE_DIRTY = "ValueDirty";
	public static readonly string VALUE_A_SLEEP = "ValueAsleep";
	public static readonly string VOLUME_KEY = "Volume";


	public static string FilePetData = "vtfr";



	public const string SCENE_AR = "ARScenes";



	public static string AssetVR(string name)
	{
		return string.Format("assetVR {0}", name);
	}

	public static string AssetAR(string name)
	{
		return string.Format("assetAR {0}", name);
	}

	public static string AssetGame(string name)
	{
		return string.Format("assetGame {0}", name);
	}

	public static string TopicFileNameLocal(string gradeId)
	{
		return string.Format("ListTopic {0}{1}", gradeId, (LanguageController.Instance.IsVietNamese ? "Vie" : "Eng"));
	}

	public static string SkillFileNameLocal(string topicId)
	{
		return string.Format("Skill {0}{1}", topicId, (LanguageController.Instance.IsVietNamese ? "Vie" : "Eng"));
	}

	public static string Key_opponent_arena(int userId)
	{
		return string.Format("{0} {1}", userId, System.DateTime.Today.Date.ToShortDateString());
	}

}
