using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

    private bool isPaused = false;

	void Start () {
	
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused == false)
            {
                isPaused = true;
                Time.timeScale = 0f;
            }
            else
            {
                isPaused = false;
                Time.timeScale = 1f;  
            } 
        }
	
	}
}
