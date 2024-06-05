using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDoors : MonoBehaviour
{
    [SerializeField] private GameObject boss;

    public static bool firstBossWon;

    void Start()
    {
        boss = GameObject.FindGameObjectWithTag(GameTags.boss);

        if (PlayerPrefs.GetInt(GameTags.firstBossDefeated) == 1)
        {
            Destroy(boss);
            Destroy(gameObject);
        }
    }


    void Update()
    {        
        if (boss == null)
        {
            PlayerPrefs.SetInt(GameTags.firstBossDefeated, 1);
            Destroy(gameObject);
        }
    }
}
