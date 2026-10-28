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

    // echo : player has echolocated

    public float maxDist;
    public GameObject textPref;

    float waitTime;
    Vector3 pos;

    BALGOS pl;
    mesh_manager m;
    List<texter> t = new List<texter>();
    AudioSource s;
    public string nextScene;

    public float textNoise;

    void Start(){
        pl = FindObjectsByType<BALGOS>(FindObjectsSortMode.None)[0];
        m = FindObjectsByType<mesh_manager>(FindObjectsSortMode.None)[0];
        s = GetComponentInChildren<AudioSource>();

        newCommand();
        // loads first command
    }

    void Update(){
        if(commands.Count > 0){
            if(commCondCheck(commands[0])){
                // is condition satisfied?
                commands.RemoveAt(0);
                while(t.Count > 0){
                    t[0].TextRemoval(); 
                    Destroy(t[0].gameObject);
                    t.RemoveAt(0);
                }
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
        else if(comm.cond == "echo"){
            if(!pl.canEcholocate){check = true;}
            // checks if player has echolocated
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
                if(commands[0].comm[commands[0].comm.Length-1] != ' '){
                    foreach(string s in commands[0].comm.Split(",")){
                        spawnText(s,textNoise);
                    }
                    s.Play(); 
                }
                // if last letter in text string is whitespace it doesnt make noise for the text
                else{
                    foreach(string s in commands[0].comm.Split(",")){
                        spawnText(s,0);
                    }
                }
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

    public void spawnText(string text, float noise){
        float minDist = 2.5f;
        int count = 0;
        Vector2[] dirs = {
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(1,0),
            new Vector2(1,-1),
            new Vector2(0,-1),
            new Vector2(-1,-1),
            new Vector2(-1,0),
            new Vector2(-1,1),
        };
        Vector2 dir = dirs[count]; // direction text will go in

        Vector3 pos = transform.position + new Vector3(dir.x*minDist,dir.y*minDist,0);

        bool check = false;
        float unit = FindObjectsByType<mesh_manager>(FindObjectsSortMode.None)[0].grid.spacing;
        float spacing = 1;
        float size = 0.5f + (text.Length*((3+spacing)*unit));
        while(!check && count < dirs.Length){
            RaycastHit2D hitUpA = Physics2D.Raycast(pos-new Vector3(5*unit,5*unit,0), transform.up, 5*unit*2);
            RaycastHit2D hitUpB = Physics2D.Raycast(pos-new Vector3(5*unit,5*unit,0), transform.up + new Vector3(size,0,0), 5*unit*2);
            // 2 raycasts at each end of the word which check the space is clear
            RaycastHit2D hitAcrA = Physics2D.Raycast(pos-new Vector3(5*unit,0,0), transform.right, size);
            RaycastHit2D hitAcrB = Physics2D.Raycast(pos-new Vector3(5*unit,0,0), -transform.right, 0.5f);
            // 2 raycats that check we have enough space either side
            bool c = hitAcrA.collider != null || hitAcrB.collider != null;
            c = c || hitUpA.collider != null || hitUpB.collider != null;
            if(c){
                float d = 0.1f;
                pos += new Vector3(dir.x*d,dir.y*d,0);
                // we are not at a valid position, restart loop and offset start pos
                Vector3 p = pos + new Vector3(size,0,0);
                if(Vector3.Distance(pos,pl.transform.position) > maxDist && Vector3.Distance(p,pl.transform.position) > maxDist){
                    count ++;
                    dir = dirs[count];
                    pos = transform.position + new Vector3(dir.x*minDist,dir.y*minDist,0);
                    // if both ends of the text are too far away we restart
                }
            }
            else{
                texter T = Instantiate(textPref,pos,transform.rotation).GetComponent<texter>();
                T.text = text;
                T.INIT();
                m.grid.editnodeval(m.grid.pos_to_ind(T.transform.position + new Vector3(size/2,0,0)),noise);
                t.Add(T);
                check = true;
            }
        }
    }

}
