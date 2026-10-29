using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class save_data{ // saves data

    public static void SAVE (setting_vals data){
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Player.Data";
        FileStream stream = new FileStream(path, FileMode.Create);

        setting_vals data_stored = data;

        formatter.Serialize(stream, data_stored);
        stream.Close(); 
        Debug.Log("saved");
    } 

    public static setting_vals LOAD () {
        string path = Application.persistentDataPath + "/Player.Data";
        if (File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            setting_vals data_stored = formatter.Deserialize(stream) as setting_vals;
            stream.Close(); 
            Debug.Log("loaded");
            return data_stored;
        }
        else{
            Debug.LogError("error.File not found :(");
            return null;
        }
        
    }


}
