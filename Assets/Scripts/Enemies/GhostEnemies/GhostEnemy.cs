using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GhostEnemy : MonoBehaviour
{
    [Header("ID")]
    [SerializeField] private string id;

    [Header("Estadisticas")]
    [SerializeField] private float vida;
    [SerializeField] private int damage;

    [Header("Movimiento")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private int currentWaypoint;
    [SerializeField] private int lastWaypoint;
    [SerializeField] private Transform player;
    [SerializeField] private float detectRadius;
    [SerializeField] private Collider2D[] detectedObjects;
    [SerializeField] private LayerMask detectLayer;

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
    [SerializeField] private float invChanceTimer; //Segundos que hay que esperar antes de volver a intentar si se puede hacer invisible
    [SerializeField] private float invisibleTime; //Tiempo que está invisible
    [SerializeField] private bool canDisappear = true;

    //Control de movimiento con el pathfinding
    private AIDestinationSetter setter;
    private AIPath path;
    [SerializeField] private bool canMove;

    //Animaciones
    private Animator anim;
    AudioManager audioManager;
    private SpriteRenderer sr;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioManager = GameObject.FindGameObjectWithTag(GameTags.audio).GetComponent<AudioManager>();
        setter = GetComponent<AIDestinationSetter>();
        path = GetComponent<AIPath>();
        canMove = true;

        //Iniciamos el primer punto al que van los enemigos
        currentWaypoint = Random.Range(0, waypoints.Length);
        setter.target = waypoints[currentWaypoint];

        //SpriteRenderer y guardar el color para que los "GhostEnemy" puedan volverse invisibles
        canDisappear = true;
        sr = GetComponent<SpriteRenderer>();

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

    private void Update()
    {
        if (canMove)
        {
            Rotation();
            Movement();
        }
    }

    private void Rotation()
    {
        if (setter.target.transform.position.x < gameObject.transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        if (setter.target.transform.position.x > gameObject.transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void Movement()
    {
        if (player == null)
        {
            if (path.remainingDistance <= 0.1f)
            {
                currentWaypoint = Random.Range(0, waypoints.Length);
                while (currentWaypoint == lastWaypoint)
                {
                    currentWaypoint = Random.Range(0, waypoints.Length);
                }
                lastWaypoint = currentWaypoint;
                setter.target = waypoints[currentWaypoint];
                path.SearchPath();
            }
        }
        else
        {
            setter.target = player.transform;
            path.maxSpeed = 3f;
        }

        //Hacemos un Overlap para detectar a todos los objetos a su alrededor aunque solo queramos al jugador
        detectedObjects = Physics2D.OverlapCircleAll(this.transform.position, detectRadius, detectLayer);

        //Cada vez que encontremos algo dentro del rango, le comprobamos el tag y si es el player, lo asignamos
        for (int i = 0; i < detectedObjects.Length; i++)
        {
            if (detectedObjects[i].tag == GameTags.player)
            {
                player = detectedObjects[i].transform;
            }
        }

        //Si no se está detectando ningun objeto, el jugador está fuera de rango y dejamos de persegirlo.
        if (detectedObjects.Length == 0 && player != null) //Comprobamos que player no sea null para mejor funcionamiento
        {
            setter.target = waypoints[lastWaypoint];
            player = null;
            path.maxSpeed = 2f;
        }
    }

    private IEnumerator DisappearRoutine()
    {
        while (true) // Repetir indefinidamente
        {
            if (canDisappear && canMove) // Solo intentar desaparecer si puede
            {
                Disappear();
            }
            yield return new WaitForSeconds(invChanceTimer); // Espera el tiempo antes de intentar de nuevo
        }
    }

    private void Disappear()
    {
        int invisibleProbabilityTime = Random.Range(1, 3);

        if (invisibleProbabilityTime == 1)
        {
            canDisappear = false;
            StartCoroutine(InvisibleEffect());
        }
    }

    private IEnumerator InvisibleEffect()
    {
        for (float f = 1; f >= 0f; f -= 0.02f)
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
        setter.target = null;
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
        setter.target = null;
        canMove = false;
        //Forzar al objeto a reaparecer (volver a su color original)
        Color originalColor = sr.material.color;
        originalColor.a = 1f; // Forzar el valor alfa a 1 para que sea completamente visible
        yield return new WaitForSeconds(takeDmgAnim.length + 0.6f);
        isInvulnerable = false;
        canMove = true;

        //Si no se está detectando ningun objeto, el jugador está fuera de rango y dejamos de persegirlo.
        if (detectedObjects.Length == 0 && player != null) //Comprobamos que player no sea null para mejor funcionamiento
        {
            setter.target = waypoints[lastWaypoint];
            player = null;
            path.maxSpeed = 2f;
        }

        if (detectedObjects.Length > 0)
        {
            setter.target = player.transform;
        }
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
            StartCoroutine(AfkTime());
        }
    }

    private IEnumerator AfkTime()
    {
        setter.target = null;
        canMove = false;
        //Forzar al objeto a reaparecer (volver a su color original)
        Color originalColor = sr.material.color;
        originalColor.a = 1f; // Forzar el valor alfa a 1 para que sea completamente visible
        sr.material.color = originalColor;
        yield return new WaitForSeconds(1.5f);
        canMove = true;

        //Si no se está detectando ningun objeto, el jugador está fuera de rango y dejamos de persegirlo.
        if (detectedObjects.Length == 0 && player != null) //Comprobamos que player no sea null para mejor funcionamiento
        {
            setter.target = waypoints[lastWaypoint];
            player = null;
            path.maxSpeed = 2f;
        }

        if (detectedObjects.Length > 0)
        {
            setter.target = player.transform;
        }
    }

    public void AfkGettingDmg()
    {
        StartCoroutine(AfkTime());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, detectRadius);
    }
    
}
