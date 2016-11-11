using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum CatchPhrase {
    None = 0,
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4
}

public class RandomThought : MonoBehaviour {

    public GUIText text;
    private CatchPhrase _choosenOne = CatchPhrase.None;
    private string phraseToPut;

    void Start()
    {
        _choosenOne = (CatchPhrase)Random.Range(0, 4) + 1;
        PickUpLine(_choosenOne);
        SetTextOnScreen();
        StartCoroutine(Wait(4));
    }
	
	// Update is called once per frame
	void Update () {
	}

    private void PickUpLine(CatchPhrase phrase)    {
        switch (phrase)
            {
                case CatchPhrase.None:
                default:
                Debug.LogWarning("CatchPhrase: non géré " + phrase);
                    break;
               
            case CatchPhrase.One:
                phraseToPut = "Toutes les villes ne sont-elles pas des villes de ninjas";
                break;

            case CatchPhrase.Two:
                phraseToPut = "Si la lumiere est si rapide, comment se fait-il qu'elle ne rattrape jamais un ninja";
                break;

            case CatchPhrase.Three:
                phraseToPut = "Un ninja n'a de raison d'être que s'il n'existe pas";
                break;

            case CatchPhrase.Four:
                phraseToPut = "Les pets c'est comme les ninja, plus ils sont silencieux, plus ils font mals";
                break;
            }
        }

    private void SetTextOnScreen(){
        text.text = phraseToPut;
    }

    IEnumerator Wait (int nb){
        yield return new WaitForSeconds(nb);
        SceneManager.LoadScene("main");
    }
}
