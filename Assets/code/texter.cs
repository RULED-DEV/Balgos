using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class texter : MonoBehaviour
{
    public string text;
    // string we want to write into letters

    string chars = "abcdefghijklmnopqrstuvwxyz,.";

    public List<Sprite> textSpr = new List<Sprite>();
    // text sprites

    public mat_vals MV;

    public List<mediun> textList = new List<mediun>();

    public void Start(){
        INIT();
    }

    public void INIT(){
        unit = FindObjectsByType<mesh_manager>(FindObjectsSortMode.None)[0].grid.spacing;
        sizing = FindObjectsByType<mesh_manager>(FindObjectsSortMode.None)[0].s.resolution;
        sizing = 1/sizing;
        Vector2 pos = transform.position;
        pos = new Vector2(pos.x - (pos.x%unit), pos.y - (pos.y%unit));
        transform.position = pos;
        TextGeneration();
    }

    public float size;
    float sizing;
    public float spacing;
    float unit;

    public void TextGeneration(){
        for(int i = 0; i < text.Length; i++){
            if(text[i] != ' '){
                Vector2 pos = transform.position + new Vector3(unit/2,unit,0);

                Vector2 offset = new Vector2(((3*unit) + (spacing*unit))*i,0);

                GameObject g = new GameObject();
                g.transform.parent = transform;
                g.transform.position = pos+(offset*size);
                g.transform.localScale = Vector3.one * sizing * size;

                SpriteRenderer s = g.AddComponent<SpriteRenderer>();
                
                int ind = chars.IndexOf(text[i]);
                s.sprite = textSpr[ind];

                PolygonCollider2D p = g.AddComponent<PolygonCollider2D>();
                p.isTrigger = true;

                mediun medium = g.AddComponent<mediun>();
                textList.Add(medium);
                medium.medium_val = MV;
                medium.width = size;
                medium.height = size;

                s.enabled = false;
            }
        }
    }

    public void TextRemoval(){
        foreach(mediun m in textList){
            m.resetNodeVal();
            m.OnApplicationQuit();
            Destroy(m.gameObject);
            // removes text
        }
    }
}
