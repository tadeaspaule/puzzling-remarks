using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionScreen : MonoBehaviour
{
    public Image image;
    float fadeSpeed = 0.5f;

    // Update is called once per frame
    void Update()
    {
        Color c = image.color;
        image.color = new Color(c.r,c.g,c.b,Mathf.Max(0f,c.a - Time.deltaTime * fadeSpeed));
        if (image.color.a <= 0f) gameObject.SetActive(false);
    }
}
