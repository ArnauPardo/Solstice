using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataGameManager : MonoBehaviour
{

    public static DataGameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }else if (Instance != null)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CurrentScene(string value)
    {
        PlayerPrefs.SetString(GameTags.savedScene, value);
    }

    public void CurrentPosition(float valueX, float valueY, float valueZ)
    {
        PlayerPrefs.SetFloat(GameTags.playerXPosition, valueX);
        PlayerPrefs.SetFloat(GameTags.playerYPosition, valueY);
        PlayerPrefs.SetFloat(GameTags.playerZPosition, valueZ);
    }

    public void CurrentHearths(int value)
    {
        PlayerPrefs.SetInt(GameTags.savedHealth, value);
    }
    public void CurrentMoney(int value)
    {
        PlayerPrefs.SetInt(GameTags.savedMoney, value);
    }
}
