using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

public class mesh_manager : MonoBehaviour
{
    public noise_grid grid;

    public mat_vals air;
    public setting_vals s;

    float refrate;
    int frame = 0;

    public SpriteRenderer soundrenderer;

    public GameObject cursPrefab;

    Texture2D soundmap;
    NativeArray<Color32> soundColors;

    public col_rank[] coloursequence;

    public void Awake(){
        
        // settings init
        if(s.ManualSaveCheck) s.saveVals();
        s.loadVals();
        setSeq(s.colVals);
        Application.targetFrameRate = s.frameRate;
        if(s.cursorInLevel && FindObjectsByType<mesh_manager>(FindObjectsSortMode.None).Length == 0){
            // spawn cursor
            Instantiate(cursPrefab);
        }
        
        Cursor.visible = s.seeCursor;
        // sets cursor to be on/off depending on seeCursor

        // loads setting data

        if(s != null){
            soundColors = new NativeArray<Color32>(Mathf.RoundToInt((s.width/s.resolution)*(s.height/s.resolution)), Allocator.Persistent);
            soundmap = new Texture2D(Mathf.RoundToInt(s.width/s.resolution),Mathf.RoundToInt(s.height/s.resolution), TextureFormat.RGBA32, false);
            soundmap.wrapMode = TextureWrapMode.Clamp;
            soundmap.filterMode = FilterMode.Point;
            
            grid.construct_grid(air,s.resolution);
            // initialises grid

            Debug.Log("starts");
        }
        else{
            Invoke("Awake",0);
        }
        
    }

    public void FixedUpdate(){
        if(Time.time > refrate){ // really expensive, rendering costs MASSIVE
            // act nodes contribute heavily to rendering costs 
            // refreshes the screen
            refrate = Time.time + s.refreshrate;
            grid.updateMap(frame);
            UpdatesoundmapTexture();
            ApplysoundmapToRenderer();
            frame ++ ;
        }
    }

    public void OnApplicationQuit(){
        grid.remove_grid();
        if(soundColors.IsCreated) soundColors.Dispose();
        // unloads grid
    }

    void UpdatesoundmapTexture()
    {
        NativeArray<col_rank> colseq = new NativeArray<col_rank>(coloursequence.Length, Allocator.TempJob);
        for(int i = 0; i < coloursequence.Length; i++){
            colseq[i] = coloursequence[i];
        }
        var job = new DecibelsToColorJob
        {
            NodeArr = grid.Nodes,
            basecol = new Color32(0, 0, 0, 0), // background color
            colranks = colseq,

            colors = soundColors,

            gridwidth = Mathf.RoundToInt(grid.width/grid.spacing),
            colourwidth = Mathf.RoundToInt(s.width/s.resolution),
            spacing = grid.spacing,
            initpos = soundrenderer.transform.position,
        };

        job.Schedule(soundColors.Length, 512).Complete();
        soundmap.SetPixelData(soundColors, 0);
        soundmap.Apply(false, false);
        colseq.Dispose();
    }

    void ApplysoundmapToRenderer()
    {
        Sprite sprite = Sprite.Create(
            soundmap,
            new Rect(0, 0, (s.width/s.resolution), (s.height/s.resolution)), // size in pixels
            new Vector2(0f, 0f), // pivot
            pixelsPerUnit: 100
        );
        soundrenderer.transform.localScale = new Vector2(1/s.resolution,1/s.resolution);
        soundrenderer.sprite = sprite;
    }

    void setSeq(savCol[] colVals){
        for(int i = 0; i < colVals.Length; i++){
            coloursequence[i].col = new Color{
                r = colVals[i].R,
                g = colVals[i].G,
                b = colVals[i].B,
                a = 1
            };
        }
        // takes savCol from settings and applies it to coloursequence
    }

    void setCol(){
        for(int i = 0; i < coloursequence.Length; i++){
            s.colVals[i].R = coloursequence[i].col.r;
            s.colVals[i].G = coloursequence[i].col.g;
            s.colVals[i].B = coloursequence[i].col.b;
        }
        // takes coloursequence and applies it to settings
    }
}