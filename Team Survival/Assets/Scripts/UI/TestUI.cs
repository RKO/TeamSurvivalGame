using UnityEngine;
using UnityEngine.UI;

public class TestUI : MonoBehaviour {
    private Text _text;


	// Use this for initialization
	void Start () {
        _text = GetComponentInChildren<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        _text.text = "Enemies Left: " + GameManager.Instance.unitManager.GetUnitCount(Team.Enemies);
    }
}
