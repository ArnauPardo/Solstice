using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAttackEffect : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 0.15f);
    }

}
