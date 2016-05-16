using UnityEngine;
using UnityEngine.Networking;

public class SpawnManagerNetwork : MonoBehaviour {

	// Use this for initialization
	void Start () {
        WaveConfig waveConfig = GetComponent<SpawnManager>().WaveCfg;

        foreach (Wave wave in waveConfig.waves)
        {
            ClientScene.RegisterPrefab(wave.UnitPrefab);
        }
	}
}
