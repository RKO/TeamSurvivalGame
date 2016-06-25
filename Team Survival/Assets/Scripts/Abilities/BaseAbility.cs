using UnityEngine;

public abstract class BaseAbility {

    protected BaseUnit _caster;
    protected float _cooldown;
    protected float _cooldownCounter;

    public BaseAbility(BaseUnit caster, float cooldown) {
        _caster = caster;
        _cooldown = cooldown;
    }

    public void Update() {
        CalculateCooldown();

        if (_cooldownCounter < 0)
            _cooldownCounter = 0;

        AbilityUpdate();
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
