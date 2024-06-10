using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.Rendering.DebugUI;
using Button = UnityEngine.UI.Button;

public class MenuOpciones : MonoBehaviour
{
    public GameObject menu;
    public GameObject opciones;
    public TMP_Dropdown resolucionDropdown;
    public Slider sliderMusica;
    public Slider sliderEfectos;
    AudioManager audioManager;

    private Resolution[] resoluciones;
    private AudioSource musicaSource;
    private AudioSource efectosSource;

    [SerializeField] private TextMeshProUGUI continuarText;
    [SerializeField] private Button botonContinuar;

    private void Awake()
    {
        botonContinuar.GetComponent<Button>().interactable = false;
        audioManager = GameObject.FindGameObjectWithTag(GameTags.audio).GetComponent<AudioManager>();
        Settings.newGame = false;
        Settings.gameContinue = false;

        // Buscar AudioSource en el objeto que no se destruye al cambiar de escena
        GameObject audioObject = GameObject.Find("AudioManager");
        if (audioObject != null)
        {
            musicaSource = audioObject.GetComponentInChildren<AudioSource>(true); // true para buscar en objetos inactivos
            efectosSource = audioObject.GetComponentsInChildren<AudioSource>(true)[1]; // el segundo AudioSource es para efectos
        }
        else
        {
            Debug.LogError("No se encontró el objeto que no se destruye al cambiar de escena.");
        }
    }

    void Start()
    {
        audioManager.MusicaMenu();
        resoluciones = Screen.resolutions;

        resolucionDropdown.ClearOptions();
        List<string> opcionesRes = new List<string>();
        int resolucionActual = GetCurrentResolutionIndex();

        foreach (var resolucion in resoluciones)
        {
            string opcion = $"{resolucion.width} x {resolucion.height}";
            opcionesRes.Add(opcion);
        }

        resolucionDropdown.AddOptions(opcionesRes);
        resolucionDropdown.value = resolucionActual;
        resolucionDropdown.RefreshShownValue();

        // Asignar métodos de control a los sliders
        sliderMusica.onValueChanged.AddListener(ActualizarVolumenMusica);
        sliderEfectos.onValueChanged.AddListener(ActualizarVolumenEfectos);

        if (PlayerPrefs.HasKey(GameTags.savedHealth))
        {
            continuarText.color = Color.white;
            botonContinuar.interactable = true;
        }
    }

    int GetCurrentResolutionIndex()
    {
        Resolution currentResolution = Screen.currentResolution;
        for (int i = 0; i < resoluciones.Length; i++)
        {
            if (resoluciones[i].width == currentResolution.width && resoluciones[i].height == currentResolution.height)
            {
                return i;
            }
        }
        return 0; // Si no se encuentra la resolución actual, devolver la primera resolución
    }

    public void Continuar()
    {
        audioManager.PararMusica();
        Settings.gameContinue = true;
        PlayerHealth.health = PlayerPrefs.GetInt(GameTags.savedHealth);
        PlayerPrefs.SetInt(GameTags.currentHealth, PlayerPrefs.GetInt(GameTags.savedHealth));
        Settings.dinero = PlayerPrefs.GetInt(GameTags.savedMoney);
        PlayerPrefs.SetString(GameTags.lastScene, SceneManager.GetActiveScene().name);
        CurasRecogidas.Instance.ClearData();
        SceneManager.LoadScene(PlayerPrefs.GetString(GameTags.savedScene));
        audioManager.Nivel1();

    }

    public void Jugar()
    {
        audioManager.PararMusica();
        PlayerPrefs.DeleteAll();
        MonedasRecogidas.Instance.ClearData();
        CurasRecogidas.Instance.ClearData();
        Settings.newGame = true;
        PlayerPrefs.SetInt(GameTags.maxHearths, 5);
        PlayerPrefs.SetInt(GameTags.currentMoney, 0);
        PlayerHealth.health = PlayerPrefs.GetInt(GameTags.maxHearths);
        Settings.dinero = PlayerPrefs.GetInt(GameTags.currentMoney);
        SceneManager.LoadScene(GameTags.intro);
    }

    public void SetResolucion(int indiceResolucion)
    {
        if (resoluciones != null && indiceResolucion >= 0 && indiceResolucion < resoluciones.Length)
        {
            Resolution resolucion = resoluciones[indiceResolucion];
            Screen.SetResolution(resolucion.width, resolucion.height, Screen.fullScreen);
        }
    }

    public void Salir()
    {
        Application.Quit();
    }

    public void AbrirOpciones()
    {
        menu.SetActive(false);
        opciones.SetActive(true);
    }

    public void CerrarOpciones()
    {
        menu.SetActive(true);
        opciones.SetActive(false);
    }

    public void CalidadGrafica(int indiceCalidad)
    {
        QualitySettings.SetQualityLevel(indiceCalidad);
    }

    public void PantallaCompleta(bool pantallaCompletaActivada)
    {
        Screen.fullScreen = pantallaCompletaActivada;
    }

    // Método para actualizar el volumen de la música
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


