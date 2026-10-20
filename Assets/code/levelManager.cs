using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelManager : MonoBehaviour
{

    public float enemySpawnPeriod; // how frequently enemies should spawn
    float spawnTimer;

    public int initEnemySpawnAmount; // when starting, how many enemies should spawn

    public BALGOS player;

    public List<enemy> enemyAct = new List<enemy>(); // all enemies on the field
    public GameObject enemyTemplate; // base enemy to be spawned in

    public List<enemy> goal = new List<enemy>(); // all alive goals
    public GameObject goalTemplate; // base template for goal enemy

    public void StartLevel(){
        player = GameObject.Find("BALGOS").GetComponent<BALGOS>();
        // retrieves player
        for(int i = 0; i < initEnemySpawnAmount; i++){
            spawnTimer = Time.time + enemySpawnPeriod;
            enemyAct.Add(Instantiate(enemyTemplate,transform.position,transform.rotation).GetComponent<enemy>());
            // spawns enemy and adds it to the list

            enemyAct[enemyAct.Count-1].transform.position = enemyAct[enemyAct.Count-1].setRandPos(player.transform.position);
            // enemy is set to a valid position that is away from the player
        }
        // spawns in initial enemies
    }

    void Update()
    {
        while(spawnTimer < Time.time){
            spawnTimer = Time.time + enemySpawnPeriod;
            enemyAct.Add(Instantiate(enemyTemplate,transform.position,transform.rotation).GetComponent<enemy>());
            // spawns enemy and adds it to the list

            enemyAct[enemyAct.Count-1].transform.position = enemyAct[enemyAct.Count-1].setRandPos(player.transform.position);
            // enemy is set to a valid position that is away from the player
        }
    }

    public void removeEnemy(enemy E){
        int ind = -1;
        ind = enemyAct.IndexOf(E);
        if(ind >= 0){enemyAct.RemoveAt(ind);}
        ind = goal.IndexOf(E);
        if(ind >= 0){goal.RemoveAt(ind);}
        if(goal.Count == 0){
            // victory achieved move to next level
        }
    }
}
