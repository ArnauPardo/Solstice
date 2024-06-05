using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    [Header("Cambio de escena")]
    [SerializeField] private LevelConnection connection;

    [SerializeField] private string targetSceneName;

    [SerializeField] private Transform spawnPoint;

    [Header("Transición")]
    [SerializeField] private Animator anim;
    [SerializeField] private AnimationClip changeSceneAnim;
    private PlayerControl pc;


    private void Awake()
    {        
        if (connection == LevelConnection.ActiveConnection)
        {
            pc = FindObjectOfType<PlayerControl>();
            pc.transform.position = spawnPoint.position;
            pc.transform.rotation = spawnPoint.rotation;
            FindObjectOfType<ParallaxEffect>().transform.position = pc.transform.position;
            pc.canMove = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.GetComponent<PlayerControl>();
        if (player != null )
        {            
            pc = player;
            anim = GameObject.FindGameObjectWithTag(GameTags.transicion).GetComponent<Animator>();
            LevelConnection.ActiveConnection = connection;
            StartCoroutine(ChangeScene());
        }      
    }

    private IEnumerator ChangeScene()
    {
        //Se puede acceder al RigidBody del player y poner su velocidad en 0. También acceder al animator de Player y ponerlo en idle, pero creo que queda mejor así
        pc.canMove = false;
        anim.SetTrigger(GameTags.changingScene);
        PlayerPrefs.SetString(GameTags.lastScene, SceneManager.GetActiveScene().name);
        yield return new WaitForSeconds(changeSceneAnim.length);
        SceneManager.LoadScene(targetSceneName);
    }

}
