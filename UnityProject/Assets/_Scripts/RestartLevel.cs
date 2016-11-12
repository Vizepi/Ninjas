using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RestartLevel : MonoBehaviour {

    public string levelName;

	// Use this for initialization
	void Start () {
        StartCoroutine(Wait(6));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator Wait (int nb){
        yield return new WaitForSeconds(nb);
        SceneManager.LoadScene(levelName);
    }
}
