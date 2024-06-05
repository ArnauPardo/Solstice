using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    [SerializeField] private GameObject[] door;
    [SerializeField] private Transform[] doorClosedPoint;
    [SerializeField] private GameObject boss;

    private Animator anim;

    private void Start()
    {
        boss = GameObject.FindGameObjectWithTag(GameTags.boss);
        anim = boss.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag(GameTags.player))
        {
            anim.SetTrigger(GameTags.isActive);
            door[0].transform.position = Vector3.MoveTowards(door[0].transform.position, doorClosedPoint[0].position, 2f);
            door[1].transform.position = Vector3.MoveTowards(door[1].transform.position, doorClosedPoint[1].position, 2f);
            Destroy(gameObject);
        }
        
    }
}
