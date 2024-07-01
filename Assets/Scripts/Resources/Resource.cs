using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class Resource : PooledObject
{
    [SerializeField] private AudioClip[] _collectedSoundEffects;
    [SerializeField] private AudioClip _carriedSoundEffect;
    [SerializeField] public Transform _rightHandle;
    [SerializeField] public Transform _leftHandle;

    private Animator _animator;
    private Worker _worker;
    private float _workerOffset = 0.15f;

    public event Action<Resource> OnDelivered;

    public AudioClip CollectedSoundEffect => _collectedSoundEffects[UnityEngine.Random.Range(0, _collectedSoundEffects.Length - 1)];
    public AudioClip CarriedSoundEffect => _carriedSoundEffect;
    public Transform RightHandle => _rightHandle;
    public Transform LeftHandle => _leftHandle;
    public int IsStopped { get; } = Animator.StringToHash(nameof(IsStopped));

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _animator.SetBool(IsStopped, false);
    }

    private void Update()
    {
        if (_worker != null)
        {
            transform.position = _worker.transform.position + _worker.transform.forward * _workerOffset + Vector3.up * 0.53f;

            Vector3 rotation = transform.eulerAngles;
            rotation = new Vector3(rotation.x, _worker.transform.eulerAngles.y, rotation.z);
            transform.eulerAngles = rotation;
        }
    }

    public void BeCarried(Worker worker)
    {
        _worker = worker;
        _animator.SetBool(IsStopped, true);
        transform.up = Vector3.forward;
    }

    public void BeCollected()
    {
        OnDelivered?.Invoke(this);
        _worker = null;
        gameObject.SetActive(false);
    }
}
