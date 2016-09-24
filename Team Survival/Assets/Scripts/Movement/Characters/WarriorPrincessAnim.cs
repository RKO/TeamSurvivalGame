using UnityEngine;

public class WarriorPrincessAnim : MonoBehaviour
{
    private const int WeaponState = 8; //1h weapon + shield = 8

    private Animator _animator;
    private AnimationSync _animSync;

	// Use this for initialization
	void Start () {
        _animator = GetComponent<Animator>();
        _animator.SetInteger("WeaponState", WeaponState);

        _animator.SetBool("NonCombat", false);
        _animator.SetBool("Idling", true);

        _animSync = GetComponentInParent<AnimationSync>();

        _animSync.SubscribeTriggerAnimation(OnAnimationTriggered);
    }

    // Update is called once per frame
    void Update () {

        bool idle = false;
        bool combat = true;

        UnitAnimation CurrentAnimation = _animSync.CurrentAnimation;

        switch (CurrentAnimation)
        {
            case UnitAnimation.Idle:
                idle = true;
                break;
            case UnitAnimation.Walking:
                combat = false;
                break;
            case UnitAnimation.Running:
                combat = true;
                break;
            default:
                break;
        }

        _animator.SetBool("NonCombat", !combat);
        _animator.SetBool("Idling", idle);
    }

    private void OnAnimationTriggered(UnitTriggerAnimation triggerAnim)
    {
        if(triggerAnim == UnitTriggerAnimation.Jump)
            _animator.SetTrigger("Jump");
        else
            _animator.SetTrigger("Use");
    }
}
