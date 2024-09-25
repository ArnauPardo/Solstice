using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{

    [Header("ID")]
    [SerializeField] private string id;

    [Header("Estadisticas")]
    [SerializeField] private float vida;
    [SerializeField] private int damage;

    [Header("Movimiento")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private bool lookingRight = true;
    [SerializeField] private bool isMoving = true;

    [SerializeField] private float idleTime = 1f;
    [SerializeField] private float timer;

    public Transform rightPoint;
    public Transform leftPoint;

    [Header("Recibir Dano")]
    [SerializeField] private bool isInvulnerable = false;
    [SerializeField] private AnimationClip takeDmgAnim;

    [Header("Muerte")]
    public GameObject objetoPadre;
    public AnimationClip muerte;  //Para saber la duraci�n del clip
    private CapsuleCollider2D capsuleCollider;
    private bool isDead = false;

    [Header("Soltar objetos al morir")]
    public GameObject[] items;
    private int minCoins = 0;
    private int maxCoins = 2;

    [Header("Efecto invisiblidad")]
    [SerializeField] private int valorAleatorio; //El pj no siempre se hace invisible, tiene un 50% de probabilidades cada X segundos. Esta variable guarda el valor
    [SerializeField] private float invChanceTimer; //Segundos que hay que esperar antes de volver a intentar si se puede hacer invisible
    [SerializeField] private float invisibleTime; //Tiempo que está invisible
    [SerializeField] private float colorAlfa;
    [SerializeField] private bool canDisappear = true;

    //Animaciones
    private Animator anim;
    AudioManager audioManager;
    private SpriteRenderer sr;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioManager = GameObject.FindGameObjectWithTag(GameTags.audio).GetComponent<AudioManager>();

        //SpriteRenderer y guardar el color para que los "GhostEnemy" puedan volverse invisibles
        canDisappear = true;
        sr = GetComponent<SpriteRenderer>();
        Color c = sr.material.color;
        colorAlfa = c.a;

        //Condicional para que no hagan respawn los enemigos derrotados
        if (EnemiesKilled.Instance.enemies.Contains(id))
        {
            Destroy(gameObject);
        }

        if (gameObject.CompareTag("GhostEnemy"))
        {
            StartCoroutine(DisappearRoutine());
        }
    }

    void Update()
    {
        //Para que se quede quito al morir
        if (!isDead && !isInvulnerable)
        {
            Movement();            
        }
    }
    
    private void Movement()
    {
        if (lookingRight && isMoving)
        {
            anim.SetBool(GameTags.seMovement, true);
            //Movemos al enemigo hasta la primera posici�n
            transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.position = Vector2.MoveTowards(transform.position, rightPoint.position, speed * Time.deltaTime);
            //Calculamos que ha llegado al destino y lo giramos
            if (Vector2.Distance(transform.position, rightPoint.position) <= 0.1f)
            {
                lookingRight = false;
                isMoving = false;
                anim.SetBool(GameTags.seMovement, false);
                timer = idleTime;
            }
        }
        else if (!lookingRight && isMoving)
        {
            anim.SetBool(GameTags.seMovement, true);
            //Movemos al enemigo hasta la segunda posici�n
            transform.rotation = Quaternion.Euler(0, 180, 0);
            transform.position = Vector2.MoveTowards(transform.position, leftPoint.position, speed * Time.deltaTime);
            //Calculamos que ha llegado al destino y lo giramos
            if (Vector2.Distance(transform.position, leftPoint.position) <= 0.1f)
            {
                lookingRight = true;
                isMoving = false;
                anim.SetBool(GameTags.seMovement, false);
                timer = idleTime;
            }
        }

        //Tiempo que est� en enemigo en idle
        if (!isMoving)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                isMoving = true;
            }
        }
    }

    private IEnumerator DisappearRoutine()
    {
        while (true) // Repetir indefinidamente
        {
            if (canDisappear) // Solo intentar desaparecer si puede
            {
                Disappear();
            }
            yield return new WaitForSeconds(invChanceTimer); // Espera el tiempo antes de intentar de nuevo
        }
    }

    private void Disappear()
    {
        int invisibleProbabilityTime = Random.Range(1, 3);
        valorAleatorio = invisibleProbabilityTime;

        if (invisibleProbabilityTime == 1)
        {
            canDisappear = false;
            StartCoroutine(InvisibleEffect());
        }
    }

    private IEnumerator InvisibleEffect()
    {
        for (float f = colorAlfa; f >= 0f; f -= 0.02f)
        {
            Color a = sr.material.color;
            a.a = f;
            sr.material.color = a;
            yield return (0.05f);
        }

        yield return new WaitForSeconds(invisibleTime);

        for (float f = 0; f <= 1f; f += 0.02f)
        {
            Color e = sr.material.color;
            e.a = f;
            sr.material.color = e;
            yield return (0.05f);
        }

        canDisappear = true;
    }

    public void GetDamage(int damage)
    {
        if (!isInvulnerable)
        {
            vida -= damage;

            if (vida <= 0)
            {                               
                StartCoroutine(Dead());
            }

            //Esta condici�n tiene que ir despu�s de if (vida <= 0), sino la animaci�n de muerte no se realiza
            if (!isDead)
            {                
                StartCoroutine(TakeDmgAnim());
            }
        }
    }

    private IEnumerator Dead()
    {
        audioManager.PlaySFX(audioManager.slimeDeath);
        capsuleCollider.isTrigger = true;
        isDead = true;
        anim.SetTrigger(GameTags.seDeath);
        EnemiesKilled.Instance.enemies.Add(id);
        yield return new WaitForSeconds(muerte.length); //muerte.length es la duraci�n de la animaci�n de morir
        CoinGenerator();
        Destroy(objetoPadre); //Destruyo el objeto "objetoPadre" que contiene al enemigo. Es para optimizaci�n del proyecto
    }

    private IEnumerator TakeDmgAnim() //Invulnerabilidad del enemigo cuando ha recibido un golpe y est� realizando la animacion
    {
        audioManager.PlaySFX(audioManager.slimeDamage);
        anim.SetTrigger(GameTags.seTakeDmg);
        isInvulnerable = true;
        yield return new WaitForSeconds(takeDmgAnim.length + 0.2f);
        isInvulnerable = false;
    }

    private void CoinGenerator()
    {
        int coins = Random.Range(minCoins, maxCoins + 1);

        for (int i = 0; i < coins; i++)
        {
            Instantiate(items[0], transform.position, transform.rotation);
        }

        int dropProbability = Random.Range(1, 11);

        if (PlayerHealth.health < 5 && dropProbability <= 3) //30% de probabilidad que drope una vida
        {
            Instantiate(items[1], transform.position, transform.rotation);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(GameTags.player))
        {
            //Accedemos al script de combate del jugador y le aplicamos el da�o. Le pasamos el primer punto de impacto de la colisi�n
            collision.gameObject.GetComponent<CombatCC>().TakeDamage(damage, collision.GetContact(0).normal);
        }
    }
}
