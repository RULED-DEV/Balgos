using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class valReader : MonoBehaviour
{
    public button button;
    public GameObject obj;
    Camera cam;

    public Vector2 movePos;
    Vector2 initPos;

    public float sliderMaxDist;
    public float sliderMaxVal;
    public float sliderMinVal;

    public bool isSlider;

    public bool toggleOutput;
    public float valOutput;

    

    void Awake(){
        cam = FindObjectsByType<Camera>(FindObjectsSortMode.None)[0];
        initPos = obj.transform.position;
        button.transform.parent = null;
    }

    void updateVal(){
        if(isSlider){
            // slides slider
            Vector3 mousePosition = Input.mousePosition;
            mousePosition = cam.ScreenToWorldPoint(mousePosition);
            mousePosition = new Vector3(mousePosition.x,0,0);
            float val = Mathf.Clamp(mousePosition.x,initPos.x,initPos.x+sliderMaxDist);
            // clamps value to be within accepted range
            obj.transform.position = new Vector3(val,obj.transform.position.y,obj.transform.position.z);

            valOutput = Mathf.Abs(val-initPos.x) / sliderMaxDist;
            valOutput = sliderMinVal+(sliderMaxVal-sliderMinVal)*valOutput;
        }
        else{
            // toggles button
            if((Vector2)obj.transform.position == initPos){
                obj.transform.position = movePos;
                toggleOutput = false;
            }
            else{
                obj.transform.position = initPos;
                toggleOutput = true;
            }
        }
    }

    void setVal(bool inp1, float inp2){
        // used to set val and obj state/position
        if(isSlider){
            valOutput = inp2;
            float t = (valOutput - sliderMinVal) / (sliderMaxVal - sliderMinVal);
            float val = initPos.x + t * sliderMaxDist;
            obj.transform.position = new Vector3(val,obj.transform.position.y,obj.transform.position.z);
        }
        else{
            if(inp1){
                obj.transform.position = initPos;
                toggleOutput = true;
            }
            else{
                obj.transform.position = movePos;
                toggleOutput = false;
            }
        }
    }
}
