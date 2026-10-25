using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialser : MonoBehaviour
{

    public TutorialComm[] Commands;

    public float maxDist;

    public GameObject textPref;

    void Awake(){
        spawnText("this is a text test, i love it");
    }

    public void spawnText(string text){
        float posx = Random.Range(-maxDist*100,maxDist*100);
        float posy = Random.Range(-maxDist*100,maxDist*100);
        if(Mathf.Abs(posx) < 2){
            if(posx < 0){posx = -2;}
            else{posx = 2;}
        }
        if(Mathf.Abs(posy) < 2){
            if(posy < 0){posy = -2;}
            else{posy = 2;}
        }
        Vector3 pos = new Vector3(posx/100,posy/100,0);
        texter g = Instantiate(textPref,transform.position+pos,transform.rotation).GetComponent<texter>();
        g.text = text;
        // places g at a distance away from the player

        float unit = FindObjectsByType<mesh_manager>(FindObjectsSortMode.None)[0].grid.spacing;
        float size = 0.5f + (text.Length*((3+g.spacing)*unit));
        bool check = true;

        if(posy > 0){posy = 1;}
        else{posy = -1;}
        int looper = 0;
        while(check && looper < 30){
            looper ++;
            RaycastHit2D hitUpA = Physics2D.Raycast(g.transform.position, posy*transform.up, 5*unit);
            RaycastHit2D hitUpB = Physics2D.Raycast(g.transform.position, posy*transform.up+new Vector3(size,0,0), 5*unit);
            // 2 vertical checks in the direction of the tutorialser from the texter

            RaycastHit2D hitAcrA = Physics2D.Raycast(g.transform.position, transform.right, size);
            RaycastHit2D hitAcrB = Physics2D.Raycast(g.transform.position, -transform.right, 0.5f);
            // 2 horizontal checks to ensire we are not clipping into walls
            if(hitUpA.collider != null || hitUpB.collider != null){
                // moves text vertically towards tutorialser
                g.transform.position = g.transform.position + (new Vector3(0,5*unit,0)*posy);
            }
            else if(hitAcrA.collider != null){
                // moves text left away from struck wall
                g.transform.position = g.transform.position - new Vector3((3+g.spacing)*unit,0,0);
            }
            else if(hitAcrB.collider != null){
                // moves text right away from struck wall
                g.transform.position = g.transform.position + new Vector3((3+g.spacing)*unit,0,0);
            }
            else{
                check = false;
                // passed the check
            }
        }
    }

}
