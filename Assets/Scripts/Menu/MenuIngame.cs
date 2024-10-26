using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MenuInGame : MonoBehaviour
{
    public GameObject pausa;
    public GameObject opciones;
    public GameObject menu;

    private AudioManager audioManager;
    private AudioSource musicaSource;
    private AudioSource efectosSource;

    public static bool juegoPausado = false;
    private bool menuOpcionesOpen = false;

    void Awake()
    {
        // Buscar AudioSource en el objeto que no se destruye al cambiar de escena
        GameObject audioObject = GameObject.Find("AudioManager");
        if (audioObject != null)
        {
            audioManager = audioObject.GetComponent<AudioManager>();

            // Buscar AudioSource para la música
            Transform musicChild = audioObject.transform.Find("Music");
            if (musicChild != null)
            {
                musicaSource = musicChild.GetComponent<AudioSource>();
            }
            else
            {
                Debug.LogError("No se encontró el AudioSource para música.");
            }

            // Buscar AudioSource para efectos de sonido
            Transform sfxChild = audioObject.transform.Find("SFX");
            if (sfxChild != null)
            {
                efectosSource = sfxChild.GetComponent<AudioSource>();
            }
            else
            {
                Debug.LogError("No se encontró el AudioSource para efectos de sonido.");
            }
        }
        else
        {
            Debug.LogError("No se encontró el objeto que no se destruye al cambiar de escena.");
        }
    }



    void Start()
    {
        // Asignar métodos de control a los sliders
        // Supongo que tienes sliders para el volumen de la música y los efectos
        Slider[] sliders = GetComponentsInChildren<Slider>(true);
        foreach (Slider slider in sliders)
        {
            if (slider.CompareTag("Musica"))
            {
                slider.onValueChanged.AddListener(ActualizarVolumenMusica);
            }
            else if (slider.CompareTag("Efectos"))
            {
                slider.onValueChanged.AddListener(ActualizarVolumenEfectos);
            }
        }
    }

    void Update()
    {
        if (menuOpcionesOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            Volver();
        }        
        else if (!menuOpcionesOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado)
            {                
                Continuar();
            }
            else
            {
                //Se muestra el cursor y se desbloquea
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Pausar();
            }
        }
    }

    void Pausar()
    {
        Time.timeScale = 0f; // Pausa el tiempo en el juego
        pausa.SetActive(true); // Activa el menú de pausa
        juegoPausado = true;
    }

    // Método para continuar el juego
    public void Continuar()
    {
        //Se oculta el cursor y se bloquea en medio de la pantalla
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pausa.SetActive(false); // Desactiva el menú de pausa
        Time.timeScale = 1f; // Reanuda el tiempo en el juego
        juegoPausado = false;
    }

    public void AbrirOpciones()
    {
        menu.SetActive(false);
        opciones.SetActive(true);
        menuOpcionesOpen = true;
    }

    // Método para cerrar la escena actual y abrir la escena "Menu"
    public void AbrirMenu()
    {
        juegoPausado = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu"); // Carga la escena llamada "Menu"
    }
    public void Volver()
    {
        opciones.SetActive(false);
        menu.SetActive(true);
        menuOpcionesOpen = false;
    }

    public void ActualizarVolumenMusica(float volumen)
    {
        if (musicaSource != null)
        {
            musicaSource.volume = volumen;
        }
    }

    // Método para actualizar el volumen de los efectos de sonido
    public void ActualizarVolumenEfectos(float volumen)
    {
        if (efectosSource != null)
        {
            efectosSource.volume = volumen;
        }
    }
}


