using UnityEngine;

[System.Serializable]
public class Wave {
    public bool isDone { get; set; }

    public bool isSpawning { get; set; }

    public int UnitCount;

    public GameObject UnitPrefab;

    public UnitData SpawnUnitData;
}
