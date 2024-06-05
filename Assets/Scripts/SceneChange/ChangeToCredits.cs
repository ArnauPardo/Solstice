using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeToCredits : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private AnimationClip changeSceneAnim;
    AudioManager audioManager;

    private PlayerControl pc;

    private void Start()
    {
        anim = GameObject.FindGameObjectWithTag(GameTags.transicion).GetComponent<Animator>();
        audioManager = GameObject.FindGameObjectWithTag(GameTags.audio).GetComponent<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerControl>();
        if (player != null)
        {
            audioManager.PararMusica();
            pc = player;
            anim = GameObject.FindGameObjectWithTag(GameTags.transicion).GetComponent<Animator>();
            StartCoroutine(ChangeScene());
        }
    }

    private IEnumerator ChangeScene()
    {
        pc.canMove = false;
        anim.SetTrigger(GameTags.changingScene);
        PlayerPrefs.SetString(GameTags.lastScene, SceneManager.GetActiveScene().name);
        yield return new WaitForSeconds(changeSceneAnim.length);
        SceneManager.LoadScene(GameTags.credits);
    }
}
