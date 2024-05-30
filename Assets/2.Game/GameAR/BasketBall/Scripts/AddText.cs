using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace Game.BasketBall
{

    public class AddText : MonoBehaviour
    {
        public Color color;
        private TextMeshPro _textMeshPro;
        public void Init(int value)
        {
            if (_textMeshPro == null)
                _textMeshPro = GetComponent<TextMeshPro>();
            _textMeshPro.text = "+" + value;
            _textMeshPro.color = color;
            TweenControl.GetInstance().ValueTo(transform, (param) =>
            {
                _textMeshPro.color = new Color(color.r, color.g, color.b, param);
            }, 1, 0, 0.95f);
            TweenControl.GetInstance().Move(transform, transform.position + new Vector3(0, 0.15f, 0), 1, () =>
            {
                Destroy(gameObject);
            });
        }

    }
}

