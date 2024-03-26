using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Utils;
using UnityEngine;
using UnityEngine.UI;

# region Random single
public class RandomSingleObject<T> : MonoBehaviour
{

    public static Dictionary<List<object>, List<object>> SpawnRandomDic = new Dictionary<List<object>, List<object>>();

    public static Dictionary<List<T>, List<object>> KeyDic = new Dictionary<List<T>, List<object>>();

    public static void FreeDictionary()
    {
        SpawnRandomDic.Clear();
        KeyDic.Clear();
    }
    public static void Remove(List<T> listObject)
    {
        if (KeyDic.ContainsKey(listObject))
        {
            var listObjectKey = KeyDic[listObject];
            if (SpawnRandomDic.ContainsKey(listObjectKey))
            {
                SpawnRandomDic.Remove(listObjectKey);
                KeyDic.Remove(listObject);
            }
        }
        else
        {
            return;
        }
    }
    public static void ManualReFill(List<T> listObject)
    {
        if (KeyDic.ContainsKey(listObject))
        {
            var listObjectKey = KeyDic[listObject];
            if (SpawnRandomDic.ContainsKey(listObjectKey))
            {
                var listTemp = SpawnRandomDic[listObjectKey];
                // refill 
                listTemp = new List<object>();
                SpawnRandomDic[listObjectKey] = ReFill(listTemp, listObjectKey);
            }
        }
    }

    //public static T GetRandom(this List<T> listInput)
    //{
    //    return GetRandomData(listInput);
    //}

    public static T GetRandom(List<T> listInput)
    {
        if (KeyDic.ContainsKey(listInput))
        {
            return GetRandomRemainOnDic(listInput);
        }
        else
        {
            var keyList = ConvertListToListObject(listInput);

            var listTemp = new List<object>();
            for (int i = 0; i < listInput.Count; i++)
            {
                var newObj = new ObjRandomStatus(listInput[i]);
                listTemp.Add(newObj);
            }
            SpawnRandomDic.Add(keyList, listTemp);

            return GetRandomRemainOnDic(listInput);
        }
    }

    public static void ManuaRefesh(List<T> listInput)
    {
        if (KeyDic.ContainsKey(listInput))
        {
            var listTemp = SpawnRandomDic[KeyDic[listInput]];
            for (int i = 0; i < listTemp.Count; i++)
            {
                var newRemain = listTemp[i] as ObjRandomStatus;
                newRemain.used = false;
            }
        }
    }

    public static T GetRandomRemainOnDic(List<T> listInput)
    {
        var tempList = SpawnRandomDic[KeyDic[listInput]];

        var lstGroup = tempList.GroupBy(it => (it as ObjRandomStatus).countUsed).ToList();
        lstGroup = lstGroup.OrderBy(it => it.Key).ToList();
        tempList = lstGroup[0].ToList();
        //tuoc
        var listRemain = new List<ObjRandomStatus>();
        for (int i = 0; i < tempList.Count; i++)
        {
            var newRemain = tempList[i] as ObjRandomStatus;

            listRemain.Add(newRemain);
        }
        //=======

        //var listRemain = new List<ObjRandomStatus>();

        //for (int i = 0; i < tempList.Count; i++)
        //{
        //    var newRemain = tempList[i] as ObjRandomStatus;

        //    if (!newRemain.used) listRemain.Add(newRemain);
        //}

        if (listRemain.Count == 0)
        {
            for (int i = 0; i < tempList.Count; i++)
            {
                var newRemain = tempList[i] as ObjRandomStatus;
                newRemain.used = false;
            }
            return GetRandomRemainOnDic(listInput);
        }
        else
        {
            var randomIndex = UnityEngine.Random.Range(0, listRemain.Count);

            var random = listRemain[randomIndex];

            random.used = true;
            random.countUsed++;
            return (T)Convert.ChangeType(random.obj, typeof(T));
        }
    }

    public static List<T> GetRandomList(int count, List<T> lisInput)
    {
        var newList = new List<T>();

        var outOfPool = false;

        for (int i = 0; i < count; i++)
        {
            if (!outOfPool)
            {
                var random = GetRandomSingle(lisInput);
                newList.Add(random);
                if (CheckIsLastObjInPool(lisInput))
                {
                    outOfPool = true;
                    ManualReFill(lisInput);
                }
            }
            else
            {
                var random = GetRandomException(newList, lisInput);
                RemoveInTemp(lisInput, random);
                newList.Add(random);
            }
        }
        return newList;
    }

    public static T GetRandomException(List<T> listEx, List<T> ListObj)
    {
        var tempList = new List<T>();

        for (int i = 0; i < ListObj.Count; i++)
        {
            var obj = ListObj[i];

            if (!listEx.Contains(obj))
            {
                tempList.Add(obj);
            }
        }

        var random = UnityEngine.Random.Range(0, tempList.Count * 100) % tempList.Count;



        return tempList[random];
    }

    public static void RemoveInTemp(List<T> ListObj, T obj)
    {
        if (KeyDic.ContainsKey(ListObj))
        {
            var key = KeyDic[ListObj];
            var tempList = SpawnRandomDic[key];
            tempList.Remove(obj as object);
        }
        else
        {
            Debug.Log("khong co list nay trong danh sach quan ly");
        }
    }

    public static bool CheckIsLastObjInPool(List<T> listObject)
    {
        if (KeyDic.ContainsKey(listObject))
        {
            var key = KeyDic[listObject];
            var tempList = SpawnRandomDic[key];
            return tempList.Count == 0 ? true : false;
        }
        return false;
    }

    public static T GetRandomSingle(List<T> listObject)
    {
        var newList = ConvertListToListObject(listObject);
        return (T)Convert.ChangeType(GetRandomObject(newList), typeof(T));
    }

    public static void AddObjectToTempList(T Object, List<T> listObject)
    {
        var newList = ConvertListToListObject(listObject);

        AddObjectToTemp(newList, Object);
    }

    #region private function
    static List<object> ConvertListToListObject(List<T> listSource)
    {

        if (KeyDic.ContainsKey(listSource))
        {
            var newList = KeyDic[listSource];
            return newList;
        }
        else
        {

            List<object> newList = new List<object>();
            for (int i = 0; i < listSource.Count; i++)
            {
                var sourceElement = listSource[i] as object;

                newList.Add(sourceElement);
            }

            KeyDic.Add(listSource, newList);
            return newList;
        }

    }

    static void AddObjectToTemp(List<object> listObject, T obj)
    {
        if (SpawnRandomDic.ContainsKey(listObject))
        {
            var tempList = SpawnRandomDic[listObject];
            tempList.Add(obj as object);
        }
        else
        {
            Debug.Log("list not remain");
        }
    }


    static object GetRandomObject(List<object> listObject)
    {
        if (SpawnRandomDic.ContainsKey(listObject))
        {
            var tempList = SpawnRandomDic[listObject];

            ReFill(tempList, listObject);

            var randomIndex = UnityEngine.Random.Range(0, tempList.Count * 100) % tempList.Count;
            var randomObject = tempList[randomIndex];
            tempList.Remove(randomObject);
            return randomObject;
        }
        else
        {
            var tempList = new List<object>();
            SpawnRandomDic.Add(listObject, tempList);

            ReFill(tempList, listObject);

            var randomIndex = UnityEngine.Random.Range(0, tempList.Count * 100) % tempList.Count;
            var randomObject = tempList[randomIndex];
            tempList.Remove(randomObject);
            return randomObject;
        }
    }

    static List<object> ReFill(List<object> listTarget, List<object> listSource)
    {
        if (listTarget.Count < 1)
        {
            for (int i = 0; i < listSource.Count; i++)
            {
                listTarget.Add(listSource[i]);
            }
        }
        return listTarget;
    }

    class ObjRandomStatus
    {
        public object obj;
        public bool used;
        public int countUsed;

        public ObjRandomStatus(T obj, bool used = false)
        {
            this.obj = obj;
            this.used = used;
            countUsed = 0;
        }
    }
    #endregion
}



# endregion

public interface ISpawn<T>
{
    void OnSpawnDone(T objSpawn);
}
public class SpawnObject : MonoBehaviour, ISpawn<SpawnObject>
{
    public void OnSpawnDone(SpawnObject objSpawn)
    {
        SpawnDone();
    }
    public virtual void SpawnDone()// child override this
    {

    }
}



//public class RandomSpawn<T> : MonoBehaviour
//{
//    public static Dictionary<T, SpawnData> spawnDic = new Dictionary<T, SpawnData>();

//    static GameObject spawnFuntion = new GameObject();

//    static SpawnCoroutine spawnClass = new SpawnCoroutine();

//    static bool HaveObject = false;

//    public static void StartSpawn(float maxTime, float deltaTime, T objSpawn, T paren, System.Action<T> SpawnFunc = null, List<T> listTrans = null)
//    {
//        if (HaveObject)
//        {

//        }
//        else
//        {
//            spawnFuntion.AddComponent<SpawnCoroutine>();
//            spawnClass = spawnFuntion.GetComponent<SpawnCoroutine>();
//            DontDestroyOnLoad(spawnFuntion);
//            HaveObject = true;
//        }

//        SpawnData spawnData = new SpawnData();


//        if (spawnDic.ContainsKey(objSpawn))
//        {
//            spawnData = spawnDic[objSpawn];
//            spawnData.maxTime = maxTime;
//            spawnData.deltaTime = deltaTime;
//            spawnData.listChar = listTrans;
//            spawnClass.StopCoroutineCustom(spawnData.thisCoroutine);
//        }
//        else
//        {
//            spawnData = new SpawnData(maxTime, deltaTime)
//            {
//                listChar = listTrans
//            };
//            spawnDic.Add(objSpawn, spawnData);
//        }

//        spawnData.onSpawnFunction = SpawnFunc;
//        spawnData.thisCoroutine = spawnClass.StartCoroutineCustom(spawnData, objSpawn, paren);
//    }
//    public static void PauseSpawn(T objSpawn)
//    {
//        if (spawnDic.ContainsKey(objSpawn))
//        {
//            var spawnData = spawnDic[objSpawn];
//            spawnData.Spawning = false;
//        }
//        else
//            return;
//    }

//    public static void StopSpawm(T objSpawn, bool clearParent = false)
//    {
//        if (spawnDic.ContainsKey(objSpawn))
//        {
//            var spawnData = spawnDic[objSpawn];
//            spawnClass.StopCoroutineCustom(spawnData.thisCoroutine);
//            if (clearParent)
//            {
//                FunctionCommon.DeleteAllChild(spawnData.parent.gameObject);
//            }
//            spawnDic.Remove(objSpawn);
//        }
//    }

//    public static void ResumeSpawn(T objSpawn)
//    {
//        if (spawnDic.ContainsKey(objSpawn))
//        {
//            var spawnData = spawnDic[objSpawn];
//            spawnData.Spawning = true;
//        }
//        else
//            return;
//    }




//    public class SpawnData
//    {
//        public float maxTime;
//        public float deltaTime;
//        public float currentTime;
//        public Transform parent;

//        public bool Spawning;

//        public List<T> listSpawned = new List<T>();

//        public Coroutine thisCoroutine;

//        public System.Action<T> onSpawnFunction;

//        public List<T> listChar = new List<T>();

//        public SpawnData(float maxTime = 0, float deltaTime = 0)
//        {
//            this.maxTime = maxTime;
//            this.deltaTime = deltaTime;
//            currentTime = maxTime;
//            Spawning = false;
//        }


//    }

//}
public class FunctionCommon<T>
{

    public static List<T> CloneList(List<T> listObj)
    {
        var listTarget = new List<T>();

        for (int i = 0; i < listObj.Count; i++)
        {
            var obj = listObj[i];
            listTarget.Add(obj);
        }
        return listTarget;
    }

    public static List<T> SwapIndex(List<T> listSwap, int index1, int index2)
    {
        var obj1 = listSwap[index1];
        var obj2 = listSwap[index2];

        listSwap[index2] = obj1;
        listSwap[index1] = obj2;

        return listSwap;
    }

    public static List<T> Swap(List<T> target)
    {
        List<T> result = new List<T>();
        List<int> indexPos = new List<int>();
        for (int i = 0; i < target.Count; i++)
        {
            indexPos.Add(i);
        }
        for (int i = 0; i < target.Count; i++)
        {
            int r = UnityEngine.Random.Range(0, indexPos.Count * 1000) % indexPos.Count;
            result.Add(target[indexPos[r]]);
            indexPos.RemoveAt(r);
        }
        return result;
    }
    public static List<T> ArrayToList(T[] array)
    {
        List<T> listTarget = new List<T>();

        for (int i = 0; i < array.Length; i++)
        {
            listTarget.Add(array[i]);
        }

        return listTarget;
    }
}
public class FunctionCommon : MonoBehaviour
{
    #region random char
    private static List<char> listChar = new List<char>() { 'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'z', 'x', 'c', 'v', 'b', 'n', 'm' };
    public static List<char> GetListKeyBoard(string wordMissing, int num, bool random = false)
    {
        if (num < wordMissing.Length) return null;
        List<char> result = new List<char>();
        for (int i = 0; i < wordMissing.Length; i++)
        {
            if (!result.Contains(wordMissing[i]))
                result.Add(wordMissing[i]);
        }
        List<char> poolChar = new List<char>();
        for (int i = 0; i < listChar.Count; i++)
        {
            poolChar.Add(listChar[i]);
        }
        for (int i = 0; i < result.Count; i++)
        {
            if (poolChar.Contains(result[i]))
                poolChar.Remove(result[i]);
        }
        List<char> addChar = new List<char>();
        if (num - wordMissing.Length > 0)
            addChar = RandomSingleObject<char>.GetRandomList(num - result.Count, poolChar);

        for (int i = 0; i < addChar.Count; i++)
        {
            result.Add(addChar[i]);
        }
        if (random)
        {
            result = FunctionCommon<char>.Swap(result);
        }
        return result;
    }

	#endregion

	/// <summary>
	/// Đây là code cũ, chỉ phù hợp cho project iDigi vì cố định tỉ lệ màn hình, project mới không còn phù hợp do app được sử dụng trên đa màn hình
	/// </summary>
	public static float screenScale
    {
        get
        {
            return (float)(Screen.width) / 1920f;
        }
    }

    public static int ConvertToIn(float value)
    {
        var intValue = (int)(value);

        return Mathf.Abs(value - intValue) >= 0.5f ? intValue + 1 : intValue;
    }



    public static string ConvertIntToMinus(int time)
    {
        var minus = time / 60;
        var seccon = time % 60;

        var minusString = minus >= 10 ? minus.ToString() : "0" + minus;
        var secconString = seccon >= 10 ? seccon.ToString() : "0" + seccon;
        return minusString + ":" + secconString;
    }

    //Tien
    public static string ConvertIntToMinusHasSpace(int time)
    {
        var minus = time / 60;
        var seccon = time % 60;

        var minusString = minus >= 10 ? minus.ToString() : "0" + minus;
        var secconString = seccon >= 10 ? seccon.ToString() : "0" + seccon;
        return minusString + " : " + secconString;
    }
    /// <summary>
    /// Tính lại khoảng cách (nếu dùng tranform) với các màn trình khấc nhau
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static float ConvertDistanceByScreen(float distance)
    {
        return distance * Screen.width / 1920f;
    }


    public static List<string> StringToListString(string input)
    {
        var value = new List<string>();

        var newArray = input.ToCharArray();

        for (int i = 0; i < newArray.Length; i++)
        {
            value.Add(newArray[i].ToString());
        }

        return value;
    }

    #region convert pos
    public static Vector2 ConvertInputScreenToRect(Vector2 input)
    {
        float rate = Screen.width / 1920f;
        if (rate == 1) return input;
        Vector2 pos = new Vector2();
        pos.x = input.x / rate;
        pos.y = input.y / rate - (Screen.height / rate - 1080f) / 2f;
        return pos;
    }
    #endregion

    #region swap
    public static List<Vector3> SwapPosition(List<Vector3> position)
    {
        List<Vector3> resultPos = new List<Vector3>();
        List<int> indexPos = new List<int>();
        for (int i = 0; i < position.Count; i++)
        {
            indexPos.Add(i);
        }
        for (int i = 0; i < position.Count; i++)
        {
            int r = UnityEngine.Random.Range(0, indexPos.Count);
            resultPos.Add(position[indexPos[r]]);
            indexPos.RemoveAt(r);
        }
        return resultPos;
    }
    public static List<int> SwapIndex(List<int> indextarget)
    {
        List<int> result = new List<int>();
        List<int> indexTmp = new List<int>();
        for (int i = 0; i < indextarget.Count; i++)
        {
            indexTmp.Add(i);
        }
        for (int i = 0; i < indextarget.Count; i++)
        {
            int r = UnityEngine.Random.Range(0, indexTmp.Count);
            result.Add(indextarget[indexTmp[r]]);
            indexTmp.RemoveAt(r);
        }
        return result;
    }
    public static List<int> RandomList(List<int> indextarget, int numResult = -1)
    {
        List<int> result = new List<int>();
        List<int> result2 = new List<int>();
        List<int> indexTmp = new List<int>();
        for (int i = 0; i < indextarget.Count; i++)
        {
            indexTmp.Add(i);
        }
        for (int i = 0; i < indextarget.Count; i++)
        {
            int r = UnityEngine.Random.Range(0, indexTmp.Count);
            result.Add(indextarget[indexTmp[r]]);
            result2.Add(indexTmp[r]);
            indexTmp.RemoveAt(r);
        }

        if (numResult == -1)
        {
            return result;
        }
        else if (numResult <= result2.Count && numResult > 0)
        {
            List<int> end = new List<int>();
            for (int i = 0; i < result2.Count; i++)
            {
                end.Add(indextarget[result2[i]]);
            }
            for (int i = end.Count - 1; i >= 0; i--)
            {
                if (i >= numResult)
                {
                    end.RemoveAt(i);
                }
            }
            return end;
        }
        else
        {
            return null;
        }
    }
    public static List<int> GetIndexList(int countList)
    {
        List<int> index = new List<int>();
        for (int i = 0; i < countList; i++)
        {
            index.Add(i);
        }
        return index;
    }
    #endregion

    #region random
    public static List<int> RandomIndex(int countList, int numResult)
    {
        if (countList < numResult)
        {
            Debug.LogError("RandomIndex error");
            return null;
        }
        List<int> result;
        List<int> index = GetIndexList(countList);
        result = SwapIndex(index);
        for (int i = 0; i < countList - numResult; i++)
        {
            result.RemoveAt(result.Count - 1);
        }
        return result;
    }

    public static void RandomPosition(List<GameObject> listGo)
    {
        List<Vector3> listPos = new List<Vector3>();
        for (int i = 0; i < listGo.Count; i++)
        {
            listPos.Add(listGo[i].transform.position);
        }

        for (int i = 0; i < listGo.Count; i++)
        {
            listGo[i].transform.position = RandomSingleObject<Vector3>.GetRandomSingle(listPos);
        }
    }
    #endregion

    #region path smooth
    public static Vector3 GetMiddlePoint(Vector3 begin, Vector3 end, float delta = 0)
    {
        Vector3 center = Vector3.Lerp(begin, end, 0.5f);
        Vector3 beginEnd = end - begin;
        Vector3 perpendicular = new Vector3(-beginEnd.y, beginEnd.x, 0).normalized;
        Vector3 middle = center + perpendicular * delta;
        return middle;
    }

    public static List<Vector3> PathSmooth(List<Vector3> point, int smooth)
    {
        if (point.Count < 3)
        {
            Debug.LogError("PathSmooth error");
            return null;
        }
        List<Vector3> path = point;
        for (int i = 0; i < smooth; i++)
        {
            path = LoopSmooth(path);
        }
        return path;
    }
    private static List<Vector3> LoopSmooth(List<Vector3> path)
    {
        List<Vector3> result = new List<Vector3>();
        result.Add(path[0]);
        for (int i = 0; i < path.Count - 1; i += 2)
        {
            Vector3 point1 = 3 * path[i] / 4f + path[i + 1] / 4;
            Vector3 point2 = path[i] / 4f + 3 * path[i + 1] / 4;
            result.Add(point1);
            result.Add(point2);
        }
        result.Add(path[path.Count - 1]);
        return result;
    }


    public static void MovePath(Transform target, List<Transform> path, float speed,
        Action callBack = null,
        Action onMoveEachDone = null,
        float delayStart = 0f)
    {
        if (OnMovePath) return;
        List<Vector3> path1 = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            path1.Add(path[i].position);
        }
        OnMovePath = true;
        pathMove.Clear();
        speedMove = speed * Screen.width / 1920f;
        delayOnStar = delayStart;
        if (callBack != null)
        {
            onMovePathDone = callBack;
        }
        if (onMoveEachDone != null)
        {
            onMoveEachPathDone = onMoveEachDone;
        }

        for (int i = 0; i < path1.Count; i++)
        {
            pathMove.Add(path1[i]);
        }
        if (path1.Count >= 2)
        {
            LoopMove(target);
        }
        else
        {
            OnMovePath = false;
            if (onMovePathDone != null)
            {
                onMovePathDone();
            }

        }
    }
    public static void MovePath(Transform target, List<Vector3> path, float speed, Action callBack = null, Action onMoveEachPath = null)
    {
        if (OnMovePath) return;
        OnMovePath = true;
        pathMove.Clear();
        speedMove = speed * Screen.width / 1920f;
        delayOnStar = 0f;
        if (callBack != null)
        {
            onMovePathDone = callBack;
        }

        if (onMoveEachPath != null)
        {
            onMoveEachPathDone = onMoveEachPath;
        }

        for (int i = 0; i < path.Count; i++)
        {
            pathMove.Add(path[i]);
        }
        if (path.Count >= 2)
        {
            LoopMove(target);
        }
        else
        {
            OnMovePath = false;
            if (onMovePathDone != null)
            {
                onMovePathDone();
            }

        }
    }

    private static List<Vector3> pathMove = new List<Vector3>();
    private static float speedMove;
    public static bool OnMovePath = false;
    private static Action onMovePathDone;
    private static Action onMoveEachPathDone;
    private static float delayOnStar = 0f;
    private static void LoopMove(Transform target)
    {
        target.transform.position = pathMove[0];
        target.DelayCall(delayOnStar, () =>
        {
            TweenControl.GetInstance().Move(target, pathMove[1], Vector2.Distance(pathMove[0], pathMove[1]) / speedMove,
            () =>
            {
                pathMove.RemoveAt(0);
                if (pathMove.Count >= 2)
                {
                    if (onMoveEachPathDone != null)
                        onMoveEachPathDone();
                    LoopMove(target);
                }
                else
                {
                    OnMovePath = false;
                    if (onMovePathDone != null)
                    {
                        onMovePathDone();
                    }
                }
            });
        });
    }
    #endregion


    public static string ColorToHex(Color32 color)
    {
        Color32 c = color;
        var hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", c.r, c.g, c.b, c.a);
        return hex;
    }


    #region convert coordinates
    public static Vector2 GetAnchoredPositionFromWorldPosition(Vector3 _worldPostion, Camera _camera, Canvas _canvas)
    {
        //Vector2 myPositionOnScreen = _camera.WorldToScreenPoint(_worldPostion); // for transform.position?
        Vector2 myPositionOnScreen = _camera.WorldToViewportPoint(_worldPostion); //for RectTransform.AnchoredPosition?
        float scaleFactor = _canvas.scaleFactor;
        return new Vector2(myPositionOnScreen.x / scaleFactor, myPositionOnScreen.y / scaleFactor);
    }

    public static void WorldSpaceToCanVas(RectTransform target, Vector3 pos)
    {
        //convert game object position to VievportPoint
        Vector2 viewportPoint = Camera.main.WorldToViewportPoint(pos);
        // set MIN and MAX Anchor values(positions) to the same position (ViewportPoint)
        target.anchorMin = viewportPoint;
        target.anchorMax = viewportPoint;
    }
    #endregion

    #region delete all child
    public static void DeleteAllChild(GameObject target)
    {
        List<GameObject> listChild = new List<GameObject>();
        for (int i = 0; i < target.transform.childCount; i++)
        {
            listChild.Add(target.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < listChild.Count; i++)
        {
            Destroy(listChild[i]);
        }
    }
    public static void DeleteAllChild(Transform target)
    {
        List<GameObject> listChild = new List<GameObject>();
        for (int i = 0; i < target.transform.childCount; i++)
        {
            listChild.Add(target.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < listChild.Count; i++)
        {
            Destroy(listChild[i]);
        }
    }
    #endregion

    #region setText
    public static void SetText(Text target, string text)
    {
        if (target == null) return;
        string newText = text.Replace("\\n", "\n");
        target.text = newText;
    }
    public static string RemoveLastChar(string input)
    {
        return input.Remove(input.Length - 1);
    }
    #endregion

    //#region showSub




    //public static List<AudioSource> ArrayToList(AudioSource[] arraySource)
    //{
    //    List<AudioSource> listTarget = new List<AudioSource>();

    //    for (int i = 0; i < arraySource.Length; i++)
    //    {
    //        listTarget.Add(arraySource[i]);
    //    }

    //    return listTarget;
    //}

    //public static void Setsub(Text target, string textSub, AudioClip listVoiceSub, float timeScale = 0.25f, System.Action voiceDoneCallBack = null, bool stopOther = false, bool noneHide = false)
    //{
    //    TweenControl.GetInstance().ScaleFromZero(target.gameObject, timeScale);
    //    var time = 0.0f;
    //    if (listVoiceSub != null)
    //    {
    //        time += listVoiceSub.length;
    //        SetText(target, textSub);
    //        if (GameAudio.GetInstance() != null)
    //        {
    //            GameAudio.GetInstance().PlayClip(listVoiceSub, stopOther);
    //        }
    //        else
    //        {
    //            GameAudioNew.GetInstance().PlayClip(SourceType.voiceOver, listVoiceSub, stopOther);
    //        }
    //    }
    //    if (voiceDoneCallBack != null)
    //    {
    //        TweenControl.GetInstance().DelayCall(target.transform, time, () =>
    //        {
    //            if (!noneHide)
    //            {
    //                TweenControl.GetInstance().ScaleFromOne(target.gameObject, timeScale, () =>
    //                {
    //                    voiceDoneCallBack?.Invoke();
    //                });
    //            }
    //            else
    //            {
    //                voiceDoneCallBack?.Invoke();
    //            }
    //        });
    //    }
    //}


    //public static void Setsub(Text target, List<string> listTextSub, List<AudioClip> listVoiceSub, float timeScale = 0.25f, System.Action voiceDoneCallBack = null, bool stopOther = false, bool noneHide = false)
    //{
    //    TweenControl.GetInstance().ScaleFromZero(target.gameObject, timeScale);
    //    var time = 0.0f;
    //    for (int i = 0; i < listVoiceSub.Count; i++)
    //    {
    //        if (i > 0)
    //        {
    //            var index = i;
    //            TweenControl.GetInstance().DelayCall(target.transform, time, () =>
    //            {
    //                SetText(target, listTextSub[index]);
    //                if (GameAudio.GetInstance() != null)
    //                {
    //                    GameAudio.GetInstance().PlayClip(listVoiceSub[index], stopOther);
    //                }
    //                else
    //                {
    //                    GameAudioNew.GetInstance().PlayClip(SourceType.voiceOver, listVoiceSub[index], stopOther);
    //                }
    //            });
    //            if (listVoiceSub[i] != null)
    //            {
    //                time += listVoiceSub[i].length;
    //            }
    //        }
    //        else
    //        {
    //            if (listVoiceSub[0] != null)
    //            {
    //                time += listVoiceSub[0].length;
    //            }
    //            SetText(target, listTextSub[0]);
    //            if (GameAudio.GetInstance() != null)
    //            {
    //                GameAudio.GetInstance().PlayClip(listVoiceSub[0], stopOther);
    //            }
    //            else
    //            {
    //                GameAudioNew.GetInstance().PlayClip(SourceType.voiceOver, listVoiceSub[0], stopOther);
    //            }
    //        }
    //    }
    //    TweenControl.GetInstance().DelayCall(target.transform, time, () =>
    //    {
    //        if (!noneHide)
    //        {
    //            TweenControl.GetInstance().ScaleFromOne(target.gameObject, timeScale, () =>
    //            {
    //                voiceDoneCallBack?.Invoke();
    //            });
    //        }
    //        else
    //        {
    //            voiceDoneCallBack?.Invoke();
    //        }
    //    });
    //}

    //#endregion

    #region angle between



    // keyPoint check left side or right size
    public static SideData CheckSide(Vector2 vect1, Vector2 vect2)
    {
        var side = new SideData();

        var vectorDir = new Vector2(vect1.y, -vect1.x); // vector phap tuyen ve phia ben phai

        var angle = Vector2.Angle(vectorDir, vect2);
        var angle2 = Vector2.Angle(vect1, vect2);

        side.side = angle >= 0 && angle <= 90 ? Side.Right : Side.Left;

        side.angle = angle2;

        return side;
    }

    #endregion

    public static void HightLightObj(GameObject obj, float deltaScale = 1.1f, float timeScale = 0.1f, System.Action callBack = null)
    {
        TweenControl.GetInstance().Scale(obj, Vector3.one * deltaScale, timeScale, () =>
        {
            TweenControl.GetInstance().Scale(obj, Vector3.one, timeScale, () =>
           {
               callBack?.Invoke();
           }, EaseType.Linear);
        }, EaseType.Linear);
    }
}

public class SideData
{
    public Side side;
    public float angle;
}

public enum Side
{
    Left,
    Right
}
