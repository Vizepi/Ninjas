using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnPlayer : MonoBehaviour {

    //Spawn Ennemy
    public static Gamecontrolleur GameControl;
    public float Distance = 15.0F;
    public GameObject[] Spawn_ennemies;
    public GameObject[] ennemy_type;


    void Start() {
        GameControl = GameObject.FindGameObjectWithTag("GameController").GetComponent<Gamecontrolleur>();

    }

    // Update is called once per frame
    void Update() {

    }
    private static List<Transform> Spawn_ennemie = new List<Transform>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "SpawnEnnemy")
        {
            AddSpawn(other.transform);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
            if (other.tag == "SpawnEnnemy")
            {
                RemoveSpawn(other.transform);
            }
        }


    public static void AddSpawn(Transform spawn){
        if (!Spawn_ennemie.Contains(spawn)){
            Spawn_ennemie.Add(spawn);
            Debug.Log("Ajouter spawn");
        }
    }

    public static void RemoveSpawn(Transform spawn){
            Spawn_ennemie.Remove(spawn);
        Debug.Log("Retirer Spawn");
    }
   
    public static void Spawn_near_player(){
        if(Spawn_ennemie.Count <= 0) {
            return;
        }
        int rand_spawn = Random.Range(0, Spawn_ennemie.Count);
        Debug.Log(Spawn_ennemie.Count);
        Debug.Log(rand_spawn);
        Debug.Log(GameControl);
        Debug.Log(GameControl.ennemy_type[0]);
        Debug.Log(Spawn_ennemie[rand_spawn].position);
        Instantiate(GameControl.ennemy_type[0], Spawn_ennemie[rand_spawn].position, Quaternion.identity);
        Debug.Log("Instance");
    }
}