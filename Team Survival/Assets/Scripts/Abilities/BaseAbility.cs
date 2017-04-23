using UnityEngine;

public abstract class BaseAbility : MonoBehaviour {

    public string UniqueID { get { return GetType().FullName; } }

    [SerializeField]
    public string DisplayName;
    [SerializeField]
    public string Description;
    [SerializeField]
    public Sprite Icon;
    [SerializeField]
    public Texture2D TempIcon;
    [SerializeField]
    public AbilitySlot Slot;
    [SerializeField]
    protected float Cooldown;
    [SerializeField]
    protected float Duration;

    protected UnitShell _unit;
    protected float _cooldownCounter;
    protected float _durationCounter;

    public float CooldownPercent { get; private set; }

    public bool IsActive { get { return Duration > 0 && _durationCounter > 0; } }

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

        if(Cooldown > 0)
            CooldownPercent = _cooldownCounter / Cooldown;
    }

    public void Activate() {
        if (CanActivate)
        {
            DoActivate();
            _cooldownCounter = Cooldown;
            _durationCounter = Duration;
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
