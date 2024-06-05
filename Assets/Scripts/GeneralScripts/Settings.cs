using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Settings : MonoBehaviour
{
    public static bool gameContinue;
    public static bool newGame;

    public static bool isDead = false;

    public static int dinero;

    //Referenciamos los objetos que muestran el dinero y la vida para poder actualizarlo al cambiar de escena
    private GameObject puntuacion;
    private PlayerHealth playerHealth;

    //Referencio los objetos de la calidad gráfica
    public TMP_Dropdown resolucionDropdown;
    public TMP_Dropdown calidadDropdown;
    public Toggle pantallaCompletaToggle;

    //Referencias de los objetos del sonido
    public AudioSource musicaSource;
    public AudioSource efectosSource;

    //Instancia del script CombatCC para poder actualizar la variable que guarda la posicion del jugador al cambiar de escena para cuando cae al vacio, colocar el jugador a esa posicion
    private CombatCC combatCC;
    //Variables para colocar al jugador en la posicion correcta, cuando el jugador continua una partida
    private GameObject player;
    private Vector3 playerPos;


    private void Awake()
    {
        //Cuando el jugador continua una partida anterior
        if (SceneManager.GetActiveScene().name != GameTags.menu && gameContinue)
        {
            player = GameObject.FindGameObjectWithTag(GameTags.player);
            playerHealth = player.GetComponent<PlayerHealth>();
            playerPos.x = PlayerPrefs.GetFloat(GameTags.playerXPosition);
            playerPos.y = PlayerPrefs.GetFloat(GameTags.playerYPosition);
            playerPos.z = PlayerPrefs.GetFloat(GameTags.playerZPosition);
        }        

        if (PlayerPrefs.GetString(GameTags.lastScene) == GameTags.menu && gameContinue)
        {
            player.transform.position = playerPos;            
            puntuacion = GameObject.FindGameObjectWithTag(GameTags.puntuacionText);
            RefreshSavedData(PlayerPrefs.GetInt(GameTags.savedMoney));
        }

        //Cuando el jugador empieza una partida nueva
        if (SceneManager.GetActiveScene().name != GameTags.menu && newGame)
        {
            player = GameObject.FindGameObjectWithTag(GameTags.player);
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        if (PlayerPrefs.GetString(GameTags.lastScene) == GameTags.menu && newGame)
        {
            player.transform.position = new Vector3(-2.1f, -2.4f, 0f);
            player.transform.rotation = Quaternion.Euler(0, 180, 0);
            puntuacion = GameObject.FindGameObjectWithTag(GameTags.puntuacionText);
            RefreshSavedData(PlayerPrefs.GetInt(GameTags.savedMoney));
        }

        //Cargar valores al cambiar de escena
        if (PlayerPrefs.GetString(GameTags.lastScene) != GameTags.menu)
        {            
            LoadCurrentData();
        }
        
        ApplySettings();
        
    }

    private void Start()
    {
        if (PlayerPrefs.GetString(GameTags.lastScene) != GameTags.menu)
        {
            puntuacion = GameObject.FindGameObjectWithTag(GameTags.puntuacionText);
            if (puntuacion != null)
            {
                RefreshCurrentData(PlayerPrefs.GetInt(GameTags.currentMoney));
            }
        }
    }

    private void OnDestroy() //Al cambiar de escena se destruyen todos los objetos de la escena y por tanto, es activa este método
    {
        SaveCurrentData();
    }

    public void SaveCurrentData() //Guardamos valores para el cambio de escenas
    {
        if (resolucionDropdown != null && !PlayerPrefs.HasKey(GameTags.resolucionIndex))
        {
            PlayerPrefs.SetInt(GameTags.resolucionIndex, resolucionDropdown.value);
        }

        if (calidadDropdown != null && !PlayerPrefs.HasKey(GameTags.calidadIndex))
        {
            PlayerPrefs.SetInt(GameTags.calidadIndex, calidadDropdown.value);
        }

        if (pantallaCompletaToggle != null && !PlayerPrefs.HasKey(GameTags.pantallaCompleta))
        {
            PlayerPrefs.SetInt(GameTags.pantallaCompleta, pantallaCompletaToggle.isOn ? 1 : 0);
        }

        PlayerPrefs.SetInt(GameTags.currentMoney, dinero);
        PlayerPrefs.SetInt(GameTags.currentHealth, PlayerHealth.health);
        PlayerPrefs.Save();
    }

    public void LoadCurrentData() //Cargamos valores para el cambio de escenas
    {
        PlayerPrefs.GetInt(GameTags.currentMoney);
        PlayerPrefs.GetInt(GameTags.currentHealth);
        if (resolucionDropdown != null && PlayerPrefs.HasKey(GameTags.resolucionIndex))
        {
            resolucionDropdown.value = PlayerPrefs.GetInt(GameTags.resolucionIndex);
        }

        if (calidadDropdown != null && PlayerPrefs.HasKey(GameTags.calidadIndex))
        {
            calidadDropdown.value = PlayerPrefs.GetInt(GameTags.calidadIndex);
        }

        if (pantallaCompletaToggle != null && PlayerPrefs.HasKey(GameTags.pantallaCompleta))
        {
            pantallaCompletaToggle.isOn = PlayerPrefs.GetInt(GameTags.pantallaCompleta) == 1;
        }
    }

    public void RefreshCurrentData(int money)
    {
        if (puntuacion != null)
        {
            puntuacion.GetComponent<Text>().text = money.ToString();
        }
        PlayerHealth.health = PlayerPrefs.GetInt(GameTags.currentHealth);
        playerHealth.UpdateHeartsUI();
    }

    public void RefreshSavedData(int money)
    {
        if (puntuacion != null)
        {
            puntuacion.GetComponent<Text>().text = money.ToString();
        }
        PlayerHealth.health = PlayerPrefs.GetInt(GameTags.savedHealth);
        playerHealth.UpdateHeartsUI();
    }

    public void ApplySettings()
    {
        if (pantallaCompletaToggle != null)
        {
            Screen.fullScreen = pantallaCompletaToggle.isOn;
        }

        if (calidadDropdown != null)
        {
            QualitySettings.SetQualityLevel(calidadDropdown.value);
        }
    }
    public void ActualizarVolumenMusica(float volumen)
    {
        if (musicaSource != null)
        {
            musicaSource.volume = volumen;
        }
    }

    // Metodo para actualizar el volumen de los efectos de sonido
    public void ActualizarVolumenEfectos(float volumen)
    {
        if (efectosSource != null)
        {
            efectosSource.volume = volumen;
        }
    }
}
