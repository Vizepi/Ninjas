using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RestartLevel : MonoBehaviour {

    public string levelName;
    public int nbWait;

	// Use this for initialization
	void Start () {
        StartCoroutine(Wait(nbWait));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator Wait (int nb){
        yield return new WaitForSeconds(nb);
        SceneManager.LoadScene(levelName);
    }
}
