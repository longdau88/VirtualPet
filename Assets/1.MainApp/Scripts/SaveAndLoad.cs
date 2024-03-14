using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveAndLoad<T>
{
	public static bool FileIsExpired(string fileName, ObjectSaveType objType, PathFile pathFile, float maxDate)
	{
		fileName = fileName.Replace(".", "");
		fileName = fileName + NameExtend(objType);

		fileName = fileName.Replace("/", "");

		var filePath = Path.Combine(FilePath(pathFile), fileName + NameExtend(objType));

		var dateTime = File.GetLastWriteTime(filePath);
		var length = DateTime.Now - dateTime;

		return length.TotalDays >= maxDate;
	}

	public static bool FileIsExpired(string fileName, float maxDate)
	{
		var dateTime = File.GetLastWriteTime(fileName);
		var length = DateTime.Now - dateTime;

		return length.TotalDays >= maxDate;
	}

	private static bool FileIsExpired(string filePath, int timeExpired)
	{
		var dateTime = File.GetLastWriteTime(filePath);
		var length = DateTime.Now - dateTime;

		return length.TotalMinutes >= timeExpired;
	}

	public static void DeleteFile(string filePath)
	{
		if (!File.Exists(filePath))
		{
			return;
		}

		try
		{
			File.Delete(filePath);
		}
		catch (Exception e)
		{
			Debug.Log(string.Format("Fail to delete file {0}. Reason:  {1}", filePath, e));
		}
	}

	public static void DeleteFile(string fileName, ObjectSaveType objType, PathFile pathFile)
	{
		fileName = fileName.Replace(".", "");
		fileName = fileName + NameExtend(objType);

		fileName = fileName.Replace("/", "");

		var filePath = Path.Combine(FilePath(pathFile), fileName + NameExtend(objType));

		try
		{
			File.Delete(filePath);
		}
		catch (Exception e)
		{
			Debug.Log(string.Format("Fail to delete file {0}. Reason:  {1}", filePath, e));
		}
	}

    public static void Save(T obj, string fileName, ObjectSaveType objType, string extendPath = "")
    {
        string path = Application.persistentDataPath + "/data/" + extendPath;
        var filePath = path + fileName + NameExtend(objType);
		if (!CheckFilePathExits(filePath, PathFile.Metadata))
		{
			var fileCreate = File.Create(filePath);
			fileCreate.Close();
		}
		File.WriteAllBytes(filePath, ObjectToByteArray(obj as object));
    }

	public static void SaveFile(byte[] obj, string fileName, ObjectSaveType objType, PathFile pathFile)
	{
		if (string.IsNullOrEmpty(fileName)) return;
		string path = FilePath(pathFile);

		fileName = fileName.Replace(".", "");
		fileName = fileName.Replace("/", "");

		var filePath = Path.Combine(path, fileName + NameExtend(objType));

		if (!CheckFilePathExits(filePath, pathFile))
		{
			var fileCreate = File.Create(filePath);
			fileCreate.Close();
		}

		try
		{
			File.WriteAllBytes(filePath, obj);
		}
		catch (Exception e)
		{
			Debug.LogError("Fail to save file: " + e.Message);
		}
	}

	public static void SaveFileCustomize(byte[] obj, string fileName, ObjectSaveType objType, PathFile pathFile)
	{
		if (string.IsNullOrEmpty(fileName)) return;
		string path = FilePath(pathFile);

		fileName = fileName.Replace(".", "");
		fileName = fileName.Replace("/", "");

		var filePath = Path.Combine(path, fileName + NameExtend(objType));

		if (!CheckFilePathExits(filePath, pathFile))
		{
			var fileCreate = File.Create(filePath);
			fileCreate.Close();
		}

		try
		{
			File.WriteAllBytes(filePath, obj);
		}
		catch (Exception e)
		{
			Debug.LogError("Fail to save file: " + e.Message);
		}
	}

	public static void SaveFile(T obj, string fileName, ObjectSaveType objType, PathFile pathFile)
	{
		string path = FilePath(pathFile);

		fileName = fileName.Replace(".", "");
		fileName = fileName.Replace("/", "");

		var filePath = Path.Combine(path, fileName + NameExtend(objType));

		if (!CheckFilePathExits(filePath, pathFile))
		{
			var fileCreate = File.Create(filePath);
			fileCreate.Close();
		}
		File.WriteAllBytes(filePath, ObjectToByteArray(obj as object));
	}

	public static void SaveAssetBundle(byte[] obj, string fileName, PathFile pathFile)
	{
		string path = FilePath(pathFile);

		fileName = fileName.Replace(".", "");
		fileName = fileName.Replace("/", "");

		var filePath = Path.Combine(path, fileName + NameExtend(ObjectSaveType.bundle));

		if (!CheckFilePathExits(filePath, PathFile.Assetbundle))
		{
			var fileCreate = File.Create(filePath);
			fileCreate.Close();
		}
		;
		try
		{
			File.WriteAllBytes(filePath, obj);
		}
		catch (Exception e)
		{
			Debug.Log("Failed To Save Data to: " + path.Replace("/", "\\"));
			Debug.Log("Error: " + e.Message);
		}
	}

    public static byte[] ObjectToByteArray(object obj)
    {
        return (byte[])obj;
    }

    public static object ByteArrayToObject(byte[] arrBytes)
    {
        return (object)arrBytes;
    }

    public static T ByteArrayToObjectConvert(byte[] array)
    {
		try
		{
			return (T)Convert.ChangeType(ByteArrayToObject(array), typeof(T));
		}
		catch (System.Exception)
		{
			return default(T);
		}
    }

	public byte[] ToByteArray<T>(T obj)
	{
		if (obj == null)
			return null;
		BinaryFormatter bf = new BinaryFormatter();
		using (MemoryStream ms = new MemoryStream())
		{
			bf.Serialize(ms, obj);
			return ms.ToArray();
		}
	}

	public static T FromByteArray(byte[] data)
	{
		if (data == null)
			return default(T);
		BinaryFormatter bf = new BinaryFormatter();
		using (MemoryStream ms = new MemoryStream(data))
		{
			object obj = bf.Deserialize(ms);
			return (T)obj;
		}
	}

	#region text file

	public static void SaveJson(T obj, string fileName, ObjectSaveType type = ObjectSaveType.txt, string extendPath = "", PathFile pathFile = PathFile.Json)
    {
		string path = FilePath(pathFile);
		fileName = fileName.Replace(".", "");
		fileName = fileName.Replace("/", "");

		var filePath = Path.Combine(path, fileName + NameExtend(type));

		if (!CheckFilePathExits(filePath, PathFile.Json))
		{
			var thisFile = File.Create(filePath);
			thisFile.Close();
		}

		File.WriteAllText(filePath, JsonUtility.ToJson(obj as object));
    }

    public static T LoadFileText(string fileName, string extendPath = "")
    {
		string path = PathJson;

		fileName = fileName.Replace(".", "");
		fileName = fileName.Replace("/", "");


		var filePath = Path.Combine(path, fileName + NameExtend(ObjectSaveType.txt));
        var checkFile = CheckFilePathExits(filePath, PathFile.Json);
        return JsonUtility.FromJson<T>(checkFile ? File.ReadAllText(filePath) : "");
    }

	public static T LoadFileTextNoExpired(string fileName)
	{
		string path = PathDataNoExpired;

		fileName = fileName.Replace(".", "");
		fileName = fileName.Replace("/", "");


		var filePath = Path.Combine(path, fileName + NameExtend(ObjectSaveType.txt));
		var checkFile = CheckFilePathExits(filePath, PathFile.DataNoExpired);
		return JsonUtility.FromJson<T>(checkFile ? File.ReadAllText(filePath) : "");
	}

    #endregion

	public static T LoadFile(string fileName, ObjectSaveType objType, PathFile pathFile)
	{
		var path = "";

		fileName = fileName.Replace(".", "") + NameExtend(objType);

		fileName = fileName.Replace("/", "");

		path = Path.Combine(FilePath(pathFile), fileName);
		
		var byArray = File.ReadAllBytes(path);

		return (T)ByteArrayToObjectConvert(byArray);
	}

	public static byte[] LoadByteArray(string fileName, ObjectSaveType objType, PathFile pathFile)
	{
		var path = "";

		fileName = fileName.Replace(".", "") + NameExtend(objType);

		fileName =  fileName.Replace("/", "");

		path = Path.Combine(FilePath(pathFile), fileName);
		return File.ReadAllBytes(path);
	}

	public static AssetBundleCreateRequest LoadAssetBundle(string fileName, PathFile pathFile)
	{
		var path = "";

		fileName = fileName.Replace(".", "") + NameExtend(ObjectSaveType.bundle);

		fileName = fileName.Replace("/", "");

		path = Path.Combine(FilePath(pathFile), fileName);

		return AssetBundle.LoadFromFileAsync(path);
	}

    public static bool CheckFilePathExits(string path, PathFile pathFile)
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

	public static bool CheckFileExist(string fileName, ObjectSaveType type, PathFile pathFile)
	{
		var timeExpired = TimeExpiredFile(pathFile);

		if (timeExpired == 0) return false;

		var pathData = FilePath(pathFile);

		fileName = fileName.Replace(".", "");
		fileName = fileName.Replace("/", "");
		var path = Path.Combine(pathData, (fileName + NameExtend(type)));
		
		if (File.Exists(path))
		{
			if (FileIsExpired(path, timeExpired) && pathFile != PathFile.DataNoExpired)
			{
				DeleteFile(path);
				return false;
			}
			else
			{
				return true;
			}
		}
		else
			return false;
	}

	public static bool CheckFileExistCustomize(string fileName, ObjectSaveType type, PathFile pathFile)
	{
		if (type == ObjectSaveType.mp3) return false;

		var pathData = FilePath(pathFile);

		fileName = fileName.Replace(".", "");
		fileName = fileName.Replace("/", "");
		var path = Path.Combine(pathData, (fileName + NameExtend(type)));

		if (File.Exists(path))
		{
			return true;
		}
		else
			return false;
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

	public static string FilePath(PathFile path)
	{
		switch (path)
		{
			case PathFile.Json:
				return PathJson;
			case PathFile.Metadata:
				return PathMetaData;
			case PathFile.PublisherCustomizeData:
				return PathPublisherCustomizeData;
			case PathFile.Assetbundle:
				return PathAssetBundle;
			case PathFile.DataNoExpired:
				return PathDataNoExpired;
			default:
				return PathJson;
		}
	}

	static int TimeExpiredFile(PathFile path)
	{
		return 1;
	}
	public static string PathJson
	{
		get { return Path.Combine(Application.temporaryCachePath, "data", "json"); }
	}

	public static string PathMetaData
	{
		get { return Path.Combine(Application.temporaryCachePath, "data", "metadata"); }
	}

	public static string PathPublisherCustomizeData
	{
		get { return Path.Combine(Application.temporaryCachePath, "data", "publishercustomizedata"); }
	}

	public string GetPathPublisherCustomizeData()
	{
		return PathPublisherCustomizeData;
	}

	public static string PathAssetBundle
	{
		get { return Path.Combine(Application.temporaryCachePath, "data", "assetbundle"); }
	}

	public static string PathDataNoExpired
	{
		get { return Path.Combine(Application.temporaryCachePath, "data", "otherNoExpired"); }
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

public enum PathFile
{
	Json,
	Metadata,
	PublisherCustomizeData,
	Assetbundle,
	DataNoExpired
}

