using UnityEngine;
using UnityEngine.UI;

public class TestUI : MonoBehaviour {
    private GameManager _gameManager;
    public Text _waveText;
    public Text _enemiesLeftText;
	
    void Start () {
        _gameManager = GameManager.Instance;
    }

	// Update is called once per frame
	void Update () {
        SpawnManager spawnManager = _gameManager.spawnManager;

        int wave = Mathf.Min(spawnManager.CurrentWave + 1, spawnManager.WaveCount);

        _waveText.text = "Wave " + wave + "/" + spawnManager.WaveCount;

        _enemiesLeftText.text = "Enemies Left: " + _gameManager.unitManager.GetUnitCount(Team.Enemies);
    }
}
