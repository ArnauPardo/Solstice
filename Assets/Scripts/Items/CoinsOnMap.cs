using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsOnMap : MonoBehaviour
{
    [SerializeField] private string id;

    [Header("Características al tocar las esferas")]
    [SerializeField] private int coinValue = 1;
    private GameObject puntuacion;
    [SerializeField] private GameObject particulasExplosion;
    protected GameObject particulasClon;
    AudioManager audioManager;

    private void Awake()
    {        
        if (MonedasRecogidas.Instance.coins.Contains(id))
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        puntuacion = GameObject.FindGameObjectWithTag(GameTags.puntuacionText);
        audioManager = GameObject.FindGameObjectWithTag(GameTags.audio).GetComponent<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameTags.player))
        {
            audioManager.PlaySFX(audioManager.Coin);
            Settings.dinero += coinValue;
            puntuacion.GetComponent<Text>().text = "" + Settings.dinero;
            particulasClon = Instantiate(particulasExplosion, collision.gameObject.transform.position, Quaternion.identity);
            MonedasRecogidas.Instance.coins.Add(id);

            Destroy(particulasClon, 2.0f);
            Destroy(this.gameObject);
        }
    }
}
