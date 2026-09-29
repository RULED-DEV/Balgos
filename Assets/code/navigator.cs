using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

public class navigator : MonoBehaviour
{
    
    public bool nav;
    public Transform tarPos;
    mesh_manager mesh;

    public float targetRad;

    void Start(){
        mesh = FindObjectsByType<mesh_manager>(FindObjectsSortMode.None)[0];
    }

    void Update(){
        if(nav){
            nav = !nav;
            Navigate();
        }
    }

    public List<NavNode> posList;

    public void Navigate(){
        bool goal = false;
        int MaxLoop = 0;
        posList = new List<NavNode>();
        NavNode ex = new NavNode();
        ex.pos = (Vector2)transform.position;
        posList.Add(ex);
        while(!goal && MaxLoop < 30){
            MaxLoop++;
            // produces set of 5 positions at walls
            List<NavNode> prongArr = generatePositions();
            // culls positions and finds most likely to navigate toward player
            // edits position to be in line with player/toward center of room
            NavNode node = cullNode(prongArr);
            posList.Add(node);
            Debug.Log(node.pos);
            Debug.Log(node.priority);
            Debug.Log(node.propDir);
            // checks list so far, removes redundant moves, checks if goal is reached
            goal = checkList();
            if(MaxLoop > 4){goal = true;}
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
                    bool checkPath = PosA.x < tarPos.position.x && PosB.x > tarPos.position.x;
                    checkPath = checkPath || (PosA.x > tarPos.position.x && PosB.x < tarPos.position.x);
                    float chVar = Mathf.Abs(PosA.y-tarPos.position.y);
                    if(Mathf.Abs(PosA.x-PosB.x) < 0.5f){
                        checkPath = PosA.y < tarPos.position.y && PosB.y > tarPos.position.y;
                        checkPath = checkPath || (PosA.y > tarPos.position.y && PosB.y < tarPos.position.y);
                        chVar = Mathf.Abs(PosA.x-tarPos.position.x);
                    }
                    // checkPath checks if the target position lies between the nodes
                    // chVar is the distance from the travel directio of the nodes and the target pos
                    if(chVar < targetRad && checkPath){
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

    int2 denyDir;
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
        denyDir = -selected.propDir;
        return selected;
    }

    List<NavNode> priorityAssignment(List<NavNode> n){
        float longestLength = 0;
        int posLongest = 0;
        
        for(int i = 1; i < n.Count; i++){
            NavNode N = n[i];
            if(N.propDir.x == desDir.x || N.propDir.y == desDir.y){
                N.priority ++;
                // in direction of player, increases priority
            }
            if(Vector2.Distance(N.pos,posList[posList.Count-1].pos) > longestLength){
                posLongest = i;
                longestLength = Vector2.Distance(N.pos,posList[posList.Count-1].pos);
                // the node that is furthest from prior valid node is awarded additional priority.
            }
            if(N.propDir.x == denyDir.x && N.propDir.y == denyDir.y){
                N.priority = -5;
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
        if(transform.position.x < tarPos.position.x){
            desDir.x = 1;
        }
        if(transform.position.y < tarPos.position.y){
            desDir.y = 1;
        }
        int2[] dirs = {new int2(1,0),new int2(-1,0),new int2(0,1),new int2(0,-1),desDir};
        List<NavNode> l = new List<NavNode>();
        foreach(int2 d in dirs){
            l.Add(searchProng(d));
        }
        return l;
    }

    public NavNode searchProng(int2 dir){
        NavNode returnNode = new NavNode();
        // makes node to be returned
        returnNode.priority = -5;
        // if we reach this then a node with low priority is returned (I.E wont possibly be selected)
        returnNode.propDir = dir;
        // assigns direction

        int tot = Mathf.RoundToInt(mesh.grid.width/mesh.grid.spacing);
        // total width of grid in nodes
        int searchPos = mesh.grid.pos_to_ind(posList[posList.Count-1].pos);
        // position to start search from

        bool searchConclude = true;
        // checks if loop should end
        int breakOUT = 0;
        // counter to limit looping

        while(searchConclude && breakOUT < tot){
            // loop until hit wall (medium denoted by low spreadrate)
            breakOUT++;

            searchPos += 5*(dir.x * 1);
            searchPos += 5*(dir.y * tot);
            // moves search position bit by bit

            if(mesh.grid.Nodes[searchPos].spreadRate > 3){
                // breaks loop and adds position of colision to list
                searchConclude = false;

                returnNode.priority = 0;
                // sets priority to 0 if node valid
                returnNode.pos = mesh.grid.worldPositions[searchPos];
                // sets node pos
            }
        }
        return returnNode;
    }
}
