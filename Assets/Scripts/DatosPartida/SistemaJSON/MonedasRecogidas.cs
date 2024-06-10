using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class MonedasRecogidas
{
    private static MonedasRecogidas _instance;
    public static MonedasRecogidas Instance
    {
        get //Cada vez que se accede a la Instance, se ejectua este código
        {
            if (_instance == null)
            {
                _instance = new MonedasRecogidas();
                _instance.LoadData();
            }
            return _instance;
        }
    }

    public List<string> coins = new List<string>();

    public void SaveData()
    {
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(Application.persistentDataPath + "/monedasRecogidas.json", json);
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/monedasRecogidas.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            MonedasRecogidas data = JsonUtility.FromJson<MonedasRecogidas>(json);
            coins = data.coins;
        }
    }

    public void ClearData()
    {
        string path = Application.persistentDataPath + "/monedasRecogidas.json";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        coins.Clear();
    }
}
