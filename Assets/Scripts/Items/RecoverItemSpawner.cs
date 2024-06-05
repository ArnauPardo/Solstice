using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverItemSpawner : MonoBehaviour
{

    [Header("Fuerzas al Dropearse")]
    [SerializeField] private float initialForce = 2f;  // Fuerza inicial del rebote
    [SerializeField] private float lifeTime = 10f;     // Tiempo de vida antes de destruirse

    [SerializeField] private GameObject particulasExplosion;
    protected GameObject particulasClon;

    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider2D;
    private CombatCC combatCC;
    private PlayerHealth playerHealth;
    AudioManager audioManager;

    void Start()
    {
        playerHealth = PlayerHealth.Instance;
        combatCC = CombatCC.Instance;
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();

        // Aplicar una fuerza inicial para el rebote
        rb.AddForce(new Vector2(Random.Range(-1f, 1f), 1f) * initialForce, ForceMode2D.Impulse);
        audioManager = GameObject.FindGameObjectWithTag(GameTags.audio).GetComponent<AudioManager>();

        // Destruir la moneda despu�s de un tiempo para evitar que se acumulen en la escena
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (PlayerHealth.health >= playerHealth.maxHealth)
        {
            rb.gravityScale = 0f;
            capsuleCollider2D.isTrigger = true;
        }
        else
        {
            rb.gravityScale = 1f;
            capsuleCollider2D.isTrigger = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(GameTags.player) && PlayerHealth.health < playerHealth.maxHealth)
        {
            particulasClon = (GameObject)Instantiate(particulasExplosion, collision.gameObject.transform.position, Quaternion.identity);
            audioManager.PlaySFX(audioManager.Heal);
            combatCC.RecoverLife(gameObject);
        }
    }
}
