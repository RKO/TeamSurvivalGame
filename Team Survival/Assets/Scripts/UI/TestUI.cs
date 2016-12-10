using UnityEngine;
using UnityEngine.UI;

public class TestUI : MonoBehaviour {
    private GameManager _gameManager;
    private SpawnManager _spawnManager;
    public Text _waveText;
    public Text _enemiesLeftText;

	// Update is called once per frame
	void Update () {
        if (GameManager.Instance == null)
            return;

        if (_spawnManager == null)
        {
            _gameManager = GameManager.Instance;
            _spawnManager = _gameManager.spawnManager;
        }

        int wave = Mathf.Min(_spawnManager.CurrentWave + 1, _spawnManager.WaveCount);

        _waveText.text = "Wave " + wave + "/" + _spawnManager.WaveCount;

        _enemiesLeftText.text = "Enemies Left: " + _gameManager.unitManager.GetUnitCount(Team.Enemies);
    }
}
