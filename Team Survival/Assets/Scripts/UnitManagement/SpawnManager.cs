using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
    private SpawnPoint[] spawnPoints;
    private WaveConfig waveConfig;

    public bool IsRunning { get; private set; }

    private int currentWave = 0;

    // Use this for initialization
    void Start () {
        spawnPoints = FindObjectsOfType<SpawnPoint>();
        if (spawnPoints == null || spawnPoints.Length == 0)
            Debug.LogWarning("No spawn points found.");

        waveConfig = FindObjectOfType<WaveConfig>();
        if (waveConfig == null)
            Debug.LogWarning("No WaveConfig found.");
    }

    public void StartSpawning() {
        IsRunning = true;
    }

    public void PauseSpawning() {
        IsRunning = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (IsRunning) {
            StartNextWave();
        }
	}

    private void StartNextWave() {
        //No waves configured.
        if (waveConfig.waves.Count == 0)
        {
            Debug.LogWarning("No waves to spawn!");
            return;
        }

        //No more waves.
        if (currentWave >= waveConfig.waves.Count) {
            GameManager.Instance.DisplayGlobalMessage("The last wave has been defeated!");
            IsRunning = false;
            return;
        }


        Wave wave = waveConfig.waves[currentWave];
        //Current wave is finished, pick the next one.
        if (wave.isDone) {
            currentWave++;
            //TODO Wait time between waves.
            return;
        }

        //If the current wave has not been started, start it.
        if (!wave.isSpawning) {
            wave.isSpawning = true;
            GameManager.Instance.DisplayGlobalMessage("Starting wave "+(currentWave+1)+"/"+ waveConfig.waves.Count);

            StartCoroutine(SpawnSingleWave(wave));
        }
    }

    private IEnumerator SpawnSingleWave(Wave wave) {
        for (int i = 0; i < wave.UnitCount; i++)
        {
            foreach (SpawnPoint spawnPoint in spawnPoints)
            {
                Vector3 spawnPosition = spawnPoint.transform.position;
                if (spawnPoint.HasSpawnArea)
                {
                    spawnPosition = spawnPoint.spawnBox.GetRandomPointInCollider();
                }

                Instantiate(wave.UnitPrefab, spawnPosition, spawnPoint.transform.rotation);
            }

            yield return new WaitForSeconds(1);
        }

        wave.isDone = true;
    }
}
