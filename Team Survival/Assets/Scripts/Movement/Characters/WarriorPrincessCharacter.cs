﻿using UnityEngine;
using System.Collections.Generic;

public class WarriorPrincessCharacter : BaseUnit
{
    private const int WeaponState = 8; //1h weapon + shield = 8

    //private Animator _animator;
    //private AnimationSync _animSync;

    public override Team GetTeam
    {
        get { return Team.Players; }
    }

    private string _name = "Player";
    public override string Name { get { return _name; } }

    public Dictionary<UnitTriggerAnimation, string> AnimationMap = new Dictionary<UnitTriggerAnimation, string>();

    // Use this for initialization
    void Start () {
        //UnitAnimator = GetComponent<Animator>();
        UnitAnimator.SetInteger("WeaponState", WeaponState);

        UnitAnimator.SetBool("NonCombat", false);
        UnitAnimator.SetBool("Idling", true);

        //_animSync = GetComponentInParent<AnimationSync>();

        //_animSync.SubscribeTriggerAnimation(OnAnimationTriggered);
    }

    // Update is called once per frame
    void Update () {

        bool idle = false;
        bool combat = true;

        //UnitAnimation CurrentAnimation = _animSync.CurrentAnimation;

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

        UnitAnimator.SetBool("NonCombat", !combat);
        UnitAnimator.SetBool("Idling", idle);
    }

    public override void TriggerAnimation(UnitTriggerAnimation triggerAnim)
    {
        if (triggerAnim == UnitTriggerAnimation.Jump)
            UnitAnimator.SetTrigger("Jump");
        else
            UnitAnimator.SetTrigger("Use");
    }
}