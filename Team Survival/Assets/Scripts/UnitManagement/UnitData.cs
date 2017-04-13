using UnityEngine;
using System.Collections;

public class UnitData : ScriptableObject {

    [SerializeField]
    private string _uniqueID;
    public string UnitID { get { return _uniqueID; } }

    [SerializeField]
    private float _unitMoveSpeed;
    public float MoveSpeed { get { return _unitMoveSpeed; } }

    [SerializeField]
    private Team _defaultTeam;
    public Team DefaultTeam { get { return _defaultTeam; } }

    [SerializeField]
    private string _unitName;
    public string UnitName { get { return _unitName; } }

    [SerializeField]
    private int _maxHealth;
    public int MaxHealth { get { return _maxHealth; } }

    [SerializeField]
    private GameObject _model;
    public GameObject Model { get { return _model; } }
}
