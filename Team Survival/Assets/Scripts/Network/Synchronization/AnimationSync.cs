using UnityEngine.Networking;
using UnityEngine;

public class AnimationSync : NetworkBehaviour {
    private Animation _legacyAnimator;
    private Animator _mechanimAnimator;
    private NetworkAnimator _netAnimator;

    [SyncVar]
    private UnitAnimation _currentAnimation = UnitAnimation.Idle;

    private void Start() {
        _legacyAnimator = GetComponentInChildren<Animation>();
        _mechanimAnimator = GetComponent<Animator>();
        _netAnimator = GetComponent<NetworkAnimator>();

        if (_legacyAnimator != null)
            ApplyAnimation(_currentAnimation);
    }

    [Server]
    public void SetNewAnimation(UnitAnimation newAnimation)
    {
        if (newAnimation == _currentAnimation)
            return;

        _currentAnimation = newAnimation;

        if (_legacyAnimator != null)
            RpcSetNewAnimation(newAnimation);
        else
            ApplyMechanim(newAnimation);
    }

    [ClientRpc]
    private void RpcSetNewAnimation(UnitAnimation newAnimation) {
        ApplyAnimation(newAnimation);
    }

    private void ApplyAnimation(UnitAnimation newAnimation) {
        switch (newAnimation)
        {
            case UnitAnimation.Idle:
                _legacyAnimator.Play("idle");
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
                _netAnimator.SetTrigger("Death");
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
        if (_legacyAnimator != null)
        {
            RpcTriggerAnimation(triggerAnim);
        }
        else
        {
            TriggerMechanimAnimation(triggerAnim);
        }
    }

    [ClientRpc]
    private void RpcTriggerAnimation(UnitTriggerAnimation triggerAnim) {
        if (triggerAnim == UnitTriggerAnimation.Jump)
        {
            _legacyAnimator.Play("jump");
        }
        else {
            _legacyAnimator.Play("attack");
        }
    }

    private void TriggerMechanimAnimation(UnitTriggerAnimation triggerAnim) {
        if (triggerAnim == UnitTriggerAnimation.Jump)
        {
            _netAnimator.SetTrigger("Jump");
        }
        else {
            _netAnimator.SetTrigger("Attack");
        }
    }
}
