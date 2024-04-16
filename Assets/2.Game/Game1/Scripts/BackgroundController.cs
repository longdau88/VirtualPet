using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.TheRunner2
{
    public class BackgroundController : MonoBehaviour
    {
        [SerializeField] List<ItemController> items;

        private Vector2 _size;
        private RectTransform _rect;
        private GameManager _gameControl;
        private bool _hasHeart;

        public void Init(GameManager gameControl, Vector2 posStart, Vector2 sizeBG, bool hasHeart)
        {
            _gameControl = gameControl;
            _rect = GetComponent<RectTransform>();
            _size = sizeBG;
            _rect.sizeDelta = _size;
            _rect.anchoredPosition = posStart;
            _hasHeart = hasHeart;
            LoadHeart();
            LoadImageItem();
        }
        public void Move()
        {
            TweenControl.GetInstance().MoveRectX(_rect, -_size.x, (_rect.anchoredPosition.x + _size.x) / _gameControl.speedMove, () =>
            {
                _gameControl.DestroyBackground(this);
            });
        }

        public void SetPause(bool isPause)
        {
            if (isPause)
            {
                TweenControl.GetInstance().PauseTweener(transform);
            }
            else
            {
                TweenControl.GetInstance().PlayTweener(transform);
            }

        }
        private void LoadHeart()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].typeItem == TypeItem.HEART)
                {
                    items[i].gameObject.SetActive(_hasHeart);
                }
                else
                {
                    items[i].gameObject.SetActive(true);
                }
            }
        }
        private void LoadImageItem()
        {
            /*var lstItem = items.Where(c => c.typeItem == TypeItem.BUBBLE).ToList();

            int countBubble = lstItem.Count;

            for (int i = 0; i < countBubble; i++)
            {
                if (i < (int)(countBubble * 2f / 3f))
                {
                    string s;

                    do
                    {
                        s = RandomSingleObject<string>.GetRandom(_gameControl.lstAnswerTrue);
                    }
                    while (s.Equals("_"));
                    var item = RandomSingleObject<ItemController>.GetRandom(lstItem);

                    item.Init(null, s);

                    lstItem.Remove(item);
                }
                else
                {
                    var s = RandomSingleObject<string>.GetRandom(_gameControl.lstAnswerFalse);
                    var item = RandomSingleObject<ItemController>.GetRandom(lstItem);

                    item.Init(null, s);
                    lstItem.Remove(item);
                }
            }*/
        }
    }
}