using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

public class navigator : MonoBehaviour
{
    
    public Vector3 tarPos;
    mesh_manager mesh;

    public float navTargetRad;

    public bool idle;

    void Start(){
        mesh = FindObjectsByType<mesh_manager>(FindObjectsSortMode.None)[0];
    }

    public List<NavNode> posList;

    public void Navigate(){
        bool goal = false;
        int MaxLoop = 0;
        denyDir = "";
        posList = new List<NavNode>();
        NavNode ex = new NavNode();
        ex.pos = (Vector2)transform.position;
        posList.Add(ex);
        while(!goal && MaxLoop < 30){
            MaxLoop++;
            // produces set of 5 positions at walls
            List<NavNode> prongArr = generatePositions();
            // culls positions and finds most likely to navigate toward player
            
            NavNode node = cullNode(prongArr);
            // edits position to be in line with player/toward center of room
            node = editNode(node);
            posList.Add(node);
            // checks list so far, removes redundant moves, checks if goal is reached
            goal = checkList();
        }
    }

    bool checkList(){
        bool ch = false;
        // checks list, identifies if we are at goal or not
        if(posList.Count > 1){
            // does the check
            for(int i = 1; i < posList.Count; i++){
                if(!ch){
                    float2 PosA = posList[i].pos;
                    float2 PosB = posList[i-1].pos;
                    // grabs positions
                    bool checkPath = PosA.x < tarPos.x && PosB.x > tarPos.x;
                    checkPath = checkPath || (PosA.x > tarPos.x && PosB.x < tarPos.x);
                    // checks if target is between nodes
                    float chVar = Mathf.Abs(PosA.y-tarPos.y);
                    // checks if target was passed over
                    if(Mathf.Abs(PosA.x-PosB.x) < 0.5f){
                        checkPath = PosA.y < tarPos.y && PosB.y > tarPos.y;
                        checkPath = checkPath || (PosA.y > tarPos.y && PosB.y < tarPos.y);
                        chVar = Mathf.Abs(PosA.x-tarPos.x);
                        // same checks as before but for different axis
                    }
                    bool Chdist = Vector2.Distance(PosA,(Vector2)tarPos) < navTargetRad;
                    // checks distance from current node to target

                    // checkPath checks if the target position lies between the nodes
                    // chVar is the distance from the travel directio of the nodes and the target pos
                    if((chVar < navTargetRad && checkPath) || Chdist){
                        Debug.Log("terminal position found");
                        ch = true;
                    }
                    if(i > 1){
                        // from the third node onward we check for double backs
                        float2 PosC = posList[i-2].pos;
                        string AtoB = "across";
                        string BtoC = "across";
                        if(Mathf.Abs(PosA.x-PosB.x) < 0.5f){AtoB = "above";}
                        if(Mathf.Abs(PosB.x-PosC.x) < 0.5f){BtoC = "above";}
                        // figures out relation between last three  nodes to remove double backs
                        if(AtoB == BtoC){
                            // checks if the nodes double back (I.E same direction is used twice in succession) and handles it
                            Debug.Log("removed double back");
                            posList.RemoveAt(i-1);
                            i--;
                        }
                    }
                }
                else{
                    posList.RemoveAt(i);
                    i--;
                    // if terminal node is found, remove all nodes after it
                }
            }
        }
        return ch;
    }

    NavNode editNode(NavNode n){
        if(n.priority <= 4){
            n.pos = n.pos + -(n.propDir*3);
        }
        // moves pos away from the wall

        NavNode prior = posList[posList.Count-1];
        bool check = n.pos.x < tarPos.x && prior.pos.x > tarPos.x;
        check = check || n.pos.x > tarPos.x && prior.pos.x < tarPos.x;
        int2 dir = new int2(0,-1);
        if(n.pos.y < tarPos.y){
            // its above us
            dir = new int2(0,1);
        }
        float2 pos = new float2(tarPos.x,n.pos.y);
        if(n.propDir.x == 0){
            // check Y axis
            check = n.pos.y < tarPos.y && prior.pos.y > tarPos.y;
            check = check || n.pos.y > tarPos.y && prior.pos.y < tarPos.y;
            pos = new float2(n.pos.x,tarPos.y);
            dir = new int2(-1,0);
            if(n.pos.x < tarPos.x){
                // its right of us
                dir = new int2(1,0);
            }
        }

        if(check){
            // path from current node to prior passes the players position
            NavNode ch = searchProng(dir,pos);
            if(ch.priority == 5){
                // we struck the player
                n.pos = pos;
            }
        }

        return n;
    }

    string denyDir;
    int intensity;
    // last direction chosen to navigate toward

    public NavNode cullNode(List<NavNode> n){
        // assigns priorities to each node
        n = priorityAssignment(n);

        NavNode selected = n[0];
        foreach(NavNode N in n){
            if(N.priority > selected.priority){
                selected = N;
            }
        }
        string replace = "across";
        if(selected.propDir.x == 0){replace = "above";}
        if(denyDir == replace){intensity++;}
        else{intensity = 0;}
        denyDir = replace;
        return selected;
    }

    List<NavNode> priorityAssignment(List<NavNode> n){
        float longestLength = 0;
        int posLongest = 0;
        
        for(int i = 0; i < n.Count; i++){
            NavNode N = n[i];
            if(N.propDir.x == desDir.x || N.propDir.y == desDir.y){
                N.priority ++;
                // in direction of player, increases priority
            }
            string chDir = "across";
            if(N.propDir.x == 0){chDir = "above";}
            // checks if propagation direction is equal to prior propogation direction
            if(chDir == denyDir){
                Debug.Log(denyDir);
                Debug.Log(N.propDir);
                N.priority -= intensity;
            }
            if(Vector2.Distance(N.pos,posList[posList.Count-1].pos) > longestLength){
                posLongest = i;
                longestLength = Vector2.Distance(N.pos,posList[posList.Count-1].pos);
                // the node that is furthest from prior valid node is awarded additional priority.
            }
            n[i] = N;
        }
        NavNode Ne = n[posLongest];
        Ne.priority += 2;
        n[posLongest] = Ne;
        return n;
    }

    int2 desDir;
    // direction to player

    public List<NavNode> generatePositions(){
        // returns 1 new position every time called
        desDir = new int2(-1,-1);
        if(transform.position.x < tarPos.x){
            desDir.x = 1;
        }
        if(transform.position.y < tarPos.y){
            desDir.y = 1;
        }
        int2[] dirs = {new int2(1,0),new int2(-1,0),new int2(0,1),new int2(0,-1)};
        List<NavNode> l = new List<NavNode>();
        foreach(int2 d in dirs){
            l.Add(searchProng(d,posList[posList.Count-1].pos));
        }
        return l;
    }

    public NavNode searchProng(int2 dir, float2 pos){
        NavNode returnNode = new NavNode();
        // makes node to be returned
        returnNode.priority = -5;
        // if we reach this then a node with low priority is returned (I.E wont possibly be selected)
        returnNode.propDir = dir;
        // assigns direction

        int tot = Mathf.RoundToInt(mesh.grid.width/mesh.grid.spacing);
        // total width of grid in nodes
        int searchPos = mesh.grid.pos_to_ind(pos);
        // position to start search from

        bool searchConclude = true;
        // checks if loop should end
        int breakOUT = 0;
        // counter to limit looping

        while(searchConclude && breakOUT < tot){
            // loop until hit wall (medium denoted by low spreadrate)
            breakOUT++;

            searchPos += (dir.x * 1);
            searchPos += (dir.y * tot);
            // moves search position bit by bit

            if(mesh.grid.Nodes[searchPos].spreadRate > 3){
                // breaks loop and adds position of colision to list
                searchConclude = false;
                returnNode.priority = 0;
                // sets priority to 0 if node valid
                returnNode.pos = mesh.grid.worldPositions[searchPos];
                // sets node pos
            }
            if(mesh.grid.Nodes[searchPos].spreadRate == 1.1f && !idle){
                // only the player has a spreadrate of 1.1f
                searchConclude = false;
                returnNode.priority = 5;
                // garuntees the prong is chosen
                returnNode.pos = mesh.grid.worldPositions[searchPos];
            }
        }
        return returnNode;
    }
}
