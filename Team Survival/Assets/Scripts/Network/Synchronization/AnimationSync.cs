using UnityEngine.Networking;
using UnityEngine;

public class AnimationSync : NetworkBehaviour {
    private Animation _legacyAnimator;
    private Animator _mechanimAnimator;
    //private NetworkAnimator _netAnimator;

    [SyncVar]
    private UnitAnimation _currentAnimation = UnitAnimation.Idle;

    private void Start() {
        _legacyAnimator = GetComponentInChildren<Animation>();
        _mechanimAnimator = GetComponentInChildren<Animator>();

        if (_legacyAnimator != null)
            ApplyAnimation(_currentAnimation);
    }

    [Server]
    public void SetNewAnimation(UnitAnimation newAnimation)
    {
        if (newAnimation == _currentAnimation)
            return;

        _currentAnimation = newAnimation;

        //if (_legacyAnimator != null)
            RpcSetNewAnimation(newAnimation);
        /*else
            ApplyMechanim(newAnimation);*/
    }

    [ClientRpc]
    private void RpcSetNewAnimation(UnitAnimation newAnimation) {
        if (_legacyAnimator != null)
            ApplyAnimation(newAnimation);
        else
            ApplyMechanim(newAnimation);
    }

    private void ApplyAnimation(UnitAnimation newAnimation) {
        switch (newAnimation)
        {
            case UnitAnimation.Idle:
                //Let the Idle be the default clip, and then don't set it. 
                //(Otherwise, it will be set right after any triggered animations, and override them)
                break;
            case UnitAnimation.Walking:
                _legacyAnimator.Play("walk");
                break;
            case UnitAnimation.Running:
                _legacyAnimator.Play("run");
                break;
            case UnitAnimation.Dying:
            case UnitAnimation.Dead:
                _legacyAnimator.wrapMode = WrapMode.ClampForever;
                _legacyAnimator.Play("death");
                break;
            default:
                break;
        }

        _currentAnimation = newAnimation;
    }

    private void ApplyMechanim(UnitAnimation newAnimation)
    {
        switch (newAnimation)
        {
            case UnitAnimation.Idle:
                _mechanimAnimator.SetInteger("Speed", 0);
                break;
            case UnitAnimation.Walking:
                _mechanimAnimator.SetInteger("Speed", 1);
                break;
            case UnitAnimation.Running:
                _mechanimAnimator.SetInteger("Speed", 2);
                break;
            case UnitAnimation.Dying:
                _mechanimAnimator.SetTrigger("Death");
                break;
            case UnitAnimation.Dead:
            default:
                break;
        }

        _currentAnimation = newAnimation;
    }

    [Server]
    public void TriggerAnimation(UnitTriggerAnimation triggerAnim)
    {
        RpcTriggerAnimation(triggerAnim);
    }

    [ClientRpc]
    private void RpcTriggerAnimation(UnitTriggerAnimation triggerAnim)
    {
        if (_legacyAnimator == null)
        {
            TriggerMechanimAnimation(triggerAnim);
            return;
        }

        if (triggerAnim == UnitTriggerAnimation.Jump)
        {
            _legacyAnimator.Play("jump");
        }
        else {
            _legacyAnimator.Play("attack", PlayMode.StopAll);
        }
    }

    private void TriggerMechanimAnimation(UnitTriggerAnimation triggerAnim)
    {
        if (triggerAnim == UnitTriggerAnimation.Jump)
        {
            _mechanimAnimator.SetTrigger("Jump");
            //_netAnimator.SetTrigger("Jump");
        }
        else {
            _mechanimAnimator.SetTrigger("Attack");
            //_netAnimator.SetTrigger("Attack");
        }
    }
}
