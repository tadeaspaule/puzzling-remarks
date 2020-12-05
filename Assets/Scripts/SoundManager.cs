using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource source;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        source = GetComponent<AudioSource>();
    }

    public void UpdateSound(float val)
    {
        source.volume = val;
    }

    public float GetVolume()
    {
        return source.volume;
    }
}
