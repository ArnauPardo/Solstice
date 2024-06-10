using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CurasRecogidas
{
    private static CurasRecogidas _instance;
    public static CurasRecogidas Instance
    {
        get //Cada vez que se accede a la Instance, se ejectua este código
        {
            if (_instance == null)
            {
                _instance = new CurasRecogidas();
            }
            return _instance;
        }
    }

    public List<string> curas = new List<string>();


    public void ClearData()
    {
        curas.Clear();
    }
}
