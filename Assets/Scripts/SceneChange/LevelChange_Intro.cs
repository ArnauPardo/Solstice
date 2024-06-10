using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor.VersionControl;

public class LevelChange_Intro : MonoBehaviour
{

    public float changeTime;
    public string sceneName;
    private AudioManager audioManager;

    //Intro Message
    [SerializeField] private Animator anim;
    private float timer = 4f;
    private float timeCount;

    public void Awake() {
        audioManager = GameObject.FindGameObjectWithTag(GameTags.audio).GetComponent<AudioManager>(); 

        StartCoroutine(SkipIntro());

        timeCount = timer;
    }

    void Update()
    {
        //Cuando el jugador pulsa cualquier tecla, le sale el aviso de omitir. Ponemos un contador para que las animación funcionen bien, sino se repite el FadeOut 2 veces reiniciando la imagen cuando no debe
        if (Input.anyKeyDown && timeCount <= 0)
        {
            StartCoroutine(SkipIntro());
            timeCount = timer;
        }

        if (timeCount > 0)
        {
            timeCount -= Time.deltaTime;
        }

        //Pulsa ESC, cambia de escena
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(sceneName);
            audioManager.Nivel1();
        }

        //Al acabar el tiempo, cambia de escena
        changeTime -= Time.deltaTime;
        if(changeTime <= 0) {
            SceneManager.LoadScene(sceneName);
            audioManager.Nivel1();
        }
    }

    private IEnumerator SkipIntro()
    {
        yield return new WaitForSeconds(1f);
        //FadeIn
        anim.SetTrigger("FadeIn");
        yield return new WaitForSeconds(3f);
        //FadeOut
        anim.SetTrigger("FadeOut");
    }
}
