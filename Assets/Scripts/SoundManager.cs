using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioSource source;
    public AudioClip placeSFX;
    public AudioClip liftSFX;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        source = GetComponent<AudioSource>();
    }

    public void PlacePiece()
    {
        source.PlayOneShot(placeSFX);
    }

    public void LiftPiece()
    {
        source.PlayOneShot(liftSFX);
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
