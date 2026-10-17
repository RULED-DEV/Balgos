using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class limb : MonoBehaviour
{

    mesh_manager mesh;
    // reference to the mesh

    public float volume;
    // how loud the noise is

    bool rand;
    public float spread;
    // wether the noise occurs at a given spread
    
    public bool continuity; // wether the noise re-occurs
    public int recor; // how mamy times it bounces
    public float T; // time between bounces
    
    public AudioSource SFX;
    public AudioClip sound;
    // needs audio

    void Start(){
        if(spread != 0){rand = true;}
        mesh = FindObjectsByType<mesh_manager>(FindObjectsSortMode.None)[0];
        NPOS = transform.position;
        LOCNOISE = volume;
        rec = recor;
    }

    Vector2 randpos(Vector2 org,float range){
        range = range * 100;
        float r1 = UnityEngine.Random.Range(-range,range);
        r1 = r1 / 100;
        float r2 = UnityEngine.Random.Range(-range,range);
        r2 = r2 / 100;
        return new Vector2(org.x + r1, org.y + r2);
    }

    Vector2 NPOS;
    float LOCNOISE;
    float rec;

    public void NOISE(){
        NPOS = transform.position;
        LOCNOISE = volume;
        rec = recor;
        noise();
    }

    void noise(){
        if(rand){NPOS = randpos(NPOS,spread);}
        mesh.grid.editnodeval(mesh.grid.pos_to_ind(NPOS),LOCNOISE);
        rec-- ;
        LOCNOISE = LOCNOISE/1.5f;
        float t = T;
        if(recor > 0 && continuity){
            Invoke("noise",t);
        }
        if(sound != null){SFX.PlayOneShot(sound);}
    }
}
