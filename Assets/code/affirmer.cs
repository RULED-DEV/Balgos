using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class affirmer : MonoBehaviour
{
    public GameObject wordTemplate;

    public Transform[] wordsPos; // list of words that can be selected
    public string[] affirmations; // all the words that can appear on screen

    public float MaxTime; // total time of the transition

    Vector2 savPos; // last position a words was placed at

    void Awake(){
        Debug.Log("affirmed");
        float amount = Random.Range(1,5);
        List<Transform> tL = new List<Transform>();
        List<string> aL = new List<string>();
        for(int i = 0; i < amount; i++){
            float time = MaxTime/amount;
            
            bool word = false;
            bool text = false;
            while(!word){
                Transform w = wordsPos[Random.Range(0,wordsPos.Length)];
                // loops through the words list until a new word is found
                if(!tL.Contains(w)){
                    texter newWord = Instantiate(wordTemplate,w.position,w.rotation).GetComponent<texter>();
                    newWord.initDelay = 0;
                    newWord.decayDelay = MaxTime;
                    // word is assigned its init/decay times
                    tL.Add(w);
                    word = true;
                    while(!text){
                        // loops through the affirmations until it finds an unused one
                        string a = affirmations[Random.Range(0,affirmations.Length)];
                        if(!aL.Contains(a)){
                            newWord.text = a;
                            // assigns affirmation
                            aL.Add(a);
                            text = true;
                            newWord.INIT();
                        }
                    }
                }
            }
        }
        Invoke("transition",MaxTime);
    }

    void transition(){
        // causes scene to swap
        FindObjectsByType<gameStateManager>(FindObjectsSortMode.None)[0].StopAllSources();
    }

}
