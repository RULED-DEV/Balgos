using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[System.Serializable]
public class setting_vals {

    public float width; // how many pixels wide do we want it
    public float height; // how many pixels talll do we want it
    public float resolution; // multiplies pixel amounts

    public savCol[] colVals;
    // defines sound colors

    public bool ManualSaveCheck;
    // used to save settings from inspector

    public void saveVals(){
        save_data.SAVE(this);
    }

    public void loadVals(){
        setting_vals s = save_data.LOAD();
        width = s.width;
        height = s.height;
        resolution = s.resolution;
        colVals = s.colVals;
    }
}