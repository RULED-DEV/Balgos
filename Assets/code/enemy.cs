using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{

    public navigator nav;
    public SoundSwitch detect;
    Rigidbody2D rb;

    public float idleSpeed;
    public float attackSpeed;

    public float attackRad;

    public string mode = "idle";

    public GameObject player;

    void Start(){
        nav = GetComponent<navigator>();
        detect = GetComponent<SoundSwitch>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(nav.posList.Count > 0){
            if(Vector2.Distance(nav.posList[0].pos,transform.position) < 0.5){
                nav.posList.RemoveAt(0);
            }
            else{
                transform.up = (Vector2)nav.posList[0].pos-(Vector2)transform.position;
                rb.velocity = transform.up*idleSpeed;
            }
        }
        else{
            mode = "idle";
            // set random tarpos
            nav.Navigate();
        }
    }

    public void DETECTNOISE(){
        detect.activated = false;
        nav.tarPos = player.transform.position;
        nav.Navigate();
        mode = "investigate";
    }
}
