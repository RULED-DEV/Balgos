using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class settingsSetter : MonoBehaviour
{

    public mesh_manager mesh;

    public valReader[] vals;
    // no collision
    // immortal
    // infinite ammo
    // no aggro
    // aud effect sim
    // mouse toggle
    // cursor in sim

    // menu aud
    // player aud
    // enemy aud
    // ambient aud
    // frame rate
    // sim rate

    // high 
    // high g
    // high b
    // med r
    // med g
    // med b
    // low r
    // low g
    // low b
    // amb r
    // amb g
    // amb b

    void Start(){
        Invoke("INIT",0.1f);
    }

    void INIT(){
        vals = GetComponentsInChildren<valReader>();
        // grabs every valreader in order above
        setvalReaderVals();
        // inits the valreaders to correct position
    }

    public void ApplySettings(){
        // takes vals from val reader and applies to setting vals
        mesh.s.noCollision = vals[0].toggleOutput;
        mesh.s.cannotDie = vals[1].toggleOutput;
        mesh.s.infiniteAmmo = vals[2].toggleOutput;
        mesh.s.enemiesCannotAggro = vals[3].toggleOutput;
        mesh.s.AudioEffectsSimulation = vals[4].toggleOutput;
        mesh.s.seeCursor = vals[5].toggleOutput;
        mesh.s.cursorInLevel = vals[6].toggleOutput;

        mesh.s.menuAud = vals[7].valOutput;
        mesh.s.playerAud = vals[8].valOutput;
        mesh.s.enemyAud = vals[9].valOutput;
        mesh.s.mouseAud = vals[10].valOutput;

        mesh.s.frameRate = (int)vals[11].valOutput;
        mesh.s.refreshrate = vals[12].valOutput;

        mesh.s.colVals[3].R = vals[13].valOutput;
        mesh.s.colVals[3].G = vals[14].valOutput;
        mesh.s.colVals[3].B = vals[15].valOutput;

        mesh.s.colVals[2].R = vals[16].valOutput;
        mesh.s.colVals[2].G = vals[17].valOutput;
        mesh.s.colVals[2].B = vals[18].valOutput;

        mesh.s.colVals[1].R = vals[19].valOutput;
        mesh.s.colVals[1].G = vals[20].valOutput;
        mesh.s.colVals[1].B = vals[21].valOutput;

        mesh.s.colVals[0].R = vals[22].valOutput;
        mesh.s.colVals[0].G = vals[23].valOutput;
        mesh.s.colVals[0].B = vals[24].valOutput;
        
        mesh.s.saveVals();
        mesh.refreshSettings();
        // saves settings and applies them
    }

    public void setvalReaderVals(){
        mesh.s.loadVals();
        mesh.refreshSettings();
        // makes sure setting vals is reset

        vals[0].setVal(mesh.s.noCollision,0);
        vals[1].setVal(mesh.s.cannotDie,0);
        vals[2].setVal(mesh.s.infiniteAmmo,0);
        vals[3].setVal(mesh.s.enemiesCannotAggro,0);
        vals[4].setVal(mesh.s.AudioEffectsSimulation,0);
        vals[5].setVal(mesh.s.seeCursor,0);
        vals[6].setVal(mesh.s.cursorInLevel,0);

        vals[7].setVal(false,mesh.s.menuAud);
        vals[8].setVal(false,mesh.s.playerAud);
        vals[9].setVal(false,mesh.s.enemyAud);
        vals[10].setVal(false,mesh.s.mouseAud);

        vals[11].setVal(false,mesh.s.frameRate);
        vals[12].setVal(false,mesh.s.refreshrate);

        vals[13].setVal(false,mesh.s.colVals[3].R);
        vals[14].setVal(false,mesh.s.colVals[3].G);
        vals[15].setVal(false,mesh.s.colVals[3].B);

        vals[16].setVal(false,mesh.s.colVals[2].R);
        vals[17].setVal(false,mesh.s.colVals[2].G);
        vals[18].setVal(false,mesh.s.colVals[2].B);

        vals[19].setVal(false,mesh.s.colVals[1].R);
        vals[20].setVal(false,mesh.s.colVals[1].G);
        vals[21].setVal(false,mesh.s.colVals[1].B);

        vals[22].setVal(false,mesh.s.colVals[0].R);
        vals[23].setVal(false,mesh.s.colVals[0].G);
        vals[24].setVal(false,mesh.s.colVals[0].B);

        // resets all val readers to settings state
    }
}
