﻿using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnManager : NetworkBehaviour {
    private SpawnPoint[] spawnPoints;
    private WaveConfig waveConfig;
    private Transform unitsTrans;

    public bool IsRunning { get; private set; }

    [SyncVar]
    private int _currentWave;
    public int CurrentWave { get { return _currentWave; } }

    public int WaveCount {
        get {
            if (waveConfig == null)
                return -1;
            return waveConfig.waves.Count;
        }
    }

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
    }

    public void StartSpawning() {
        IsRunning = true;
    }

    public void PauseSpawning() {
        IsRunning = false;
    }
	
	// Update is called once per frame
    [ServerCallback]
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
            _currentWave++;
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

                UnitShell shell = unit.GetComponent<UnitShell>();
                shell.UnitPrefabToLoad = wave.PrefabToLoad;

                //TODO Hardcoded way of giving orders...
                shell.waypoints = spawnPoint.waypoints;

                NetworkServer.Spawn(unit);
            }

            yield return new WaitForSeconds(1);
        }

        wave.isDone = true;
    }
}
