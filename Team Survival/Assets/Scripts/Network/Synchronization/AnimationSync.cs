using UnityEngine.Networking;

public class AnimationSync : NetworkBehaviour {

    public delegate void OnAnimationTrigger(UnitTriggerAnimation triggerAnim);

    protected OnAnimationTrigger onAnimationTrigger;

    [SyncVar]
    public UnitAnimation CurrentAnimation = UnitAnimation.Idle;

    [ClientRpc]
    public void RpcTriggerAnimation(UnitTriggerAnimation triggerAnim) {
        if (onAnimationTrigger != null)
        {
            onAnimationTrigger(triggerAnim);
        }
    }

    public void SubscribeTriggerAnimation(OnAnimationTrigger callback) {
        onAnimationTrigger += callback;
    }
}
