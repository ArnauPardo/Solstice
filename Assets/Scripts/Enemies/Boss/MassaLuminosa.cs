using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassaLuminosa : MonoBehaviour
{

    [SerializeField] private int damageMassa = 1;

    [SerializeField] private Transform player;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag(GameTags.player).GetComponent<Transform>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameTags.player))
        {
            if (player.position.x < transform.position.x)
            {
                //Animación destruir la massa
                //Se pueden añadir particulas al destruir
                collision.gameObject.GetComponent<CombatCC>().TakeDamage(damageMassa, transform.position);
                Destroy(this.gameObject);
            }
            else if (player.position.x >= transform.position.x)
            {
                //Animación destruir la massa
                //Se pueden añadir particulas al destruir
                collision.gameObject.GetComponent<CombatCC>().TakeDamage(damageMassa, -transform.position);
                Destroy(this.gameObject);
            }
        }

        if (collision.CompareTag(GameTags.ground))
        {
            Destroy(this.gameObject);
            //Animación destruir la massa
            //Se pueden añadir particulas al destruir
        }


    }
}
