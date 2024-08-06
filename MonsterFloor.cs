using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson; // this line may need to be deleted
// using... ; // for just in case

// IMPORTANT: When applying this file on a floor, please remeber to open the component in the inspector and add the prefabs for the monsters!
public class MonsterFloor : MonoBehaviour {
    public float length; // MUST be defined in the inspector
    public float width; // MUST be defined in the inspector
    private long timer = 0;
    private float x;
    private float y;
    private float z;
    private bool hasSummoned = false;
    private bool hasActivated = false;
    public GameObject[] prefabs;
    public AudioSource scareAudio;
    public AudioSource[] ambiances;

    void Start() {
        scareAudio.Stop();
        foreach (AudioSource audSrc in ambiances) {
            if (audSrc != null) {
                audSrc.Stop();
            }
        }
    }

    public void PlayerTouch() {
        Vector3 pos = gameObject.transform.position;
        x = pos.x;
        y = pos.y + 0.5f;
        z = pos.z;
        hasActivated = true;
    }

    void Update() {
        if (hasActivated) {
            if (timer >= 2000 && !hasSummoned) {
                int max = prefabs.Length - 1;
                int val = Random.Range(0, max);
                // the monster does need the script attached, please remember to do that also in the code
                GameObject monster = Instantiate(prefabs[val], new Vector3(x, y, z), Quaternion.identity);
                monster.AddComponent<Monster>();
                Monster comp = monster.GetComponent<Monster>();
                comp.DefineScareAudio(scareAudio);
                comp.DefineAmbiantAudios(ambiances);
                hasSummoned = true;
            } else {
                timer++;
            }
        } else {
            Collider[] colliders = Physics.OverlapBox(gameObject.transform.position, new Vector3(length, 5, width));
            foreach (Collider collider in colliders) {
                if (collider.gameObject.GetComponent<MonoBehaviour>() is /*modify*/ FirstPersonController /*just incase*/) {
                    // the class name must match with the script used for the fps controller (including the using followed by its namespace) 
                    PlayerTouch();
                }
            }
        }
    }
}