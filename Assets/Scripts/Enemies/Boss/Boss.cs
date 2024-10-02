using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{

    private Animator anim;
    public Rigidbody2D rb;
    private Transform player;
    [SerializeField] private bool lookingRight = false;

    [SerializeField] private int bossHealth = 28;

    [SerializeField] private AnimationClip takeDamageClip; //Para hacer el boss invulnerable mientras realiza la animaci�n de recibir da�o


    //Variables momento débil del jefe
    private bool isWeak;
    private Coroutine weaknessCoroutine;
    [SerializeField] private float weaknessTime;

    [Header("Ataque Basico")]
    [SerializeField] private int contactDamage;
    [SerializeField] private Transform radioAttack;
    [SerializeField] private float radio;
    [SerializeField] private int damageAttack;

    [Header("Habilidad Especial")]
    [SerializeField] private float habilityTime;
    private float currentHabilityTime;
    public bool isInvulnerable;
    [SerializeField] private bool usingHability = false;
    [SerializeField] private float timeBeetwenHabilityCast = 6.5f;
    [SerializeField] private float actualTimeCast;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject masaLuminosa;

    [Header("Soltar objetos al morir")]
    public GameObject[] items;
    private int minCoins = 6;
    private int maxCoins = 13;
    private int minBigCoins = 3;
    private int maxBigCoins = 5;


    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag(GameTags.player).GetComponent<Transform>();
        isInvulnerable = true;
    }

    private void Update()
    {
        float distanciaJugador = Vector2.Distance(transform.position, player.position);
        anim.SetFloat(GameTags.distanciaJugador, distanciaJugador);

        anim.SetBool("isWeak", isWeak);

        if (anim.GetBool(GameTags.isEnraged))
        {
            if (!usingHability)
            {
                anim.SetFloat(GameTags.habilityCast, actualTimeCast);
                actualTimeCast -= Time.deltaTime;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        isWeak = false;
        //Detenemos la corutina Weakness si está en ejecución
        if (weaknessCoroutine != null)
        {
            StopCoroutine(weaknessCoroutine);
            weaknessCoroutine = null; //Limpiamos la referencia
        }

        if (!isInvulnerable)
        {
            bossHealth -= damage;
            if (bossHealth != 15 && bossHealth > 0)
            {
                StartCoroutine(TakeDamageCoroutine());
            }
            //Animaci�n recibir da�o
        }

        if (bossHealth <= 15)
        {
            StartCoroutine(Enrage());
        }

        if (bossHealth <= 0)
        {
            //Animaci�n de morir
            Dead();
        }
    }

    private IEnumerator TakeDamageCoroutine()
    {
        isInvulnerable = true;
        anim.SetTrigger(GameTags.takeDamage);
        yield return new WaitForSeconds(takeDamageClip.length);
        isInvulnerable = false;
    }

    private void Dead()
    {
        CoinGenerator();
        //Animaci�n de morir antes de destruir el objeto
        Destroy(gameObject);
    }

    private void CoinGenerator()
    {
        int coins = Random.Range(minCoins, maxCoins + 1);

        for (int i = 0; i < coins; i++)
        {
            Instantiate(items[0], transform.position, transform.rotation);
        }

        int bigCoins = Random.Range(minBigCoins, maxBigCoins + 1);

        for (int i = 0; i < bigCoins; i++)
        {
            Instantiate(items[2], transform.position, transform.rotation);
        }

        int recoveryItemDrop = PlayerPrefs.GetInt(GameTags.maxHearths) - PlayerHealth.health;
        Debug.Log(recoveryItemDrop);

        if (PlayerHealth.health < 5)
        {
            for (int i = 0; i < recoveryItemDrop; i++)
            {
                Instantiate(items[1], transform.position, transform.rotation);
            }
        }
    }

    public void LookAtPlayer()
    {
        if ((player.position.x >= transform.position.x && !lookingRight) || (player.position.x < transform.position.x && lookingRight))
        {
            lookingRight = !lookingRight;
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
        }        
    }

    //Attack se usa desde la animación
    private void Attack()  //Podemos decidir si el jefe es invulnerable cuando realiza el ataque basico, dependera de como sea la animacion.
    {
        StartCoroutine(TimeBeforeAttack());
        //Iniciamos la corutina Weakness solo si no está en ejecución
        if (weaknessCoroutine == null)
        {
            weaknessCoroutine = StartCoroutine(Weakness());
        }
    }

    private IEnumerator TimeBeforeAttack()
    {
        yield return new WaitForSeconds(0.2f);

        //Detecto los colliders alrededor del ataque
        Collider2D[] objects = Physics2D.OverlapCircleAll(radioAttack.position, radio);

        //Si detecta un enemigo, le har� da�o
        foreach (Collider2D col in objects)
        {

            //Cuando tengamos enemigos, est� ser� la forma de hacer el da�o
            if (col.CompareTag(GameTags.player))
            {
                if (player.position.x > transform.position.x)
                {
                    col.transform.GetComponent<CombatCC>().TakeDamage(damageAttack, -radioAttack.position);
                    
                }
                else if (player.position.x < transform.position.x)
                {
                    col.transform.GetComponent<CombatCC>().TakeDamage(damageAttack, radioAttack.position);
                }
            }
        }
    }

    private IEnumerator Weakness()
    {
        isWeak = true;
        yield return new WaitForSeconds(weaknessTime);
        isWeak = false;

        weaknessCoroutine = null; //Limpiamos la referencia cuando la corutina termina
    }

    public void Hability() 
    {
        usingHability = true;
        isInvulnerable = true;
        actualTimeCast = timeBeetwenHabilityCast;
        //Animacion habilidad
        currentHabilityTime = habilityTime;

        //Llamamos al metodo SpawnMasaLuminosa repetidamente cada 0.5 segundos
        InvokeRepeating("SpawnMasaLuminosa", 0f, 0.5f);
    }

    // Este metodo sera llamado repetidamente por InvokeRepeating
    private void SpawnMasaLuminosa()
    {
        //Instanciamos objetos de masa luminosa en una posicion aleatoria
        Instantiate(masaLuminosa, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);

        //Restamos el tiempo transcurrido
        currentHabilityTime -= 0.5f;

        //Si el tiempo restante es menor o igual a 0, cancelamos InvokeRepeating
        if (currentHabilityTime <= 0)
        {
            usingHability = false;
            isInvulnerable = false;
            CancelInvoke("SpawnMasaLuminosa");
        }
    }

    private IEnumerator Enrage()
    {
        isInvulnerable = true;
        anim.SetBool(GameTags.isEnraged, true);
        yield return new WaitForSeconds(2f); //El tiempo de espera tiene que ser la duraci�n del clip de enrage
        isInvulnerable = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(GameTags.player))
        {
            //Accedemos al script de combate del jugador y le aplicamos el da�o. Le pasamos el primer punto de impacto de la colisi�n
            collision.gameObject.GetComponent<CombatCC>().TakeDamage(contactDamage, collision.GetContact(0).normal);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(radioAttack.position, radio);
    }
}
