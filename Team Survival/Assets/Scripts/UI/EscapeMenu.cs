using UnityEngine;
using System.Collections;

public class EscapeMenu : MonoBehaviour {
    public GameObject _menu;
    public GameObject _background;
    private bool _isVisible = false;

	// Use this for initialization
	void Start () {
        _isVisible = false;
        SetMenuVisible(_isVisible);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetMenuVisible(!_isVisible);
        }
	}

    private void SetMenuVisible(bool visible) {
        _isVisible = visible;
        _menu.SetActive(_isVisible);
        _background.SetActive(_isVisible);

        Cursor.visible = !_isVisible;
        if (!_isVisible)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        GameManager.Instance.IsGUIOpen = _isVisible;
    }

    public void OnResumePressed() {
        SetMenuVisible(false);
    }

    public void OnExitPressed()
    {
        SetMenuVisible(false);
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
