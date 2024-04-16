using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using Game;
using System.Linq;

public class ObjectAR : MonoBehaviour, IPointerClickHandler
{
    public Rigidbody rb;
    float timeBegin;
    ARController gameControl;

    Vector3 targetSize;
    float size;
    [SerializeField] RuntimeAnimatorController animController;

    private bool _isPreview = false;
    public bool IsPreview { get { return _isPreview; } set { _isPreview = value; } }
    public void SpawnObject(GameObject objectBubble = null)
    {
        gameControl = ARController.Instance;
        _variableJoystick = PlayerControl.Instance.variableJoystick;
    }

    public bool isVoiceVN { get; set; }

    public void AddForceObject()
    {
        Debug.Log("add force");
        var x = (Random.Range(0.03f, -0.03f));
        var z = Random.Range(0, 0.03f);
        rb.AddForce(new Vector3(x, 0.15f, z), ForceMode.Impulse);
    }

    public void AddForce(Vector3 vectorForce)
    {
        rb.AddForce(vectorForce, ForceMode.Impulse);
    }

    private void PlayAnimObject()
    {
        var anim = GetComponent<Animation>();
        if (anim != null) anim.Play();
    }

    private void SetObjectPhysics()
    {
        //gameControl.SetTextDebug("00");
        var timeDrag = Time.time - timeBegin;

        //gameControl.SetTextDebug(timeDrag.ToString());

        var force = timeDrag * 1.2f;

        if (force > 3) force = 3;
        //gameControl.SetTextDebug(force.ToString());

        var posObj = transform.position;
        var posCam = gameControl.GetCameraPostion();

        var vector = new Vector3(posObj.x - posCam.x, force, posObj.z - posCam.z);
        rb.AddForce(vector, ForceMode.Impulse);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //ARController.Instance.PlayVoiceFAQ();
    }

    #region HasControl

    public bool IsSelected { get; private set; }

    public PlayerAnimation playerAnimation;
    private VariableJoystick _variableJoystick;


    public float MoveSpeed { get; set; }
    public float FlySpeed { get; set; }
    public bool OnHoldFly { get; set; }

    private bool _enableJoystick;
    private void OnEnable()
    {
        if (playerAnimation)//model has anim
        {
            playerAnimation.Init(this);
            PlayerControl.Instance.keyOnClickEvent += OnClick;
            PlayerControl.Instance.KeyOnHoldDownEvent += OnHoldDown;
            PlayerControl.Instance.KeyOnHoldUpEvent += OnHoldUp;
            //PlayerControl.Instance.KeyOnHoldEvent += OnHold;
            OnHoldFly = false;
            EnableJoystick = true;
            MoveSpeed = playerAnimation.walkSpeed;
            FlySpeed = playerAnimation.flySpeed;
        }
        else
        {
            _enableJoystick = false;
        }
    }
    private void OnDisable()
    {
        if (playerAnimation)
        {
            PlayerControl.Instance.keyOnClickEvent -= OnClick;
            PlayerControl.Instance.KeyOnHoldDownEvent -= OnHoldDown;
            PlayerControl.Instance.KeyOnHoldUpEvent -= OnHoldUp;
            //PlayerControl.Instance.KeyOnHoldEvent -= OnHold;
        }
    }

    public void SetSelected(bool isSelected)
    {
        IsSelected = isSelected;
        //mr.material = isSelected ? selectedMaterial : normalMaterial;
    }
    public bool EnableJoystick { get => _enableJoystick; set => _enableJoystick = value; }

    private bool _onMove = false;
    public void FixedUpdate()
    {
        if (IsSelected)
        {
            //Vector3 direction = Vector3.forward * _variableJoystick.Vertical + Vector3.right * _variableJoystick.Horizontal;
            //rb.AddForce(direction * speedMove * Time.fixedDeltaTime, ForceMode.VelocityChange);
            if (_enableJoystick)
            {
                rb.velocity = new Vector3(_variableJoystick.Horizontal * MoveSpeed, OnHoldFly ? FlySpeed : 0, _variableJoystick.Vertical * MoveSpeed);
                if (_variableJoystick.Horizontal != 0 || _variableJoystick.Vertical != 0)
                {
                    rb.transform.rotation = Quaternion.LookRotation(rb.velocity);
                    playerAnimation.Move();
                    _onMove = true;
                }
                else if (OnHoldFly)
                {
                    playerAnimation.Fly();
                }
                else
                {
                    playerAnimation.Idle();
                    _onMove = false;
                }
            }
        }
    }

    public void PlayAudio()
    {
        AudioSource audi = GetComponent<AudioSource>();
        if (audi)
            audi.Play();
    }

    private AudioClip _voice;
    public void playVoiceAnim(string code, System.Action onComplete = null)
    {
        _voice = null;

        /*var foundFeature = gameControl.vocab.ar_script.FirstOrDefault(feature => feature.code == code);

        if (foundFeature != null)
        {
            if (isVoiceVN)
            {
                foundFeature.GetSoundVN((voice) =>
                {
                    _voice = voice;
                });
            }
            else
            {
                foundFeature.GetSoundEN((voice) =>
                {
                    _voice = voice;
                });
            }
        }
        else
        {
            foreach (var x in gameControl.vocab.ar_script)
            {
                if (x.code.ToLower().Equals("sound"))
                {
                    if (isVoiceVN)
                    {
                        x.GetSoundVN((voice) =>
                        {
                            _voice = voice;
                        });
                    }
                    else
                    {
                        x.GetSoundEN((voice) =>
                        {
                            _voice = voice;
                        });
                    }

                    break;
                }
            }
        }

        if (_voice != null)
        {
            gameControl.isPlayVoice = true;
            GameAudio.Instance.PlayClip(SourceType.VOICE_OVER, _voice, true, () =>
            {
                gameControl.isPlayVoice = false;
                if (onComplete != null)
                    onComplete?.Invoke();
            });
        }
        else
        {*/
            if (onComplete != null)
                onComplete?.Invoke();
        //}

    }

    public bool isclick { get; set; }
    public bool OnMove { get => _onMove; set => _onMove = value; }

    private void OnClick(PlayerActionName playerActionName)
    {
        if (!IsSelected) return;
        switch (playerActionName)
        {
            case PlayerActionName.IDLE:
                //playerAnimation.Idle();
                playVoiceAnim("idle");
                break;
            case PlayerActionName.WALK:
                //playerAnimation.Walk();
                playVoiceAnim("WALK");
                break;
            case PlayerActionName.RUN:
                //playerAnimation.Run();
                playVoiceAnim("RUN");
                break;
            case PlayerActionName.DASH:
                playerAnimation.Dash();
                playVoiceAnim("Dash");
                break;
            case PlayerActionName.JUMP:
                playerAnimation.Jump();
                playVoiceAnim("Jump");
                break;
            case PlayerActionName.CRAWL:
                playerAnimation.Crawl();
                playVoiceAnim("Crawl");
                break;
            case PlayerActionName.CLIMB:
                playerAnimation.Climb();
                playVoiceAnim("Climb");
                break;
            case PlayerActionName.FLY:
                //playerAnimation.Fly();
                playVoiceAnim("FLY");
                break;
            case PlayerActionName.POSE:
                playerAnimation.Pose();
                playVoiceAnim("Pose");
                break;
            case PlayerActionName.ATTACK:
                playerAnimation.Attack();
                playVoiceAnim("Attack");
                break;
            case PlayerActionName.HIT:
                playerAnimation.Hit();
                playVoiceAnim("Hit");
                break;
            case PlayerActionName.SHOT:
                playerAnimation.Shot();
                playVoiceAnim("Shot");
                break;
            case PlayerActionName.KICK:
                playerAnimation.Kick();
                playVoiceAnim("Kick");
                break;
            case PlayerActionName.THROW:
                playerAnimation.Throw();
                playVoiceAnim("Throw");
                break;
            case PlayerActionName.SKILL:
                playerAnimation.Skill();
                playVoiceAnim("Skill");
                break;
            case PlayerActionName.BOW:
                playerAnimation.Bow();
                playVoiceAnim("Bow");
                break;
            case PlayerActionName.CLING:
                playerAnimation.Cling();
                playVoiceAnim("Cling");
                break;
            case PlayerActionName.SIT:
                playerAnimation.Sit();
                playVoiceAnim("Sit");
                break;
            default:
                break;
        }
    }


    private void OnHoldDown(PlayerActionName playerActionName)
    {
        if (!IsSelected) return;
        isclick = true;
        switch (playerActionName)
        {
            case PlayerActionName.IDLE:
                break;
            case PlayerActionName.WALK:
                break;
            case PlayerActionName.RUN:
                if (_onMove)
                {
                    playerAnimation.Run();
                }
                break;
            case PlayerActionName.DASH:
                break;
            case PlayerActionName.JUMP:
                break;
            case PlayerActionName.CRAWL:
                break;
            case PlayerActionName.CLIMB:
                break;
            case PlayerActionName.FLY:
                OnHoldFly = true;
                break;
            case PlayerActionName.POSE:
                break;
            case PlayerActionName.ATTACK:
                break;
            case PlayerActionName.HIT:
                break;
            case PlayerActionName.SHOT:
                break;
            case PlayerActionName.KICK:
                break;
            case PlayerActionName.THROW:
                break;
            case PlayerActionName.SKILL:
                break;
            case PlayerActionName.BOW:
                break;
            case PlayerActionName.CLING:
                break;
            case PlayerActionName.SIT:
                break;
            default:
                break;
        }
    }
    private void OnHoldUp(PlayerActionName playerActionName)
    {
        if (!IsSelected) return;
        isclick = false;
        switch (playerActionName)
        {
            case PlayerActionName.IDLE:
                break;
            case PlayerActionName.WALK:
                break;
            case PlayerActionName.RUN:
                playerAnimation.Idle();
                break;
            case PlayerActionName.DASH:
                break;
            case PlayerActionName.JUMP:
                break;
            case PlayerActionName.CRAWL:
                break;
            case PlayerActionName.CLIMB:
                break;
            case PlayerActionName.FLY:
                OnHoldFly = false;
                break;
            case PlayerActionName.POSE:
                break;
            case PlayerActionName.ATTACK:
                break;
            case PlayerActionName.HIT:
                break;
            case PlayerActionName.SHOT:
                break;
            case PlayerActionName.KICK:
                break;
            case PlayerActionName.THROW:
                break;
            case PlayerActionName.SKILL:
                break;
            case PlayerActionName.BOW:
                break;
            case PlayerActionName.CLING:
                break;
            case PlayerActionName.SIT:
                break;
            default:
                break;
        }
    }

    #endregion

}

