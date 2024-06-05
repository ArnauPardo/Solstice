using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MensajeDeGuardado : MonoBehaviour
{
    [SerializeField] private GameObject message;
    [SerializeField] private GameObject light;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        light.SetActive(false);
        spriteRenderer = message.GetComponent<SpriteRenderer>();
        Color c = spriteRenderer.material.color;
        c.a = 0f;
        spriteRenderer.material.color = c;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(GameTags.player))
        {
            light.SetActive(true);
            StartCoroutine(FadeIn());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(GameTags.player))
        {
            StartCoroutine(LightOff());
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeIn()
    {
        for (float f = 0; f <= 1f; f += 0.02f)
        {
            Color c = spriteRenderer.material.color;
            c.a = f;
            spriteRenderer.material.color = c;
            yield return (0.05f);
        }
    }

    private IEnumerator FadeOut()
    {
        for (float f = 1; f >= 0f; f -= 0.02f)
        {
            Color c = spriteRenderer.material.color;
            c.a = f;
            spriteRenderer.material.color = c;
            yield return (0.05f);
        }
    }

    private IEnumerator LightOff()
    {
        yield return new WaitForSeconds(0.5f);
        light.SetActive(false);
    }
}
