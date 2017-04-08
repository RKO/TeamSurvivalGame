using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GoalZone : MonoBehaviour {
    void OnTriggerEnter(Collider other)
    {
        //Only destroy Enemies.
        UnitShell unit = other.gameObject.GetComponent<UnitShell>();
        if (unit != null && unit.CurrentTeam == Team.Enemies)
        {
            //TODO Remove a life from players.
            //Death is automatically registered by BaseUnit scripts.
            Destroy(other.gameObject);
        }
    }
}
