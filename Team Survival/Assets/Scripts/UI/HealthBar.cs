using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    private UnitShell _owner;

    [SerializeField]
    private AnimationCurve HealthColorCurve;

    [SerializeField]
    private Color FullHealthColor;

    [SerializeField]
    private Color NearDeathColor;

    [SerializeField]
    private Canvas HealthBarCanvas;

    [SerializeField]
    private Image HealthBackground;

    private void Start() {
        _owner = GetComponentInParent<UnitShell>();
        _owner.EventHandle.OnHealthChanged += UpdateHealthBar;
        _owner.EventHandle.OnLifeStateChanged += UpdateBarVisibility;
    }
	
	// Update is called once per frame
	private void Update () {
        if (Camera.main != null)
            transform.LookAt(Camera.main.transform.position);
	}

    private void UpdateHealthBar(float health)
    {
        float percent = _owner.Health / _owner.MaxHealth;
        float t = HealthColorCurve.Evaluate(percent);

        HealthBackground.color = Color.Lerp(NearDeathColor, FullHealthColor, t);
    }

    private void UpdateBarVisibility(LifeState newState)
    {
        HealthBarCanvas.enabled = (newState == LifeState.Alive);
    }
}
