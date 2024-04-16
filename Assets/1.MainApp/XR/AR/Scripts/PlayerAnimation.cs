using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour, IPlayer
{
    public Animator animator;
    public List<AnimationClip> animationClips;
    [Header("Config Animation")]
    public float flySpeed;
    public float walkSpeed;
    public float runSpeed;
    public float jumpSpeed;
    public float hightJump;
    public bool useTimeJump;
    public float timeJump;


    private string nameStateCurrent;
    private string nameStatePrevious;
    private bool _isPlaying = false;

    private ObjectAR _arObject;

    public void Init(ObjectAR obj)
    {
        _arObject = obj;
    }

    public bool IsPlaying()
    {
        return _isPlaying;
    }
    public bool IsPlaying(string clipName)
    {
        return (_isPlaying && nameStateCurrent == clipName);
    }

    public bool IsNotPlaying(string clipName)
    {
        return (_isPlaying && nameStateCurrent != clipName);
    }

    public void Play(string nameClip, float speed = 1, Action callBack = null)
    {
        AnimationClip ac = GetClipAnimation(nameClip);
        if (nameClip == "Walk" && ac == null)
        {
            ac = GetClipAnimation("Fly");
            if (ac != null && nameStateCurrent == "Fly") return;
        }

        if (ac != null)
        {
            _isPlaying = true;
            if (nameStateCurrent == null)
                nameStatePrevious = nameClip;
            else
                nameStatePrevious = nameStateCurrent;
            nameStateCurrent = nameClip;
            animator.SetTrigger(nameClip);
            animator.speed = speed;
            TweenControl.GetInstance().DelayCall(transform, ac.length, () =>
            {
                if (!ac.isLooping)
                {
                    _isPlaying = false;
                }
                callBack?.Invoke();
            });
        }
        else
        {
            Debug.LogWarning("Clip " + nameClip + " not exist in list animationClips");
        }
    }
    public void PlayByTime(string nameClip, float timePlay, Action callBack = null)
    {
        AnimationClip ac = GetClipAnimation(nameClip);
        if (ac != null)
        {
            _isPlaying = true;
            if (nameStateCurrent == null)
                nameStatePrevious = nameClip;
            else
                nameStatePrevious = nameStateCurrent;
            nameStateCurrent = nameClip;
            animator.SetTrigger(nameClip);
            animator.speed = ac.length / timePlay;
            TweenControl.GetInstance().DelayCall(transform, timePlay, () =>
            {
                if (!ac.isLooping)
                {
                    _isPlaying = false;
                }
                callBack?.Invoke();
            });
        }
        else
        {
            Debug.LogWarning("Clip " + nameClip + " not exist in list animationClips");
        }
    }


    public AnimationClip GetClipAnimation(string nameClip)
    {
        for (int i = 0; i < animationClips.Count; i++)
        {
            if (animationClips[i].name.ToLower() == nameClip.ToLower())
            {
                return animationClips[i];
            }
        }
        return null;
    }

    public void Move()
    {
        if (nameStateCurrent == "Run")
        {
            Run();
        }
        else
        {
            Walk();
        }
    }

    public void Walk()
    {
        if (nameStateCurrent != "Walk")
        {
            _arObject.MoveSpeed = walkSpeed;
            Play("Walk");
        }
    }
    public void Idle()
    {
        if (nameStateCurrent != "Idle" && nameStateCurrent != "Sit")
        {
            _arObject.MoveSpeed = walkSpeed;
            Play("Idle");
        }
    }
    public void Run()
    {
        _arObject.MoveSpeed = runSpeed;
        if (nameStateCurrent != "Run")
        {
            Play("Run");
        }
    }

    public void Dash()
    {
        if (nameStateCurrent != "Dash")
        {
            Play("Dash");
        }
    }

    public void Jump()
    {
        if (nameStateCurrent != "Jump")
        {
            _arObject.EnableJoystick = false;
            var clip = GetClipAnimation("Jump");
            if (clip)
            {
                if (useTimeJump)
                {
                    MoveJump(timeJump);
                    PlayByTime("Jump", timeJump, null);
                }
                else
                {
                    MoveJump(clip.length);
                    Play("Jump");
                }
            }
            else
            {
                Debug.LogWarning("Clip Jump missing");
            }
        }

    }
    private void MoveJump(float timeMove)
    {
        Vector3 v = _arObject.rb.velocity;
        _arObject.rb.useGravity = false;
        float hightJumpNew = transform.position.y + hightJump / 2f;
        TweenControl.GetInstance().ValueTo(transform, (param) =>
        {
            _arObject.rb.velocity = v * jumpSpeed;
            _arObject.rb.MovePosition(new Vector3(transform.position.x, param, transform.position.z));

        }, transform.position.y, hightJumpNew, timeMove / 2f, () =>
        {
            _arObject.rb.useGravity = true;
            TweenControl.GetInstance().ValueTo(transform, (param) =>
            {
                //_arObject.rb.MovePosition(new Vector3(transform.position.x, param, transform.position.z));
            }, hightJumpNew, 0, timeMove / 2f, () =>
            {
                _arObject.EnableJoystick = true;

            });
        });
    }

    public void Crawl()
    {
        if (nameStateCurrent != "Crawl")
        {
            Play("Crawl");
        }
    }

    public void Climb()
    {
        if (nameStateCurrent != "Climb")
        {
            Play("Climb");
        }
    }

    public void Fly()
    {
        if (nameStateCurrent != "Fly")
        {
            Play("Fly");
        }
    }

    public void Pose()
    {
        if (nameStateCurrent != "Pose")
        {
            Play("Pose");
        }
    }

    public void Attack()
    {
        if (nameStateCurrent != "Attack")
        {
            Play("Attack");
        }
    }

    public void Hit()
    {
        if (nameStateCurrent != "Hit")
        {
            Play("Hit");
        }
    }

    public void Shot()
    {
        if (nameStateCurrent != "Shot")
        {
            Play("Shot");
        }
    }

    public void Kick()
    {
        if (nameStateCurrent != "Kick")
        {
            Play("Kick");
        }
    }

    public void Throw()
    {
        if (nameStateCurrent != "Throw")
        {
            Play("Throw");
        }
    }

    public void Skill()
    {
        if (nameStateCurrent != "Skill")
        {
            Play("Skill");
        }
    }

    public void Bow()
    {
        if (nameStateCurrent != "Bow")
        {
            Play("Bow");
        }
    }

    public void Cling()
    {
        if (nameStateCurrent != "Cling")
        {
            Play("Cling");
        }
    }

    public void Sit()
    {
        if (nameStateCurrent != "Sit")
        {
            Play("Sit");
        }
    }
}
