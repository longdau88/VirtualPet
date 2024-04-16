using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.TheRunner2
{
    public class Player : MonoBehaviour
    {
        public string idleNameClip;
        public string jumpNameClip;
        public string runNameClip;
        public string winNameClip;
        public string dieNameClip;
        [Space]
        public float forceJump;
        public float ofssetPosXNewLife = 400;
        [Space]
        public AudioClip heartAudio;
        public AudioClip bubbleAudio;
        public AudioClip winAudio;
        public AudioClip loseAudio;
        public AudioClip hitAudio;
        public AudioClip jumpAudio;
        public AudioClip newlifeAudio;
        public AudioClip downHoleAudio;
        [Space]
        public SpineControl spineControl;
        public SpineControl spineControlMark;
        [SerializeField]
        private Rigidbody2D _rigidbody2D;

        private GameManager _gameControl;

        private bool _onGround;
        private bool _onJump = false;
        private Vector2 _posStart;
        private bool _init = false;

        public void Init(GameManager gameControl)
        {
            _posStart = transform.GetComponent<RectTransform>().anchoredPosition;
            _gameControl = gameControl;
            Idle();
            _init = true;
            _onGround = true;
            countjump = 0;

            spineControlMark.thisSkeletonControl.color = new Color(spineControlMark.thisSkeletonControl.color.r, spineControlMark.thisSkeletonControl.color.g, spineControlMark.thisSkeletonControl.color.b, 0);
        }


        public void Idle()
        {
            spineControl.SetAnimation(idleNameClip, true);
            spineControlMark.SetAnimation(idleNameClip, true);
        }

        public void Hit()
        {
            spineControlMark.thisSkeletonControl.color = new Color(spineControlMark.thisSkeletonControl.color.r, spineControlMark.thisSkeletonControl.color.g, spineControlMark.thisSkeletonControl.color.b, 1);
            TweenControl.GetInstance().DelayCall(transform, 0.2f, () =>
            {
                spineControlMark.thisSkeletonControl.color = new Color(spineControlMark.thisSkeletonControl.color.r, spineControlMark.thisSkeletonControl.color.g, spineControlMark.thisSkeletonControl.color.b, 0);
                TweenControl.GetInstance().DelayCall(transform, 0.2f, () =>
                {
                    spineControlMark.thisSkeletonControl.color = new Color(spineControlMark.thisSkeletonControl.color.r, spineControlMark.thisSkeletonControl.color.g, spineControlMark.thisSkeletonControl.color.b, 1);
                    TweenControl.GetInstance().DelayCall(transform, 0.2f, () =>
                    {
                        spineControlMark.thisSkeletonControl.color = new Color(spineControlMark.thisSkeletonControl.color.r, spineControlMark.thisSkeletonControl.color.g, spineControlMark.thisSkeletonControl.color.b, 0);
                    });
                });
            });
        }

        public void Die()
        {
            spineControl.SetAnimation(dieNameClip, true);
            spineControlMark.SetAnimation(dieNameClip, true);
        }

        public void Win()
        {
            spineControl.SetAnimation(winNameClip, true);
            spineControlMark.SetAnimation(winNameClip, true);
        }

        public void Run()
        {
            spineControl.SetAnimation(runNameClip, true);
            spineControlMark.SetAnimation(runNameClip, true);
        }

        public void Jump()
        {
            if (_onGround)
            {
                if (jumpAudio != null)
                    GameAudio.Instance.PlayClip(SourceType.SOUND_FX, jumpAudio, false);
                _onJump = true;
                _rigidbody2D.AddForce(Vector2.up * forceJump);
                spineControl.SetAnimation(jumpNameClip, false);
                spineControlMark.SetAnimation(jumpNameClip, false);
            }
            else
            {
                if (countjump >= 0 && countjump < 1)
                {
                    countjump++;
                    if (jumpAudio != null)
                        GameAudio.Instance.PlayClip(SourceType.SOUND_FX, jumpAudio, false);
                    _onJump = true;
                    _rigidbody2D.AddForce(Vector2.up * (forceJump / 2));
                    spineControl.SetAnimation(jumpNameClip, false);
                    spineControlMark.SetAnimation(jumpNameClip, false);
                }
            }
        }
        private bool _onFlick;
        public void Flick(System.Action callBack = null)
        {
            _onFlick = true;
            Hit();

            TweenControl.GetInstance().DelayCall(transform, 2f, () =>
            {
                _onFlick = false;
                Run();
                TweenControl.GetInstance().KillDelayCall(transform);
                callBack?.Invoke();
            });
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!_init) return;
            var item = collision.GetComponent<ItemController>();
            if (item)
            {
                switch (item.typeItem)
                {
                    case TypeItem.HEART:
                        if (heartAudio != null)
                            GameAudio.Instance.PlayClip(SourceType.SOUND_FX, heartAudio, false);

                        Destroy(collision.gameObject);

                        _gameControl.m_heart++;
                        if (_gameControl.m_heart >= 3) _gameControl.m_heart = 3;
                        _gameControl.SetHeart(true);

                        break;
                    case TypeItem.BUBBLE:
                        if (bubbleAudio != null)
                            GameAudio.Instance.PlayClip(SourceType.SOUND_FX, bubbleAudio, false);
                        Vector3 posItem = item.transform.position;

                        _gameControl.CheckItemBubble(item.gameObject.GetComponent<RectTransform>());
                        Destroy(item.gameObject);
                        break;
                    case TypeItem.OBSTACLE:
                        if (!_onFlick)
                        {
                            if (hitAudio != null)
                                GameAudio.Instance.PlayClip(SourceType.SOUND_FX, hitAudio, false);
                            //Destroy(collision.gameObject);

                            _gameControl.m_heart--;
                            if (_gameControl.m_heart <= 0)
                            {
                                _gameControl.SetHeart(true);
                                _gameControl.StopBG(true);
                                Die();
                                _gameControl.OnEndGame();
                            }
                            else
                            {
                                Flick();
                                _gameControl.SetHeart(true);
                            }
                        }
                        else
                        {
                            //Debug.Log("Bat tu");
                        }
                        break;
                }
            }
            else if (collision.gameObject.name == "DieZone")
            {
                if (downHoleAudio != null)
                    GameAudio.Instance.PlayClip(SourceType.SOUND_FX, downHoleAudio, false);
                NewLife();
                _gameControl.m_heart--;
                if (_gameControl.m_heart <= 0)
                {
                    _gameControl.m_heart = 0;
                    _gameControl.SetHeart(true);

                    _gameControl.StopBG(true);
                    _gameControl.OnEndGame();
                }
                else
                {
                    _gameControl.SetHeart(true);
                }
            }
        }

        int countjump;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!_init) return;
            if (collision.gameObject.name == "Ground")
            {
                if (_onJump && !_gameControl.EndGame)
                {
                    Run();
                    _onJump = false;
                    countjump = 0;
                }
                _onGround = true;
            }
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (!_init) return;
            if (collision.gameObject.name == "Ground")
            {
                _onGround = false;
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (!_init) return;
            if (collision.gameObject.name == "Ground" && !_onJump)
            {
                _onGround = true;
            }
        }

        private void Update()
        {
            if (!_init) return;
            if (_gameControl.EndGame)
            {
                Idle();
            }
#if UNITY_EDITOR
            if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && !_gameControl.EndGame)
            {
                Jump();
            }
#else
            if (Input.GetMouseButtonDown(0) && !_gameControl.EndGame)
            {
                Jump();
            }
#endif
        }
        public void NewLife()
        {
            float timeDelay = ofssetPosXNewLife / _gameControl.speedMove;
            gameObject.SetActive(false);
            _rigidbody2D.isKinematic = true;
            //Debug.Log("Time: " + timeDelay);
            TweenControl.GetInstance().DelayCall(_gameControl.transform, timeDelay, () =>
            {
                if (newlifeAudio != null)
                    GameAudio.Instance.PlayClip(SourceType.SOUND_FX, newlifeAudio);
                transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(_posStart.x, 370);
                _rigidbody2D.isKinematic = false;
                gameObject.SetActive(true);
                Run();
            });
        }
    }

}
