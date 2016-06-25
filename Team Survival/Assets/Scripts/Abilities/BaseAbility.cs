using UnityEngine;

public abstract class BaseAbility {

    public abstract string Name { get; }

    protected BaseMotor _caster;
    protected float _cooldown;
    protected float _cooldownCounter;
    public float CooldownPercent { get; private set; }

    public BaseAbility(BaseMotor caster, float cooldown) {
        _caster = caster;
        _cooldown = cooldown;
    }

    public void Update() {
        CalculateCooldown();

        if (_cooldownCounter < 0)
            _cooldownCounter = 0;

        AbilityUpdate();

        CooldownPercent = _cooldownCounter / _cooldown;
    }

    public void Activate() {
        if (_cooldownCounter == 0 && CanActivate())
        {
            DoActivate();
            _cooldownCounter = _cooldown;
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
    protected virtual bool CanActivate() { return true; }
}
