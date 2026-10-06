using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{

    public float idleSpeed;
    public float attackSpeed;
    public float fearMult;

    public float attackRad;

    public string mode = "idle";
    
    navigator nav;
    SoundSwitch detect;
    SoundSwitch fear;
    Rigidbody2D rb;
    BALGOS player;
    public LayerMask mask;

    float idleTimer;

    // works but needs tuning
    // random idle pos should be centered to room
    // FEAR should generate a position away from the player
    // movement should be inertial
    // need to impliment SFX

    void Start(){
        nav = GetComponent<navigator>();
        detect = GetComponents<SoundSwitch>()[0];
        fear = GetComponents<SoundSwitch>()[1];
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("BALGOS").GetComponent<BALGOS>();
        mode = "idle";
    }

    void Update()
    {
        if(nav.posList.Count > 0){
            if(Vector2.Distance(nav.posList[0].pos,transform.position) < nav.navTargetRad){
                nav.posList.RemoveAt(0);
            }
            else{
                transform.up = (Vector2)nav.posList[0].pos-(Vector2)transform.position;
                float speed = idleSpeed;
                if(mode == "scared"){speed = speed*fearMult;}
                rb.velocity = transform.up*speed;
            }
        }
        if((nav.posList.Count == 0 || idleTimer < Time.time) && (mode == "idle" || mode == "scared")){
            idleTimer = Time.time + 60;
            mode = "idle";
            nav.idle = true;
            setRandPos(false);
            nav.Navigate();
        }
        if(mode != "idle" && mode != "scared"){
            // doesnt work yet
            Vector2 vect = player.transform.position-transform.position;
            float d = Vector2.Distance(player.transform.position,transform.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, vect, d, mask);
            if(d < attackRad && hit.collider != null && hit.collider.tag == "Player"){
                // attack player
                mode = "attack";
            }
            else if(mode == "attack"){
                mode = "idle";
            }
        }
        if(mode == "attack"){
            transform.up = player.transform.position-transform.position;
            rb.velocity = transform.up*attackSpeed;
        }
    }

    public void DETECTNOISE(){
        if(mode != "scared"){
            detect.activated = false;
            nav.idle = false;
            nav.tarPos = player.transform.position;
            nav.Navigate();
            mode = "investigate";
        }
    }

    public void FEAR(){
        idleTimer = Time.time + 60;
        fear.activated = false;
        mode = "scared";
        nav.idle = true;
        setRandPos(true);
        nav.Navigate();
    }

    public void setRandPos(bool fear){
        Vector2 vect = Vector2.zero;
        int breakout = 0;
        while(breakout < 50){
            breakout++;
            vect = new Vector2(Random.Range(0,player.mesh.grid.width),Random.Range(0,player.mesh.grid.height));
            int pos = player.mesh.grid.pos_to_ind(vect);
            if(player.mesh.grid.Nodes[pos].decay == player.mesh.air.decay){
                if(!fear || Vector2.Distance(vect,player.transform.position) < 20);
                nav.tarPos = vect;
            }
        }
    }
}
