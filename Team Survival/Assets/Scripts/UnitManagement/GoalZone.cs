using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GoalZone : MonoBehaviour {
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Unit Reached the goal zone!");
        //TODO Register the death at a "UnitManager" script.

        //Don't destroy players. 
        //TODO Add a way to identify "teams".
        if(other.gameObject.GetComponent<UnitController>() != null)
            Destroy(other.gameObject);
    }
}
