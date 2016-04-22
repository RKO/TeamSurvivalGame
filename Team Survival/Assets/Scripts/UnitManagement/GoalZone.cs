using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GoalZone : MonoBehaviour {
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Unit Reached the goal zone!");
        //TODO Register the death at a "UnitManager" script.
        Destroy(other.gameObject);
    }
}
