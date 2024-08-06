using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson; // this line may be deleted
// using... ; // for just in case


// NO VARS IN THIS CLASS SHOULD BE DEFINED BY INSPECTOR
public class Monster : MonoBehaviour {
    public bool playerNearby; // used for scare animation
    public float chaseRadius = 10; // redefine if needed
    public float scareRadius = 1; // redefine if needed

    private AudioSource[] ambAudio;

    private AudioSource scareAudio;

    public bool chaseInitiated = false; // needs to be attached to one of the monster movement anims
    private int count = 0;

    private const float monsterSpeed = 0.05f; // change if needed
    private GameObject playerObj;

    void Start() {
        playerNearby = false;
    }

    public void DefineScareAudio(AudioSource scareAudio) {
        scareAudio.Stop();
        this.scareAudio = scareAudio;
    }

    public void DefineAmbiantAudios(AudioSource[] ambiantAudios) {
        ambAudio = ambiantAudios;
        foreach (AudioSource audSrc in ambiantAudios) {
            if (audSrc != null) {
                audSrc.Stop();
            }
        }
    }

    void Update() {
        if (ambAudio != null) {
            try {
                int rand = UnityEngine.Random.Range(0, ambAudio.Length - 1);
                AudioSource ambAud = ambAudio[rand];
                if (ambAud != null) {
                    ambAud.Play();
                }
            } catch (Exception) {

            }
        }

        if (!chaseInitiated) {
            Collider[] colliders = Physics.OverlapSphere(gameObject.transform.position, chaseRadius);
            // monster can also have ambience
            foreach (Collider collider in colliders) {
                if (collider.gameObject.GetComponent<MonoBehaviour>() is /*modify*/ FirstPersonController /*just incase*/) {
                    // the class name must match with the script used for the fps controller (including the using followed by its namespace) 
                    chaseInitiated = true;
                    playerObj = collider.gameObject;
                }
            }
        }

        // the monster should move, also, obstacles may prompt monster to jump
        if (chaseInitiated && playerObj != null && !playerNearby) {
            Vector3 pos = gameObject.transform.position;
            Vector3 playerPos = playerObj.transform.position;

            float zDiff = playerPos.z - pos.z;
            float xDiff = playerPos.x - pos.x;

            float angle = (float)Math.Atan2(zDiff, xDiff);

            // this segment of the code needs to be modified
            gameObject.transform.position +=
                new Vector3(monsterSpeed * (float)Math.Cos(angle), 0, monsterSpeed * (float)Math.Sin(angle));
            
            gameObject.transform.rotation = Quaternion.Euler(0, (angle * -180 / (float)Math.PI) + 90, 0);
            if (Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(zDiff, 2)) <= scareRadius) {
                playerNearby = true;
                scareAudio.Play();
                // possible addition, jumpscare anim
            }
        }
        if (playerNearby) {
            // possibly communicates to player and freezes them?
            count++;
            if (count >= 500) {
                Destroy(gameObject);
            }
        }
    }
}