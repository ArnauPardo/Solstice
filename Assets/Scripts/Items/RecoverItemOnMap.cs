using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverItemOnMap : MonoBehaviour
{
    public static RecoverItemOnMap Instance;

    AudioManager audioManager;

    [SerializeField] private string id;

    private PlayerHealth playerHealth;
    [SerializeField] private GameObject particulasCurar;
    protected GameObject particulasClon;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag(GameTags.audio).GetComponent<AudioManager>();
        playerHealth = GameObject.FindGameObjectWithTag(GameTags.player).GetComponent<PlayerHealth>();

        if (CurasRecogidas.Instance.curas.Contains(id))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameTags.player) && PlayerHealth.health < playerHealth.maxHealth)
        {
            RecoverLife(collision);
        }
    }

    public void RecoverLife(Collider2D col)
    {
        PlayerHealth.health++;
        particulasClon = (GameObject)Instantiate(particulasCurar, col.gameObject.transform.position, Quaternion.identity);
        audioManager.PlaySFX(audioManager.Heal);
        CurasRecogidas.Instance.curas.Add(id);
        Destroy(gameObject);
        for (int i = 0; i < playerHealth.hearts.Length; i++)
        {
            if (PlayerHealth.health > playerHealth.maxHealth)
            {
                PlayerHealth.health = playerHealth.maxHealth;
            }
            else
            {
                if (i < PlayerHealth.health)
                {

                    playerHealth.UpdateHeartsUI();
                }
            }
        }
    }
}
