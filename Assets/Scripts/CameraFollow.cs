using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector2 minCameraPos, maxCameraPos;
    public Transform player;
    public float speed = 5f;
    public Vector3 offset;

    private Vector3 vel;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag(GameTags.player).transform;
    }


    void FixedUpdate()
    {

        if (player != null) { 
            Vector3 cameraPos = player.position + offset;
            cameraPos.z = transform.position.z;
            transform.position = Vector3.SmoothDamp(transform.position, cameraPos, ref vel, speed > 0 ? 1 / speed : 0f, 9999f, Time.fixedDeltaTime);
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minCameraPos.x, maxCameraPos.x), Mathf.Clamp(transform.position.y, minCameraPos.y, maxCameraPos.y), transform.position.z);
        }
    }
}
