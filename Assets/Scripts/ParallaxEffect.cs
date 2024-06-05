using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{

    [SerializeField] private Transform cameraTransform;
    private Vector3 previousCameraPosition;
    [SerializeField] private float parallaxSpeed;

    [SerializeField] private float spriteWidth, startPos;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        //La siguiente línea no debería ser necesaría, pero estuve haciendo pruebas.
        cameraTransform.position = GameObject.FindGameObjectWithTag(GameTags.player).GetComponent<Transform>().position;
        cameraTransform.position = new Vector3(cameraTransform.position.x, cameraTransform.position.y, -10f);
        transform.position = GameObject.FindGameObjectWithTag(GameTags.player).GetComponent<Transform>().position;
        previousCameraPosition = cameraTransform.position;
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        startPos = transform.position.x;
    }

    //LateUptado se actualiza cada fotograma
    void LateUpdate()
    {
        //Variable que nos da la distancia que queremos que se muevan los sprites con respecto a la camera
        float deltaX = (cameraTransform.position.x - previousCameraPosition.x) * parallaxSpeed;
        //Aplicamos el movimiento
        transform.Translate(new Vector3(deltaX, 0, 0));
        //Le damos nuestro posicion actual a previousCameraPosition para que en el siguiente fotograma esté será el valor que se utiliza 
        previousCameraPosition = cameraTransform.position;

        //Variable que nos dice cuanto se movio la camera con respecto al sprite    (- startPos lo he añadido para intentar solucionar un problema, se puede borrar)
        //Problema: Cuando el jugador se mueve hacia el oeste, los sprites tardan más en recolocarse. Lo hacen tarde cuando el jugador ya está dentro de la zona y entonces, se ven los cambios
        float moveAmount = (cameraTransform.position.x - startPos) * (1 - parallaxSpeed);

        //Si la camera se movio más que la posición inicial del sprite + el tamaño del mismo, movemos los sprites
        if (moveAmount > startPos + spriteWidth)
        {
            transform.Translate(new Vector3(spriteWidth, 0, 0));
            startPos += spriteWidth;
        }
        //Es lo mismo, pero para la dirección contraria
        else if (moveAmount < startPos - spriteWidth)
        {
            transform.Translate(new Vector3(-spriteWidth, 0, 0));     
            startPos -= spriteWidth;
        }
    }
}
