using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class noiser : MonoBehaviour
{


    mesh_manager mesh;
    AudioSource s;

    public float time;
    float timer;

    public float volume;

    void Start(){
        s = gameObject.GetComponent<AudioSource>();
        mesh = FindObjectsByType<mesh_manager>(FindObjectsSortMode.None)[0];
    }

    void Update(){
        if(timer < Time.time){
            timer = Time.time + time;
            if(s != null){s.Play();}
            mesh.grid.editnodeval(mesh.grid.pos_to_ind(transform.position),volume);
        }
    }
}
