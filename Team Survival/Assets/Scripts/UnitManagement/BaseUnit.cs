using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    [SerializeField]
    private float _unitMoveSpeed;
    public float MoveSpeed { get { return _unitMoveSpeed; } }

    [SerializeField]
    private Team _defaultTeam;
    //TODO Rename!
    public Team DefaultTeam { get { return _defaultTeam; } }

    [SerializeField]
    private string _unitName;
    public string UnitName { get { return _unitName; } }

    [SerializeField]
    private int _maxHealth;
    public int MaxHealth { get { return _maxHealth; } }
}
