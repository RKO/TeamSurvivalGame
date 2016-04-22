using UnityEngine;

[RequireComponent(typeof(BaseMotor))]
public class UnitController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        BaseMotor motor = GetComponent<BaseMotor>();
        motor.Body.GetComponent<Renderer>().material.color = Color.red;
    }
	
	// Update is called once per frame
	void Update () {
	    
	}
}