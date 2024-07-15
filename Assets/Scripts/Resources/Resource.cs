using System;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Collider))]
public abstract class Resource : PooledObject
{
    [SerializeField] private AudioClip[] _collectedSoundEffects;
    [SerializeField] private AudioClip _deliveredSoundEffect;
    [SerializeField] private Transform _rightHandle;
    [SerializeField] private Transform _leftHandle;
    [SerializeField] private Outline _outline;

    private Animator _animator;
    private Collider _collider;

    public static int IsStopped = Animator.StringToHash(nameof(IsStopped));

    public event Action<Resource> Delivered;

    public AudioClip CollectedSoundEffect => _collectedSoundEffects[UnityEngine.Random.Range(0, _collectedSoundEffects.Length - 1)];
    public AudioClip DeliveredSoundEffect => _deliveredSoundEffect;
    public Transform RightHandle => _rightHandle;
    public Transform LeftHandle => _leftHandle;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        Reset();
    }

    public void Carry()
    {
        _animator.SetBool(IsStopped, true);
        _outline.enabled = false;
        _collider.enabled = false;
    }

    public void Collect()
    {
        Delivered?.Invoke(this);
    }

    private void Reset()
    {
        _animator.SetBool(IsStopped, false);
        _outline.enabled = true;
        _collider.enabled = true;
    }
}
