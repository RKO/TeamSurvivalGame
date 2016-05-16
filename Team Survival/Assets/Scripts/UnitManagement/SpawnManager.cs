using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
    private SpawnPoint[] spawnPoints;
    private WaveConfig waveConfig;
    private Transform unitsTrans;

    public bool IsRunning { get; private set; }

    public int CurrentWave { get; private set; }

    public int WaveCount { get { return waveConfig.waves.Count; } }

    public WaveConfig WaveCfg { get { return waveConfig; } }

    // Use this for initialization
    void Start () {
        spawnPoints = FindObjectsOfType<SpawnPoint>();
        if (spawnPoints == null || spawnPoints.Length == 0)
            Debug.LogWarning("No spawn points found.");

        waveConfig = FindObjectOfType<WaveConfig>();
        if (waveConfig == null)
            Debug.LogWarning("No WaveConfig found.");

        unitsTrans = new GameObject("Units").transform;
        unitsTrans.SetParent(this.transform, false);

        this.gameObject.AddComponent<SpawnManagerNetwork>();
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
        if (CurrentWave >= waveConfig.waves.Count) {
            GameManager.Instance.DisplayGlobalMessage("The last wave has spawned!");
            IsRunning = false;
            return;
        }


        Wave wave = waveConfig.waves[CurrentWave];
        //Current wave is finished, pick the next one.
        if (wave.isDone) {
            CurrentWave++;
            //TODO Wait time between waves.
            return;
        }

        //If the current wave has not been started, start it.
        if (!wave.isSpawning) {
            wave.isSpawning = true;
            GameManager.Instance.DisplayGlobalMessage("Starting wave "+(CurrentWave+1)+"/"+ waveConfig.waves.Count);

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

                GameObject unit = Instantiate(wave.UnitPrefab, spawnPosition, spawnPoint.transform.rotation) as GameObject;
                unit.transform.SetParent(unitsTrans, true);

                UnitController controller = unit.GetComponent<UnitController>();
                controller.SetPathWaypoints(spawnPoint.waypoints);
            }

            yield return new WaitForSeconds(1);
        }

        wave.isDone = true;
    }
}
