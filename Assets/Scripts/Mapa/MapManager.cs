using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    [SerializeField] private GameObject _miniMapa;
    [SerializeField] private GameObject _MapaGrande;
    [SerializeField] private Camera _MapaCamara;

    public bool MapaGrandeAbierto { get; private set; }

    private void Awake ()
    {
        if(instance ==null)
        {
            instance = this;
        }
        StartCloseLargeMap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!MapaGrandeAbierto)
            {
                OpenLargeMap();
            }
            else
            {
                CloseLargeMap();
            }
        }
    }

    private void OpenLargeMap()
    {
        _miniMapa.SetActive(false);
        _MapaGrande.SetActive(true);
        MapaGrandeAbierto = true;
        Vector3 cameraPosition = _MapaCamara.transform.position;
        cameraPosition.z -= 500f;
        _MapaCamara.transform.position = cameraPosition;
    }
    private void StartCloseLargeMap()
    {
        _miniMapa.SetActive(true);
        _MapaGrande.SetActive(false);
        MapaGrandeAbierto = false;
    }

    private void CloseLargeMap()
    {
        _miniMapa.SetActive(true);
        _MapaGrande.SetActive(false);
        MapaGrandeAbierto = false;
        Vector3 cameraPosition = _MapaCamara.transform.position;
        cameraPosition.z += 500f;
        _MapaCamara.transform.position = cameraPosition;
    }
}
