using UnityEngine;

public abstract class BaseAbility {

    protected float _cooldown;
    protected float _cooldownCounter;

    public BaseAbility(float cooldown) {
        _cooldown = cooldown;
    }

    public void Update() {
        if (_cooldownCounter > 0)
        {
            _cooldownCounter -= Time.deltaTime;
            if (_cooldownCounter < 0)
                _cooldownCounter = 0;
        }

        AbilityUpdate();
    }

    public void Activate(BaseUnit caster) {
        if (_cooldownCounter == 0 && CanActivate(caster))
        {
            DoActivate(caster);
            _cooldownCounter = _cooldown;
        }
    }

    protected abstract void DoActivate(BaseUnit caster);
    protected virtual void AbilityUpdate() { }
    protected virtual bool CanActivate(BaseUnit caster) { return true; }
}
