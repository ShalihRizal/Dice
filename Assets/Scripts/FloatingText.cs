using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float floatSpeed = 1f;
    public float fadeDuration = 1f;

    private float timer = 0f;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show(int value)
    {
        text.text = value.ToString();
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        if (canvasGroup != null)
            canvasGroup.alpha = 1f - (timer / fadeDuration);

        if (timer >= fadeDuration)
            Destroy(gameObject);
    }
}
