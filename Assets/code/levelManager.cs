using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelManager : MonoBehaviour
{

    public float enemySpawnPeriod; // how frequently enemies should spawn
    float spawnTimer;


    public BALGOS player;

    public List<enemy> enemyAct = new List<enemy>(); // all enemies on the field
    public GameObject enemyTemplate; // base enemy to be spawned in

    public List<enemy> goal = new List<enemy>(); // all alive goals
    public GameObject goalTemplate; // base template for goal enemy

    int goalState; // 1 : pause, 2 : rage, 3 : silence, 4 : transition
    float goalTimer;

    public void StartLevel(){
        player = GameObject.Find("BALGOS").GetComponent<BALGOS>();
        // retrieves player
        int initEnemySpawnAmount = Random.Range(2,5);
        int initGoalSpawnAmount = Random.Range(2,5);

        for(int i = 0; i < initEnemySpawnAmount; i++){
            spawnTimer = Time.time + enemySpawnPeriod;
            enemyAct.Add(Instantiate(enemyTemplate,transform.position,transform.rotation).GetComponent<enemy>());
            // spawns enemy and adds it to the list

            enemyAct[enemyAct.Count-1].transform.position = enemyAct[enemyAct.Count-1].setRandPos(player.transform.position);
            // enemy is set to a valid position that is away from the player
        }
        // spawns in initial enemies

        for(int i = 0; i < initGoalSpawnAmount; i++){
            goal.Add(Instantiate(goalTemplate,transform.position,transform.rotation).GetComponent<enemy>());
            // spawns enemy and adds it to the list

            goal[goal.Count-1].transform.position = goal[goal.Count-1].setRandPos(player.transform.position);
            // enemy is set to a valid position that is away from the player
        }
        // spawns in goal
    }

    void Update()
    {
        if(spawnTimer < Time.time){
            spawnTimer = Time.time + enemySpawnPeriod;
            enemyAct.Add(Instantiate(enemyTemplate,transform.position,transform.rotation).GetComponent<enemy>());
            // spawns enemy and adds it to the list

            enemyAct[enemyAct.Count-1].transform.position = enemyAct[enemyAct.Count-1].setRandPos(player.transform.position);
            // enemy is set to a valid position that is away from the player
        }
        if(goalState != 0){
            // all goal enemies dead, initiate leaving level
            if(goalTimer < Time.time){
                goalState ++ ; // goes to next state
                if(goalState == 2){ // rage, all anemies attack player
                    // rage state lasts longer
                    goalTimer = Time.time + 20;
                    spawnTimer = 0;
                    enemySpawnPeriod = 10;
                    // faster spawns
                    foreach(enemy e in enemyAct){e.enraged = true;}
                }
                if(goalState == 3){ // silence, all enemies dissapear
                    goalTimer = Time.time + 10;
                    foreach(enemy e in enemyAct){
                        removeEnemy(e);
                    }
                }
                if(goalState == 4){ // transition is triggered
                    gameObject.GetComponent<gameStateManager>().StopAllSources();
                } 
            }
        }
    }

    public void removeEnemy(enemy E){
        int ind = -1;
        ind = enemyAct.IndexOf(E);
        if(ind >= 0){enemyAct.RemoveAt(ind);}
        ind = goal.IndexOf(E);
        if(ind >= 0){goal.RemoveAt(ind);}
        Destroy(E.gameObject);
        if(goal.Count == 0){
            goalState = 1; // pause
            goalTimer = Time.time + 10;
        }
    }
}
