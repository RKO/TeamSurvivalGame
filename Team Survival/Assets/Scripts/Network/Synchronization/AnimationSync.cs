using UnityEngine.Networking;
using UnityEngine;

public class AnimationSync : NetworkBehaviour {
    private Animation _animation;

    [SyncVar]
    private UnitAnimation _currentAnimation = UnitAnimation.Idle;

    private void Start() {
        _animation = GetComponentInChildren<Animation>();
        ApplyAnimation(_currentAnimation);
    }

    [Server]
    public void SetNewAnimation(UnitAnimation newAnimation)
    {
        if (newAnimation == _currentAnimation)
            return;

        _currentAnimation = newAnimation;
        RpcSetNewAnimation(newAnimation);
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
}
