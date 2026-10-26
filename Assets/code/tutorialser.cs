using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialser : MonoBehaviour
{

    public List<TutorialComm> commands = new List<TutorialComm>();

    // commands : 
    // PL breach F : closes breach
    // PL breach T : opens breach
    // PL lever F : closes lever
    // PL lever T : opens lever

    // no command : spawns text

    // condition : 
    // wait : waits a certain amount of time
    // dist : waits until player is a certain dist away

    // N _name_ : sees if target of _name_ is still here

    // PL breach F : breach must be closed
    // PL breach T : breach must be open
    // PL lever F : lever must be closed
    // PL lever T : lever must be open

    public float maxDist;
    public GameObject textPref;

    float waitTime;
    Vector3 pos;

    BALGOS pl;
    texter t;
    public string nextScene;

    void Awake(){
        pl = FindObjectsByType<BALGOS>(FindObjectsSortMode.None)[0];

        newCommand();
        // loads first command
    }

    void Update(){
        if(commands.Count > 0){
            if(commCondCheck(commands[0])){
                // is condition satisfied?
                commands.RemoveAt(0);
                if(t != null){t.TextRemoval(); Destroy(t.gameObject);}
                // removes text and prior command
                newCommand();
            }
        }
        else{
            // moves to next level
            gameStateManager g = FindObjectsByType<gameStateManager>(FindObjectsSortMode.None)[0];
            g.toMenu = true;
            g.menuScene = nextScene;
            g.StopAllSources();
        }
    }

    bool commCondCheck(TutorialComm comm){
        bool check = false;
        if(comm.cond == "wait"){
            if(waitTime < Time.time){check = true;}
            // checks if enough time has passed
        }
        else if(comm.cond == "dist"){
            if(Vector3.Distance(pos,pl.transform.position) > comm.val){check = true;}
            // when player is far enough away from pos
        }
        else if(comm.cond[0] == 'N'){
            if(GameObject.Find(comm.cond.Split(" ")[1]) == null){check = true;}
            // checks if desired object still exists
        }
        else if(comm.cond == "PL breach F"){
            if(!pl.isbreachopen){check = true;}
            // checks if breach is closed
        }
        else if(comm.cond == "PL breach T"){
            if(pl.isbreachopen){check = true;}
            // checks if breach is open
        }
        else if(comm.cond == "PL lever F"){
            if(!pl.isleveropen){check = true;}
            // checks if lever is closed
        }
        else if(comm.cond == "PL lever T"){
            if(pl.isleveropen){check = true;}
            // checks if lever is open
        }
        return check;
    }

    void newCommand(){
        if(commands.Count > 0){
            if(commands[0].comm == "PL breach F"){pl.setBreach(false);}
            else if(commands[0].comm == "PL breach T"){pl.setBreach(true);}
            else if(commands[0].comm == "PL lever F"){pl.setLever(false);}
            else if(commands[0].comm == "PL lever T"){pl.setLever(true);}
            else{
                spawnText(commands[0].comm);
                // spawns text
            }

            if(commands[0].cond == "wait"){
                waitTime = Time.time + commands[0].val;
                // begins wait
            }
            else if(commands[0].cond == "dist"){
                pos = pl.transform.position;
                // saves players current position
            }
        }
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
        t = Instantiate(textPref,transform.position+pos,transform.rotation).GetComponent<texter>();
        t.text = text;
        // places t at a distance away from the player

        float unit = FindObjectsByType<mesh_manager>(FindObjectsSortMode.None)[0].grid.spacing;
        float size = 0.5f + (text.Length*((3+t.spacing)*unit));
        bool check = true;

        if(posy > 0){posy = 1;}
        else{posy = -1;}
        int looper = 0;
        while(check && looper < 30){
            looper ++;
            RaycastHit2D hitUpA = Physics2D.Raycast(t.transform.position, posy*transform.up, 5*unit);
            RaycastHit2D hitUpB = Physics2D.Raycast(t.transform.position, posy*transform.up+new Vector3(size,0,0), 5*unit);
            // 2 vertical checks in the direction of the tutorialser from the texter

            RaycastHit2D hitAcrA = Physics2D.Raycast(t.transform.position, transform.right, size);
            RaycastHit2D hitAcrB = Physics2D.Raycast(t.transform.position, -transform.right, 0.5f);
            // 2 horizontal checks to ensire we are not clipping into walls
            if(hitUpA.collider != null || hitUpB.collider != null){
                // moves text vertically towards tutorialser
                t.transform.position = t.transform.position + (new Vector3(0,5*unit,0)*posy);
            }
            else if(hitAcrA.collider != null){
                // moves text left away from struck wall
                t.transform.position = t.transform.position - new Vector3((3+t.spacing)*unit,0,0);
            }
            else if(hitAcrB.collider != null){
                // moves text right away from struck wall
                t.transform.position = t.transform.position + new Vector3((3+t.spacing)*unit,0,0);
            }
            else{
                check = false;
                // passed the check
            }
        }
    }

}
