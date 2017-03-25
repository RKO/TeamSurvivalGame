using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    private UnitShell _owner;

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
    }
	
	// Update is called once per frame
	private void Update () {
        if (Camera.main != null)
            transform.LookAt(Camera.main.transform.position);

        UpdateBar();
	}

    private void UpdateBar() {
        if (_owner == null)
            return;

        if (HealthBarCanvas.enabled && _owner.AliveState != LifeState.Alive)
        {
            HealthBarCanvas.enabled = false;
            return;
        }
        else if (!HealthBarCanvas.enabled && _owner.AliveState == LifeState.Alive)
        {
            HealthBarCanvas.enabled = true;
            return;
        }

        float percent = _owner.Health / _owner.MaxHealth;
        HealthBackground.color = Color.Lerp(NearDeathColor, FullHealthColor, percent);
    }
}
