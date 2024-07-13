using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Collider))]
public abstract class Resource : PooledObject
{
    [SerializeField] private ResourceDistributor _distributor;
    [SerializeField] private AudioClip[] _collectedSoundEffects;
    [SerializeField] private AudioClip _deliveredSoundEffect;
    [SerializeField] private Transform _rightHandle;
    [SerializeField] private Transform _leftHandle;
    [SerializeField] private Outline _outline;

    private Animator _animator;
    private Worker _worker;
    private float _workerOffset = 0.15f;
    private Coroutine _waitingForScaners;

    public event Action<Resource> Delivered;

    public AudioClip CollectedSoundEffect => _collectedSoundEffects[UnityEngine.Random.Range(0, _collectedSoundEffects.Length - 1)];
    public AudioClip DeliveredSoundEffect => _deliveredSoundEffect;
    public Transform RightHandle => _rightHandle;
    public Transform LeftHandle => _leftHandle;
    public Collider Collider { get; private set; }
    public int IsStopped { get; } = Animator.StringToHash(nameof(IsStopped));

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        Collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        _animator.SetBool(IsStopped, false);
        _outline.enabled = true;
    }

    private void OnDisable()
    {
        _waitingForScaners = null;
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

    public void Carry(Worker worker)
    {
        _worker = worker;
        _animator.SetBool(IsStopped, true);
        transform.up = Vector3.forward;
        _outline.enabled = false;
    }

    public void Collect()
    {
        Delivered?.Invoke(this);
        _worker = null;
    }

    public void Detect(ResourceScaner scaner)
    {
        _distributor.AddScaner(scaner);
        _waitingForScaners ??= StartCoroutine(WaitForScaners());
    }

    private IEnumerator WaitForScaners()
    {
        yield return new WaitForFixedUpdate();

        _distributor.CalculateClosestScaner(this);
    }
}
