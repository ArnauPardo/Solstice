using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HideCursor : MonoBehaviour
{
    void Awake()
    {
        PlayerPrefs.SetString(GameTags.lastScene, SceneManager.GetActiveScene().name);
        //Se oculta el cursor y se bloquea en medio de la pantalla
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
