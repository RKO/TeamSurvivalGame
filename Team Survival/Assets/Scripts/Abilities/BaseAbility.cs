using UnityEngine;

public abstract class BaseAbility : MonoBehaviour {

    public string GetUniqueID { get { return GetType().FullName; } }

    [SerializeField]
    protected AbilityInfo _abilityInfo;
    [SerializeField]
    protected float cooldown;
    [SerializeField]
    protected float duration;

    protected UnitShell _unit;
    protected float _cooldownCounter;
    protected float _durationCounter;

    public float CooldownPercent { get; private set; }

    public bool IsActive { get { return duration > 0 && _durationCounter > 0; } }

    public bool CanActivate { get { return _cooldownCounter == 0 && !IsActive && CheckCanActivate(); } }

    public void Setup(UnitShell unit) {
        //For AbilityInfoSync, when it creates from reflection.
        if (unit == null)
        {
            Debug.LogWarning("Unit null from somewhere");
            return;
        }

        _unit = unit;
        _durationCounter = 0;

        //Just default to 0, in case there is no cooldown.
        CooldownPercent = 0;

        Initialize();
    }

    protected virtual void Initialize() { }

    public AbilityInfo GetInfo() { 
        return _abilityInfo;
    }

    public void RunUpdate() {
        if (IsActive)
        {
            AbilityUpdate();
            _durationCounter -= Time.deltaTime;
        }
        else {
            CalculateCooldown();

            if (_cooldownCounter < 0)
                _cooldownCounter = 0;
        }

        if(cooldown > 0)
            CooldownPercent = _cooldownCounter / cooldown;
    }

    public void Activate() {
        if (CanActivate)
        {
            DoActivate();
            _cooldownCounter = cooldown;
            _durationCounter = duration;
        }
    }

    protected virtual void CalculateCooldown() {
        if (_cooldownCounter > 0)
        {
            _cooldownCounter -= Time.deltaTime;
        }
    }

    
    protected abstract void DoActivate();
    protected virtual void AbilityUpdate() { }
    protected virtual bool CheckCanActivate() { return true; }

}
