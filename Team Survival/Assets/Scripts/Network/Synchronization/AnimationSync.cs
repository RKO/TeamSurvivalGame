using UnityEngine.Networking;
using UnityEngine;

public class AnimationSync : NetworkBehaviour {
    private Animation _animation;
    private Animator _unitAnimator;
    private NetworkAnimator _netAnimator;

    [SyncVar]
    private UnitAnimation _currentAnimation = UnitAnimation.Idle;

    private void Start() {
        _animation = GetComponentInChildren<Animation>();
        _unitAnimator = GetComponent<Animator>();
        _netAnimator = GetComponent<NetworkAnimator>();

        if (_animation != null)
            ApplyAnimation(_currentAnimation);
    }

    [Server]
    public void SetNewAnimation(UnitAnimation newAnimation)
    {
        if (newAnimation == _currentAnimation)
            return;

        _currentAnimation = newAnimation;

        if (_animation != null)
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
                _animation.Play("idle");
                break;
            case UnitAnimation.Walking:
                _animation.Play("walk");
                break;
            case UnitAnimation.Running:
                _animation.Play("run");
                break;
            case UnitAnimation.Dying:
            case UnitAnimation.Dead:
                _animation.wrapMode = WrapMode.ClampForever;
                _animation.Play("death");
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
                _unitAnimator.SetInteger("Speed", 0);
                break;
            case UnitAnimation.Walking:
                _unitAnimator.SetInteger("Speed", 1);
                break;
            case UnitAnimation.Running:
                _unitAnimator.SetInteger("Speed", 2);
                break;
            default:
                break;
        }

        _currentAnimation = newAnimation;
    }

    public void TriggerAnimation(UnitTriggerAnimation triggerAnim)
    {
        if (triggerAnim == UnitTriggerAnimation.Jump)
        {
            _netAnimator.SetTrigger("Jump");
        }
        else {
            _netAnimator.SetTrigger("Attack");
        }
    }
}
