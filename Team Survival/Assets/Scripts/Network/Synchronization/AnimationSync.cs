﻿using UnityEngine.Networking;
using System.Collections;

public class AnimationSync : NetworkBehaviour {

    public enum UnitAnimation { Idle, Walking, Running, Attack1, Attack2 }

    [SyncVar]
    public UnitAnimation CurrentAnimation = UnitAnimation.Idle;
}