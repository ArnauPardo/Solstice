using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Header("--AUDIO SOURCE--")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    public static AudioManager instance; //Para comprobar si existe otro audio manager entre escenas.

    [Header("--Generales--")]
    public AudioClip Background;
    public AudioClip Boss_Song;
    public AudioClip Title;
    public AudioClip Coin;
    public AudioClip Heal;
    public AudioClip Checkpoint;
    public AudioClip StageTransition;

     [Header("--Medusa--")]
    public AudioClip medusaAttack;
    public AudioClip medusaJump;
    public AudioClip medusaDamage;    

    [Header("--Enemigo_Slime--")]
    public AudioClip slimeDamage;
    public AudioClip slimeDeath;

    [Header("--Boss_Tank--")]
    public AudioClip tankAttack;
    public AudioClip tankSkill;
    public AudioClip tankDamage;
    public AudioClip tankDeath;
    public AudioClip tankInvulnerableSound;



    private void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip) {
        SFXSource.PlayOneShot(clip);
    }

    public void MusicaMenu() {
        musicSource.clip = Title;
        musicSource.Play();
    }

    public void PararMusica() {
        musicSource.Stop();
    }

    public void Nivel1() {
        musicSource.clip = Background;
        musicSource.Play();
    }

}
