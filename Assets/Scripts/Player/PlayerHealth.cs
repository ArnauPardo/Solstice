using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static int health;
    public int maxHealth;
    public RawImage[] hearts;
    public Texture fullHeartTexture;
    public Texture emptyHeartTexture;

    public static PlayerHealth Instance;

    void Start()
    {
        maxHealth = PlayerPrefs.GetInt(GameTags.maxHearths, 5);
        Instance = this;
        UpdateHeartsUI();
    }

    public void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].texture = fullHeartTexture;
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].texture = emptyHeartTexture;
                hearts[i].enabled = i < maxHealth;
            }
        }
    }

    void Update()
    {
        if (health > maxHealth)
        {
            health = maxHealth;
        }
        UpdateHeartsUI();
    }
}

