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

    public float deathRange;
    public float deathRot;
    public mat_vals deathMat;
    // when we die how far away are the body parts allowed to move
    
    navigator nav;
    SoundSwitch detect;
    SoundSwitch fear;
    Rigidbody2D rb;
    BALGOS player;
    public LayerMask mask;

    limb[] limbs;
    float stepTimer;
    float growlTimer;
    float screamTimer;
    // scream timer controls both attack scream and fear scream

    // works but needs tuning
    // random idle pos should be centered to room
    // FEAR should generate a position away from the player
    // need to impliment SFX

    void Awake(){
        nav = GetComponent<navigator>();
        detect = GetComponents<SoundSwitch>()[0];
        fear = GetComponents<SoundSwitch>()[1];
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("BALGOS").GetComponent<BALGOS>();
        mode = "idle";
        mediun[] meds = GetComponentsInChildren<mediun>();
        limbs = GetComponentsInChildren<limb>();
        float add = 0.01f;
        foreach(mediun m in meds){
            m.medium_val.decay += add;
            add += 0.01f;
        }
        // autimatically differentiates the decay values, allowing mediuns to properly imprint onto the grid
    }

    void Update()
    {
        ambientFX();
        if(mode == "attack"){
            // attack logic
            if(!playerCheck()){
                mode = "idle";
                nav.posList = new List<NavNode>();
            }
            else{
                // player is still seen
                moveToPos(player.transform.position,attackMult);
                if(Vector2.Distance(player.transform.position,transform.position) < 3.5){
                    player.death();
                    // player is killed
                }
            }
        }
        else{
            // navigation logic
            if(nav.posList.Count > 0){
                // moves to next position
                if(Vector2.Distance(transform.position,nav.posList[0].pos) < 2f){
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
                        Debug.Log("scream");
                        limbs[5].NOISE();
                        // limbs at 6 is for scared scream
                        // when entering attack mode it screams to warn the player
                }
            }
        }
    }

    void ambientFX(){
        if(Vector3.Distance(rb.velocity,Vector3.zero) > 0.2f){
            if(Time.time > stepTimer){
                float r = UnityEngine.Random.Range(50,70);
                stepTimer = Time.time + r/100;
                int pos = Random.Range(0,4);
                limbs[pos].NOISE();
                // limbs 0-3 are the legs
            }
        }
        // if(growlTimer < Time.time){
        //     //limbs[4].NOISE();
        //     // limbs at 4 is for ambient growling
        //     growlTimer = Time.time + Random.Range(25,55);
        // }
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
        // vector pointing in direction of pos

        float savAng = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg; 
        savAng += 90; // the math is a little off but still works
        // extracts angle from dir

        rb.AddForce(dir*moveSpeed*mult);
        rb.SetRotation(Mathf.LerpAngle(transform.eulerAngles.z,savAng,Time.deltaTime*rotSpeed*mult));
        // moves/rotates enemy toward target
    }

    public void DETECTNOISE(){
        if(mode != "scared" && mode != "attack"){
            Debug.Log("detected");
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
        if(Time.time > screamTimer){
            screamTimer = Time.time + 5;
            limbs[6].NOISE();
            Debug.Log("scared");
            // limbs at 6 is for scared scream
            // when scared it makes a noise
        }
        
            
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

    public void death(){
        mediun[] meds = GetComponentsInChildren<mediun>();
        float add = 0.01f;
        foreach(mediun m in meds){
            Vector3 pos = new Vector3(Random.Range(-deathRange*10,deathRange*10),Random.Range(-deathRange*10,deathRange*10),0);
            pos = pos * 0.1f;
            float rot = Random.Range(-deathRot,deathRot);
            // sets a random rot and pos
            
            m.transform.position += pos;
            m.transform.eulerAngles += new Vector3(0,0,rot);
            // position of medium is changed

            // m.medium_val = deathMat;
            // m.medium_val.decay += add;
            // add += 0.01f;
            // medium doesnt like having its medium_val changed
            
            m.resetNodeVal();
            m.OnApplicationQuit();
            // medium is removed and activelists are disposed                        
        }
        Destroy(gameObject);
        // need to figure out how to remove shadow
    }
}
