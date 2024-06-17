using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveAndLoad<T>
{
	#region text file

	public static void SaveJson(T obj, string fileName, ObjectSaveType type = ObjectSaveType.txt, string extendPath = "")
    {
		string path = FilePath();
		fileName = fileName.Replace(".", "");
		fileName = fileName.Replace("/", "");

		var filePath = Path.Combine(path, fileName + NameExtend(type));

		if (!CheckFilePathExits(filePath))
		{
			var thisFile = File.Create(filePath);
			thisFile.Close();
		}

		File.WriteAllText(filePath, JsonUtility.ToJson(obj as object));
    }

	public static T LoadFileTextNoExpired(string fileName)
	{
		string path = PathDataNoExpired;

		fileName = fileName.Replace(".", "");
		fileName = fileName.Replace("/", "");


		var filePath = Path.Combine(path, fileName + NameExtend(ObjectSaveType.txt));
		var checkFile = CheckFilePathExits(filePath);
		return JsonUtility.FromJson<T>(checkFile ? File.ReadAllText(filePath) : "");
	}

    #endregion

    public static bool CheckFilePathExits(string path)
    {
        return File.Exists(path);
    }

    public static bool CheckPathExits(string extendPath = "")
    {
		var pathData = Path.Combine(Application.persistentDataPath, "data", extendPath);
		if (!Directory.Exists(pathData))
        {
			DirectoryInfo directory = new DirectoryInfo(pathData);
			Directory.CreateDirectory(pathData);
		}

        return true;
    }


    public static bool CheckFileExits(string fileName, ObjectSaveType type = ObjectSaveType.txt, string extendPath = "")
    {
		var pathData = Path.Combine(Application.persistentDataPath, "data");

		if (!Directory.Exists(pathData))
		{
			DirectoryInfo directory = new DirectoryInfo(pathData);
			Directory.CreateDirectory(pathData);
		}

		var path = "";

		if (string.IsNullOrEmpty(extendPath))
		{
			path = Application.persistentDataPath + "/data" + "/" + (fileName + NameExtend(type));
		}
		else
		{
			path = Application.persistentDataPath + "/data" + "/" + extendPath  + (fileName + NameExtend(type));
		}


		return File.Exists(path);
	}

	public static string NameExtend(ObjectSaveType type)
    {
        switch (type)
        {
            case ObjectSaveType.txt:
                return ".txt";
            case ObjectSaveType.mp3:
                return ".mp3";
            case ObjectSaveType.mp4:
                return ".mp4";
            case ObjectSaveType.png:
                return ".png";
            case ObjectSaveType.bundle:
                return ".unity";
            default:
                break;
        }
        return null;
    }

	public static string FilePath()
	{
		return PathDataNoExpired;
	}

	public static string PathDataNoExpired
	{
		get { return Path.Combine(Application.temporaryCachePath, "data"); }
	}
}

public enum ObjectSaveType
{
    txt,
    mp3,
    mp4,
    png,
    bundle,
}

