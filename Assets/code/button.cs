using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button : MonoBehaviour
{

    public string ACTSTR;
    // used to call function in another script
    public MonoBehaviour Conn;
    // calls function from here

    public bool levelTransition; // determines if we are activating a function or transitioning

    public bool canHold;
    // wether button continues to activate when held

    public bool held;

    void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0)){
            held = true;
        }
    }

    void Update(){
        if(held){
            if(levelTransition){
                if(!canHold) Debug.Log("going to level " + ACTSTR);
                // Conn in this case is the gameStateManager
                gameStateManager m = FindObjectsByType<gameStateManager>(FindObjectsSortMode.None)[0];
                m.menuScene = ACTSTR;
                m.toMenu = true;
                m.StopAllSources();
                // here we override the menu with the string, allowing us to go to specific levels/scenes
            }
            else{
                if(!canHold) Debug.Log("activating function " + ACTSTR);
                Conn.Invoke(ACTSTR,0);
            }
        }
        if(Input.GetMouseButtonUp(0) || canHold == false){
            held = false;
        }
    }
}
