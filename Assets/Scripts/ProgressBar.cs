using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image fillingImage;
    public GameObject knobObj;

    float target = 0f;
    bool moving = false;
    float moveSpd = 0.1f;

    // Update is called once per frame
    void Update()
    {
        if (moving) {
            fillingImage.fillAmount += Time.deltaTime * moveSpd;
            UpdateKnobLocalPos();
            if (fillingImage.fillAmount >= target) moving = false;
        }
    }

    public void SetTarget(float target)
    {
        this.target = target;
        moving = true;
    }

    public void ForceSetPlace(float place)
    {
        fillingImage.fillAmount = place;
        UpdateKnobLocalPos();
        moving = false;
    }

    void UpdateKnobLocalPos()
    {
        knobObj.transform.localPosition = new Vector3(1000f * fillingImage.fillAmount-500, -5f, 0f);
    }
}
