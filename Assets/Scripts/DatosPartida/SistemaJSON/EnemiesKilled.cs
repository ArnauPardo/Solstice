using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EnemiesKilled
{
    private static EnemiesKilled _instance;
    public static EnemiesKilled Instance
    {
        get //Cada vez que se accede a la Instance, se ejectua este código
        {
            if (_instance == null)
            {
                _instance = new EnemiesKilled();
            }
            return _instance;
        }
    }

    public List<string> enemies = new List<string>();


    public void ClearData()
    {
        enemies.Clear();
    }
}
