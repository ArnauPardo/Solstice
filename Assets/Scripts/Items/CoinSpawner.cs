using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinSpawner : MonoBehaviour
{
    [Header("Fuerzas al Dropearse")]
    [SerializeField] private float initialForce = 2f;  // Fuerza inicial del rebote
    [SerializeField] private float torqueForce = 1f;   // Fuerza de torque para girar la moneda
    [SerializeField] private float lifeTime = 10f;     // Tiempo de vida de la moneda antes de destruirse

    //Puntos que obtienes al coger una esfera de luz peque�a.
    [Header("Caracter�sticas al tocar las esferas")]
    [SerializeField] private int coinValue = 1;
    private GameObject puntuacion;
    [SerializeField] private GameObject particulasExplosion;
    protected GameObject particulasClon;

    private Rigidbody2D rb;
    AudioManager audioManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        puntuacion = GameObject.FindGameObjectWithTag(GameTags.puntuacionText);

        // Aplicar una fuerza inicial para el rebote
        rb.AddForce(new Vector2(Random.Range(-1f, 1f), 1f) * initialForce, ForceMode2D.Impulse);

        // Aplicar un torque para hacerla girar un poco
        rb.AddTorque(Random.Range(-torqueForce, torqueForce));

        // Destruir la moneda despu�s de un tiempo para evitar que se acumulen en la escena
        Destroy(gameObject, lifeTime);
        audioManager = GameObject.FindGameObjectWithTag(GameTags.audio).GetComponent<AudioManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(GameTags.player))
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
