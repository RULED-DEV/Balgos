using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[System.Serializable]
public class setting_vals {

    public float width; // how many pixels wide do we want it
    public float height; // how many pixels talll do we want it
    

    public float resolution; // multiplies pixel amounts
    public float refreshrate; // how eften we refresh the soundmap

    public float menuAud;
    public float playerAud;
    public float enemyAud;
    public float mouseAud;
    // audio levels for different aspects of the game

    public bool cursorInLevel; // does cursor make noise in levels
    public bool seeCursor; // do you want to see the cursor
    public bool AudioEffectsSimulation; // does the Aud variables effect the noise written to the grid

    public bool cannotDie; // is player invinsible
    public bool infiniteAmmo; // does weapon need to reload
    public bool noCollision; // does player phase through walls 

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

        refreshrate = s.refreshrate;

        menuAud = s.menuAud;
        playerAud = s.playerAud;
        enemyAud = s.enemyAud;
        mouseAud = s.mouseAud;

        cursorInLevel = s.cursorInLevel;
        seeCursor = s.seeCursor;
        AudioEffectsSimulation = s.AudioEffectsSimulation;

        cannotDie = s.cannotDie;
        infiniteAmmo = s.infiniteAmmo;
        noCollision = s.noCollision;

        colVals = s.colVals;
    }
}