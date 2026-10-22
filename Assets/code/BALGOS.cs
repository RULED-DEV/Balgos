using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class BALGOS : MonoBehaviour
{

    // player script

    Rigidbody2D rb;
    public mesh_manager mesh;
    public LayerMask mask;

    public float speed;
    public float rotSPEED;

    public float reloadDist;

    float step_timer;
    public float trail_noise;

    void Start(){
        foreach(body_eff_cont b in GetComponentsInChildren<body_eff_cont>()){
            b.setCAM(GetComponentInChildren<Camera>());
        }
        rb = GetComponent<Rigidbody2D>();
        mesh = FindObjectsByType<mesh_manager>(FindObjectsSortMode.None)[0];
    }

    void FixedUpdate()
    {
        movement();
        load_manager();
        fireManager();
        suicide();
        steps();
        load_vis();
    }

    void steps(){
        if(Vector3.Distance(rb.velocity,Vector3.zero) > 0.2f){
            if(Time.time > step_timer){
                float r = UnityEngine.Random.Range(50,70);
                step_timer = Time.time + r/100;
                FOOT.NOISE();
            }
        }
    }

    float echoT;

    void fireManager(){
        if(Input.GetMouseButtonDown(0)){
            if(!isbreachopen){
                isbreachopen = true;
                checkReload();
                FIRE();
                barrel_fire_point.NOISE(); // fire noise
                breach_point.NOISE(); // breach opening
            }
        }
        if(Input.GetMouseButtonDown(1) && echoT < Time.time){
            // echolocate
            checkReload();
            echoT = Time.time + 1.5f;
            echolocate_point.NOISE();
        }
    }

    public void FIRE(){
        RaycastHit2D hit = Physics2D.Raycast(barrel_fire_point.transform.position, barrel_fire_point.transform.up, 100, mask);
        GameObject g = hit.collider.gameObject;
        while(g.transform.parent != null){
            g = g.transform.parent.gameObject;
        }
        // when raycast hits a limb it will move up until it reaches the enemies main body
        enemy l = g.GetComponent<enemy>();
        if(l != null){
            l.death();
        }
        Vector3 diff = (Vector3)hit.point-barrel_fire_point.transform.position;
        for(float i = 0.01f; i < 1.01f; i += 0.01f){
            int grPoint = mesh.grid.pos_to_ind(barrel_fire_point.transform.position + (diff * i));
            mesh.grid.editnodeval(grPoint,trail_noise);
        }
    }

    public bool isbreachopen;
    // if breach closed then can fire

    public bool isleveropen;
    // if lever closed then can reload 

    public limb bullet_LOAD;
    // point where bullet is loaded/ejected
    public limb barrel_fire_point;
    // where bullet is fired
    public limb echolocate_point;
    // where we echolocate from
    public limb lever_point;
    // where the breach is opened
    public limb breach_point;
    // where the breach jams
    public limb FOOT;
    // foot noises?

    // notes on reload
    // to fire breach must be closed, breach opens on fire
    // if lever is open the player has a reload "charge"
    // reloading (Q) consumes the charge closing both lever and breach

    public void load_manager(){
        // handles loading
        // if(Input.GetKeyDown(KeyCode.E) && !isbreachopen && isleveropen){
        //     // close lever only when breach is open and lever is open
        //     isleveropen = false;
        //     lever_point.NOISE();
        // }
        if(Input.GetKeyDown(KeyCode.Q) && isbreachopen && isleveropen){
            // open/close breach when breach and lever are open
            isbreachopen = false;
            isleveropen = false;
            lever_point.NOISE();
            breach_point.NOISE();
        }
    }

    public void checkReload(){
        enemy[] E = FindObjectsByType<enemy>(FindObjectsSortMode.None);
        // gets all enemies
        
        if(E.Length > 0){
            enemy sel = E[0];
            Vector2 pos = transform.position;
            foreach(enemy e in E){
                if(Vector2.Distance(sel.transform.position,pos) > Vector2.Distance(e.transform.position,pos)){
                    sel = e;
                }
            }
            // selects the closest enemy
            if(Vector2.Distance(pos,sel.transform.position) < reloadDist && !isleveropen){
                isleveropen = true;
                lever_point.NOISE();
                // player can reload if enemy close enough
            }
        }
    }

    public void load_vis(){
        float breachANG = 0;
        if(isleveropen){
            breachANG = 90;
        }
        Vector2 bulletPOS = new Vector2(0.006f,0.08f);
        if(isbreachopen){
            bulletPOS = new Vector2(0.006f,-0.045f);
        }
        lever_point.transform.localEulerAngles = new Vector3(0,0,Mathf.LerpAngle(lever_point.transform.localEulerAngles.z,breachANG,Time.deltaTime*5));
        bullet_LOAD.transform.localPosition = Vector2.Lerp(bullet_LOAD.transform.localPosition,bulletPOS,Time.deltaTime*5);
    }

    public void death(){
        FindObjectsByType<gameStateManager>(FindObjectsSortMode.None)[0].toMenu = true;
        FindObjectsByType<gameStateManager>(FindObjectsSortMode.None)[0].StopAllSources();
        // player dies, level transitions to a new one
    }

    void movement(){
        float2 move = new float2(0,0);
        if(Input.GetKey(KeyCode.W)){
            move.y = speed;
        }
        if(Input.GetKey(KeyCode.S)){
            move.y = -speed;
        }
        if(Input.GetKey(KeyCode.A)){
            move.x = -speed;
        }
        if(Input.GetKey(KeyCode.D)){
            move.x = speed;
        }
        rb.AddForce(move);
        // make inertial later
    }

    void suicide(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            enemy[] E = FindObjectsByType<enemy>(FindObjectsSortMode.None);
            foreach(enemy e in E){e.enraged = true;}
        }
        if(Input.GetKeyUp(KeyCode.Escape)){
            enemy[] E = FindObjectsByType<enemy>(FindObjectsSortMode.None);
            foreach(enemy e in E){e.enraged = false;}
        }
        // enrages all enemies on map so player can die, leading to menu where they can quit
    }
}
