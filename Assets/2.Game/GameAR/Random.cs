using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game
{
    public class Random<T>
    {
        public static List<T> GetList(List<T> target, int numResult)
        {
            List<T> result = new List<T>();
            List<int> id = new List<int>();
            for (int i = 0; i < target.Count; i++)
            {
                id.Add(i);
            }
            if (numResult > target.Count)
                numResult = target.Count;
            for (int i = 0; i < numResult; i++)
            {
                int r = UnityEngine.Random.Range(0, id.Count);
                result.Add(target[r]);
                id.RemoveAt(r);
            }
            return result;
        }
    }
}

