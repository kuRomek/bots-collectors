using System;
using UnityEngine;

public abstract class Resource : PooledObject 
{
    [SerializeField] private AudioClip[] _collectedSoundEffects;
    [SerializeField] private AudioClip _carriedSoundEffect;

    public event Action<Resource> OnResourcePickedUp;

    public AudioClip CollectedSoundEffect => _collectedSoundEffects[UnityEngine.Random.Range(0, _collectedSoundEffects.Length - 1)];
    public AudioClip CarriedSoundEffect => _carriedSoundEffect;

    public void GetCollected()
    {
        OnResourcePickedUp?.Invoke(this);
        gameObject.SetActive(false);
    }
}
