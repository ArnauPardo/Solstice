using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelChange_Intro : MonoBehaviour
{

    public float changeTime;
    public string sceneName;
    AudioManager audioManager;

    public void Awake() {
        audioManager = GameObject.FindGameObjectWithTag(GameTags.audio).GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        changeTime -= Time.deltaTime;
        if(changeTime <= 0) {
            SceneManager.LoadScene(sceneName);
            audioManager.Nivel1();
        }
    }
}
