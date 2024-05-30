using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.SpaceInfiniteRunner
{
    public class Obj : MonoBehaviour
    {
        public ObjType objType;
        public bool hasRotate;
        public Vector3 rotate;
        public float speedRotate;
        private GamePlay _gamePlay;
        public void Init(GamePlay gamePlay, float timeDestroy)
        {
            _gamePlay = gamePlay;
            Invoke("Destroy", timeDestroy);
            Destroy(gameObject, timeDestroy);
        }
        public void Destroy()
        {
            Destroy(gameObject);
            _gamePlay.Objs.Remove(this);
        }

        private void OnDestroy()
        {
            CancelInvoke("Destroy");
        }

        private void Update()
        {
            if (hasRotate)
                transform.Rotate(rotate * Time.deltaTime * speedRotate);

        }
    }

    public enum ObjType
    {
        Rock,
        UFO,
    }
}


