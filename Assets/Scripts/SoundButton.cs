using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour
{
    Image image;
    public Sprite soundOn;
    public Sprite soundOff;
    public Slider slider;
    SoundManager soundManager;
    float lastMutedVol;
    
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        soundManager = FindObjectOfType<SoundManager>();
        UpdateSound(soundManager.GetVolume());
        slider.value = soundManager.GetVolume();
    }

    void UpdateSound(float val)
    {
        if (val == 0) image.sprite = soundOff;
        else image.sprite = soundOn;
        soundManager.UpdateSound(val);
    }

    public void MouseEnter()
    {
        // Cursor.SetCursor(null,Vector2.zero,CursorMode.)
    }

    public void MouseLeave()
    {
        
    }

    public void SliderUpdated()
    {
        UpdateSound(slider.value);
    }

    public void MouseClick()
    {
        if (slider.value > 0f) {
            lastMutedVol = slider.value;
            slider.value = 0f;
            UpdateSound(0f);
        }
        else {
            slider.value = lastMutedVol;
            UpdateSound(lastMutedVol);
        }
    }
}
