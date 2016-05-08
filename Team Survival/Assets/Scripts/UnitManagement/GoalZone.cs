using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GoalZone : MonoBehaviour {
    void OnTriggerEnter(Collider other)
    {
        //Only destroy Enemies.
        IUnit unit = other.gameObject.GetComponent<IUnit>();
        if (unit != null && unit.GetTeam == Team.Enemies)
        {
            //TODO Remove a life from players.
            //Death is automatically registered by BaseUnit scripts.
            Destroy(other.gameObject);
        }
    }
}
