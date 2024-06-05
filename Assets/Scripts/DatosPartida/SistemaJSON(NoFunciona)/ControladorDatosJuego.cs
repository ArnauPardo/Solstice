using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class ControladorDatosJuego : MonoBehaviour
{

    public GameObject player;
    public string archivoDeGuardad;
    public DatosPartida datosPartida = new DatosPartida();

    private void Awake()
    {
        archivoDeGuardad = Application.dataPath + "/ArchivoDeGuardado/datosJuego.json";
        player = GameObject.FindGameObjectWithTag(GameTags.player);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CargarDatos();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            GuardarDatos();
        }
    }

    public void CargarDatos()
    {
        if (File.Exists(archivoDeGuardad))
        {
            string contenido = File.ReadAllText(archivoDeGuardad);

            datosPartida = JsonUtility.FromJson<DatosPartida>(contenido);

            SceneManager.LoadScene(datosPartida.scene);
            player.transform.position = datosPartida.position;
            PlayerHealth.health = datosPartida.health;
            Settings.dinero = datosPartida.money;
        }
    }

    public void GuardarDatos()
    {
        DatosPartida nuevosDatos = new DatosPartida()
        {
            scene = SceneManager.GetActiveScene().name,
            position = player.transform.position,
            health = PlayerHealth.health,
            money = Settings.dinero
        };

        string cadenaJSON = JsonUtility.ToJson(nuevosDatos);

        File.WriteAllText(archivoDeGuardad, cadenaJSON);

        Debug.Log("Archivo Guardado");
    }
}
