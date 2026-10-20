using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

public class navigator : MonoBehaviour
{
    
    // system needs proper testing
    // system has a habit of jumping nodes (nearest node is returning values that are further away than preferred)

    public Vector2 tarPos;
    mesh_manager mesh;

    void Awake(){
        mesh = FindObjectsByType<mesh_manager>(FindObjectsSortMode.None)[0];
    }

    public List<NavNode> posList;
    // final list of positions to reach target
    List<NavNode> visited;
    // every node we have checked

    public NavNode current;
    // node we are currently checking
    public NavNode goal;
    // node we want to reach

    public void Navigate(){
        int MaxLoop = 0;
        posList = new List<NavNode>();
        visited = new List<NavNode>();
        goal = nearestNode(tarPos);
        current = nearestNode(transform.position);
        if(!current.valid){
            current = newValid(current);
            if((Vector2)current.pos == (Vector2)goal.pos){
                Destroy(gameObject);
            }
        }
        posList.Add(current);
        visited.Add(current);
        while((Vector2)posList[posList.Count-1].pos != (Vector2)goal.pos && MaxLoop < 200){
            // keeps on searching until it finds pos
            MaxLoop++;
            current = posList[posList.Count-1];
            int2 tr = genDir(current.pos,goal.pos); 
            float2[] positions = {
                new float2(tr.x,0),
                new float2(0,tr.y),
                new float2(-tr.x,0),
                new float2(0,-tr.y),
                // no diagonals as they can clip through walls
            };
            // all the neighbouring nodes
            bool regress = true;
            foreach(float2 p in positions){
                float2 pos = current.pos + (p*mesh.grid.navSpacing);
                NavNode check = nearestNode(pos);
                // node we want to check
                if(check.valid && !visited.Contains(check)){
                    // valid node, add to poslist
                    posList.Add(check);
                    pos = new float2(0,0);
                    visited.Add(check);
                    regress = false;
                    break;
                }
            }

            if(regress){
                // if no valid node is returned we need to go to the prior node
                posList.RemoveAt(posList.Count-1);
                if(posList.Count == 0){
                    Destroy(gameObject);
                    // removes gameobject if path is impossible
                }
            }
        }
    }

    NavNode newValid(NavNode node){
        int2 tr = genDir(node.pos,goal.pos); 
        float2[] positions = {
            new float2(tr.x,0),
            new float2(0,tr.y),
            new float2(-tr.x,0),
            new float2(0,-tr.y),
            // no diagonals as they can clip through walls
        };
        foreach(float2 p in positions){
            float2 pos = node.pos + (p*mesh.grid.navSpacing);
            NavNode check = nearestNode(pos);
            if(check.valid){
                return check;
            }
        }
        return goal;
    }

    public int2 genDir(Vector2 startPos,Vector2 endPos){
        // produces the direction of travel to target
        int2 t = new int2(1,1);
        if(endPos.x < startPos.x){t.x = -1;}
        if(endPos.y < startPos.y){t.y = -1;}
        return t;
    }

    public NavNode nearestNode(Vector2 pos){
        // takes a world position and returns the closest node
        float spacing = mesh.grid.navSpacing;
        float width = mesh.grid.width/spacing;

        float remx = pos.x % spacing;
        float addx = 0;
        if(remx >= spacing/2){
            addx = spacing;
        }
        float remy = pos.y % spacing;
        float addy = 0;
        if(remy >= spacing/2){
            addy = spacing;
        }
        // rounds pos values to grid
        remx = pos.x - (pos.x%spacing) + addx;
        remy = pos.y - (pos.y%spacing) + addy;

        int ind = Mathf.RoundToInt(((remx/spacing)+((remy/spacing)*(width))));
        ind = Mathf.Clamp(ind,0,mesh.grid.navigationGrid.Length-1);
        return mesh.grid.navigationGrid[ind];
    }
}
