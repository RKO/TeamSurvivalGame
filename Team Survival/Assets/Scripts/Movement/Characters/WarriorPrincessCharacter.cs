using UnityEngine.Networking;

public class WarriorPrincessCharacter : BaseUnit
{
    private const int WeaponState = 8; //1h weapon + shield = 8

    public override Team GetTeam
    {
        get { return Team.Players; }
    }

    private string _name = "Player";
    public override string Name { get { return _name; } }

    private NetworkAnimator netAnim;

    private UnitAnimation _currentAnimation;

    // Use this for initialization
    void Start () {
        UnitAnimator.SetInteger("WeaponState", WeaponState);

        UnitAnimator.SetBool("NonCombat", false);
        UnitAnimator.SetBool("Idling", true);

        SetNewAnimation(UnitAnimation.Idle);

        netAnim = GetComponent<NetworkAnimator>();
    }


    public override void SetNewAnimation(UnitAnimation newAnimation) {
        if (newAnimation == _currentAnimation)
            return;

        bool idle = false;
        bool combat = true;

        switch (newAnimation)
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

        UnitAnimator.SetBool("NonCombat", !combat);
        UnitAnimator.SetBool("Idling", idle);

        _currentAnimation = newAnimation;
    }

    public override void TriggerAnimation(UnitTriggerAnimation triggerAnim)
    {
        if (triggerAnim == UnitTriggerAnimation.Jump)
        {
            //UnitAnimator.SetTrigger("Jump");
            netAnim.SetTrigger("Jump");
        }
        else {
            //UnitAnimator.SetTrigger("Use");
            netAnim.SetTrigger("Use");
        }
    }
}
