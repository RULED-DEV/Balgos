using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.SceneManagement;

public class gameStateManager : MonoBehaviour
{

    mesh_manager mesh;

    public bool transition;
    public string menuScene;
    public string transitionScene;
    public string[] levelScenes;

    public bool goToLevel;
    // if true we go to a new level, else we go to transition
    public bool toMenu;
    // if true, quit to menu

    void Awake(){
        mesh = FindObjectsByType<mesh_manager>(FindObjectsSortMode.None)[0];
    }

    void Update(){
        if(transition){
            // we be transitioning
            if(mesh.grid.activeNodes.Length == 0){
                // no noise in the world
                removeAll();
                if(toMenu){
                    SceneManager.LoadScene(menuScene);
                }
                else if(goToLevel){
                    SceneManager.LoadScene(levelScenes[Random.Range(0,levelScenes.Length)]);
                }
                else{
                    SceneManager.LoadScene(transitionScene);
                }
                
            }
        }
    }

    void removeAll(){
        FindObjectsByType<mesh_manager>(FindObjectsSortMode.None)[0].OnApplicationQuit();
        mediun[] m = FindObjectsByType<mediun>(FindObjectsSortMode.None);
        foreach(mediun M in m){M.resetNodeVal();M.OnApplicationQuit();}
    }

    public void StopAllSources(){
        act_updater[] a = FindObjectsByType<act_updater>(FindObjectsSortMode.None);
        noiser[] n = FindObjectsByType<noiser>(FindObjectsSortMode.None);
        SoundSwitch[] s = FindObjectsByType<SoundSwitch>(FindObjectsSortMode.None);
        BALGOS[] b = FindObjectsByType<BALGOS>(FindObjectsSortMode.None);
        enemy[] e = FindObjectsByType<enemy>(FindObjectsSortMode.None);
        mediun[] m = FindObjectsByType<mediun>(FindObjectsSortMode.None);
        mesh.grid.active = true;

        foreach(act_updater A in a){A.enabled = false;}
        foreach(noiser N in n){N.enabled = false;}
        foreach(SoundSwitch S in s){S.enabled = false;}
        foreach(BALGOS B in b){B.enabled = false;}
        foreach(enemy E in e){Destroy(E.gameObject);}
        foreach(mediun M in m){M.resetNodeVal();M.OnApplicationQuit();}

        transition = true;
    }

    public void QUIT(){Application.Quit();}
}
