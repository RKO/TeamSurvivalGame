using UnityEngine;

public abstract class BaseAbility {

    public abstract string Name { get; }

    protected BaseMotor _caster;
    protected BaseUnit _unit;
    protected float _cooldown;
    protected float _cooldownCounter;
    protected float _duration;
    protected float _durationCounter;

    public float CooldownPercent { get; private set; }

    public bool IsActive { get { return _duration > 0 && _durationCounter < _duration; } }

    public bool CanActivate { get { return _cooldownCounter == 0 && !IsActive && CheckCanActivate(); } }

    public BaseAbility(BaseMotor caster, BaseUnit unit, float cooldown, float duration = 0) {
        _caster = caster;
        _unit = unit;
        _cooldown = cooldown;
        _duration = duration;

        //Just default to 0, in case there is no cooldown.
        CooldownPercent = 0;
    }

    public void Update() {
        if (IsActive)
        {
            AbilityUpdate();
            _durationCounter += Time.deltaTime;
        }
        else {
            CalculateCooldown();

            if (_cooldownCounter < 0)
                _cooldownCounter = 0;
        }

        if(_cooldown > 0)
            CooldownPercent = _cooldownCounter / _cooldown;
    }

    public void Activate() {
        if (CanActivate)
        {
            DoActivate();
            _cooldownCounter = _cooldown;
            _durationCounter = 0;
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
