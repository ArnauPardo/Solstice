using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CombatCC : MonoBehaviour
{

    public static CombatCC Instance;

    [Header("Ataque")]
    [SerializeField] private Transform radioAttack;

    [SerializeField] private float radio;

    [SerializeField] private int damage;

    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float timeNextAttack;

    [SerializeField] private GameObject attackEffect;
    protected GameObject attackEffectClon;

    private float recoilForce = 5f; //Fuerza para que el jugador rebote al atacar la parte dura del jefe

    [Header("Recibir dano")]
    //[SerializeField] private GameObject[] hearts;
    [SerializeField] private Sprite emptyLife;
    [SerializeField] private Sprite fullLife;
    [SerializeField] private AnimationClip medusa_dmg_Animation;

    [Header("Muerte")]
    [SerializeField] AnimationClip deathScene;
    [SerializeField] private float timeBeforeSceneChange = 2.5f; //Tenia 2.5f

    public Vector3 lastSafePosition;

    [SerializeField] private GameObject textoPartidaGuardada;

    //Detecto el otro script para saber si el jugador est� en el aire
    private PlayerControl pc;

    //Hago otra referencia distinta a PlayerControl que lo usare para poder guardar partida correctamente
    [SerializeField] private PlayerControl pcForSaveGame;

    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private PlayerHealth playerHealth;
    AudioManager audioManager;
    [SerializeField] private GameObject particulasAtacar;
    protected GameObject particulasClon;

    private void Start()
    {        
        Instance = this; //Inicializamos instancia para poder acceder al código desde otros scrpits
        AddComponents();
        HideSaveText();

        Physics2D.IgnoreLayerCollision(3, 11, false); //Devolvemos la colisiones entre jugador y enemigos. Si no lo hacemos, se buggea muchas veces
        lastSafePosition = transform.position; //Guardamos posicion a la que vuelve el jugador al caer al vacio
    }

    private void AddComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        pc = GetComponent<PlayerControl>();
        playerHealth = GetComponent<PlayerHealth>();
        audioManager = GameObject.FindGameObjectWithTag(GameTags.audio).GetComponent<AudioManager>();
    }

    private void HideSaveText()
    {
        textoPartidaGuardada = GameObject.FindGameObjectWithTag(GameTags.textoPartidaGuardada);
        if (textoPartidaGuardada != null)
        {
            textoPartidaGuardada.SetActive(false);
        }
    }

    private void Update()
    {
        if (!Settings.isDead)
        {
            Attack();
        }

        if (PlayerHealth.health <= 0)
        {
            StartCoroutine(GameOver());
        }

        //Guardar partida
        if (pcForSaveGame != null && Input.GetKeyDown(KeyCode.X))
        {
            SaveData();
            StartCoroutine(TextoPartidaGuardada());
        }
    }

    private void Attack()
    {
        //Genero un tiempo entre ataques
        if (timeNextAttack > 0)
        {
            timeNextAttack -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && timeNextAttack <= 0)
        {
            audioManager.PlaySFX(audioManager.medusaAttack);
            timeNextAttack = timeBetweenAttacks;
            HitAnim(pc.isGrounded);
        }
    }

    public void HitAnim(bool jump)
    {
        //Dependiendo si est� en el aire o no, hace una animaci�n u otra
        if (jump)
        {
            anim.SetTrigger(GameTags.playerAttackAnim);
        }
        else if (!jump)
        {
            anim.SetTrigger(GameTags.playerJumpAttackAnim);
        }
    }

    private IEnumerator GameOver()
    {
        capsuleCollider.isTrigger = true;
        rb.velocity = Vector3.zero;
        rb.gravityScale = 0.0f;
        Settings.isDead = true;
        anim.SetTrigger(GameTags.playerDeath);
        yield return new WaitForSeconds(deathScene.length);
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(timeBeforeSceneChange);
        SceneManager.LoadScene(GameTags.gameOverScene);
    }

    private void SaveData()
    {
        //Recuperamos todos los puntos de vida al guardar partida antes de guardar la vida que tenemos
        PlayerHealth.health = playerHealth.maxHealth;
        playerHealth.UpdateHeartsUI();

        DataGameManager.Instance.CurrentScene(SceneManager.GetActiveScene().name);
        DataGameManager.Instance.CurrentPosition(transform.position.x, transform.position.y, transform.position.z);
        DataGameManager.Instance.CurrentHearths(PlayerHealth.health);//Guardamos la vida despues de actualizarla, siempre deberiamos tener 5 de vida al continuar partida
        DataGameManager.Instance.CurrentMoney(Settings.dinero);
        MonedasRecogidas.Instance.SaveData(); //Guardamos los ids de las monedas ya recogidas
        CurasRecogidas.Instance.ClearData(); //Borramos las curaciones de la lista para que vuelvan a aparecer
        EnemiesKilled.Instance.ClearData(); //Borramos los enemigos de la lista para que vuelvan a aparecer
    }

    private IEnumerator TextoPartidaGuardada()
    {
        textoPartidaGuardada.SetActive(true);
        yield return new WaitForSeconds(2f);
        textoPartidaGuardada.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Detecto el script PlayerControl para poder guardar
        if (collision.CompareTag(GameTags.checkpoint))
        {
            pcForSaveGame = GetComponent<PlayerControl>();
        }

        //Detectamos el collider que hay al caer al vacío
        if (collision.CompareTag(GameTags.fallingIntoVoid)) //Vuelve al ultimo sitio donde ha saltado el jugador cuando cae al vacio
        {
            StartCoroutine(FallingIntoVoid());
        }
    }
    private IEnumerator FallingIntoVoid()
    {
        PlayerHealth.health--;

        yield return new WaitForSeconds(2f);

        if (PlayerHealth.health <= 0)
        {
            SceneManager.LoadScene(GameTags.gameOverScene);
        }
        else
        {
            transform.position = lastSafePosition;
            playerHealth.UpdateHeartsUI();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(GameTags.checkpoint) && pcForSaveGame != null)
        {
            pcForSaveGame = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(radioAttack.position, radio);
    }

    //Métodos que utilizo desde otros sitios (scripts o animaciones)
    private void Hit() //Lo usamos como evento en la animaci�n de atacar
    {
        //Detecto los colliders alrededor del ataque
        Collider2D[] objects = Physics2D.OverlapCircleAll(radioAttack.position, radio);

        foreach (Collider2D col in objects)
        {
            switch (col.tag)
            {
                case GameTags.simpleEnemy:
                    particulasClon = (GameObject)Instantiate(particulasAtacar, col.gameObject.transform.position, Quaternion.identity);
                    attackEffectClon = (GameObject)Instantiate(attackEffect, col.gameObject.transform.position, Quaternion.identity);
                    audioManager.PlaySFX(audioManager.medusaDamage);
                    col.transform.GetComponent<SimpleEnemy>().GetDamage(damage);
                    break;

                case GameTags.ghostEnemy:
                    particulasClon = (GameObject)Instantiate(particulasAtacar, col.gameObject.transform.position, Quaternion.identity);
                    attackEffectClon = (GameObject)Instantiate(attackEffect, col.gameObject.transform.position, Quaternion.identity);
                    audioManager.PlaySFX(audioManager.medusaDamage);
                    col.transform.GetComponent<GhostEnemy>().GetDamage(damage);
                    break;

                case GameTags.boss:                    
                    audioManager.PlaySFX(audioManager.tankInvulnerableSound);
                    pc.ApplyRecoil(recoilForce);
                    break;

                case GameTags.weaknessPoint:
                    particulasClon = (GameObject)Instantiate(particulasAtacar, col.gameObject.transform.position, Quaternion.identity);
                    attackEffectClon = (GameObject)Instantiate(attackEffect, col.gameObject.transform.position, Quaternion.identity);
                    audioManager.PlaySFX(audioManager.medusaDamage);
                    col.transform.GetComponentInParent<Boss>().TakeDamage(damage);
                    break;

                default:
                    // Si necesitas manejar otros casos o no hacer nada, puedes hacerlo aquí.
                    break;
            }
        }
    }

    public void TakeDamage(int damage, Vector2 position)
    {
        PlayerHealth.health -= damage;
        playerHealth.UpdateHeartsUI();
        anim.SetTrigger(GameTags.playerTakeDamageAnim);
        StartCoroutine(LostControl()); //Tiempo durante el que el jugador no puede hacer nada
        StartCoroutine(DisableCollisions()); //Ignoramos colisiones al recibir da�o durante el tiempo que queramos
        pc.Rebound(position); //Genera un rebote hacia al lado opuesto de lo que chocamos
    }

    private IEnumerator LostControl()
    {
        pc.canMove = false;
        yield return new WaitForSeconds(medusa_dmg_Animation.length);
        pc.canMove = true;
    }

    private IEnumerator DisableCollisions()
    {
        // Cuando vamos al inspector de un objeto, podemos seleccionar en que Layer est�
        // Hacemos que no haya colision entre dos Layers durante el tiempo que queramos. 3 y 11 hacen referencia a esos Layers
        // El valor "True", es que SI estamos ignorando la colisi�n, "False" NO la ignoramos
        Physics2D.IgnoreLayerCollision(3, 11, true);
        yield return new WaitForSeconds(medusa_dmg_Animation.length + 0.5f); //Testear y decidir si dar un poco m�s de tiempo de inmunidad
        Physics2D.IgnoreLayerCollision(3, 11, false);
    }

}
