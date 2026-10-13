using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{

    public float rotSpeed;
    public float moveSpeed;
    public float attackMult;
    public float fearMult;

    public float attackRad;

    public string mode = "idle";
    
    navigator nav;
    SoundSwitch detect;
    SoundSwitch fear;
    Rigidbody2D rb;
    BALGOS player;
    public LayerMask mask;

    // works but needs tuning
    // random idle pos should be centered to room
    // FEAR should generate a position away from the player
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
        if(mode == "attack"){
            // attack logic
            if(!playerCheck()){
                mode = "idle";
                nav.posList = new List<NavNode>();
            }
            else{
                // player is still seen
                moveToPos(player.transform.position,attackMult);
            }
        }
        else{
            // navigation logic
            if(nav.posList.Count > 0){
                // moves to next position
                if(Vector2.Distance(transform.position,nav.posList[0].pos) < 0.5f){
                    nav.posList.RemoveAt(0); // removes node when nearby
                }
                else{
                    if(mode == "scared"){
                        moveToPos(nav.posList[0].pos,fearMult);
                    }
                    else{
                        moveToPos(nav.posList[0].pos,1);
                    }
                }
            }
            else{
                mode = "idle";
                setRandPos(transform.position);
                // navigates in the direction of the center of the map
                nav.Navigate();
                // generates new pos to idle toward
            }
            if(mode == "investigate"){
                if(playerCheck()){
                    mode = "attack";
                }
            }
        }
    }

    bool playerCheck(){
        Vector2 vect = player.transform.position-transform.position;
        float d = Vector2.Distance(player.transform.position,transform.position);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, vect, d, mask);
        if(d < attackRad && hit.collider != null && hit.collider.tag == "Player"){
            // attack player
            return true;
        }
        return false;
    }

    void moveToPos(Vector2 pos,float mult){
        Vector2 dir = pos-(Vector2)transform.position;
        Vector2 sav = transform.up;
        transform.up = dir;
        float savAng = transform.eulerAngles.z;
        rb.AddForce(transform.up*moveSpeed*mult);
        transform.up = sav;
        transform.eulerAngles = new Vector3(0,0,Mathf.LerpAngle(transform.eulerAngles.z,savAng,Time.deltaTime*rotSpeed*mult));
    }

    public void DETECTNOISE(){
        if(mode != "scared"){
            detect.activated = false;
            nav.tarPos = player.transform.position;
            nav.Navigate();
            mode = "investigate";
        }
    }

    public void FEAR(){
        fear.activated = false;
        mode = "scared";
        setRandPos(player.transform.position);
        // navigate away from player
        nav.Navigate();
    }

    public void setRandPos(Vector2 pos){
        Vector2 output = Vector2.zero;
        while(output == Vector2.zero){
            Vector2 rand = new Vector2(Random.Range(0,50),Random.Range(0,50));
            // picks random position on the map
            NavNode n = nav.nearestNode(rand);
            if(n.valid && Vector2.Distance(pos,n.pos) > 20f){
                // generates a position that is valid and a certain distance from pos
                output = n.pos;
            }
        }
        nav.tarPos = output;
    }
}
