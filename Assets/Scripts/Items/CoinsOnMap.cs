using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsOnMap : MonoBehaviour
{

    //Puntos que obtienes al coger una esfera de luz peque�a.
    [Header("Caracter�sticas al tocar las esferas")]
    [SerializeField] private int coinValue = 1;
    private GameObject puntuacion;
    [SerializeField] private GameObject particulasExplosion;
    protected GameObject particulasClon;
    AudioManager audioManager;

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
            particulasClon = (GameObject)Instantiate(particulasExplosion, collision.gameObject.transform.position, Quaternion.identity);

            Destroy(particulasClon, 2.0f);
            Destroy(this.gameObject);
        }
    }
}
