using System;
using UnityEngine;

public class Worker : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private WorkersPlace _workersPlace;
    [SerializeField] private Base _base;
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioPlayer _audio;
    [SerializeField] private IkResourceHandler _ikResourceHandler;

    private float _rotationSpeed = 15f;
    private float _distanceTolerance = 0.01f;
    private Resource _carryingResource;
    private Vector3 _target;
    private Vector3 _idlePlace;

    public event Action<Worker, Resource> OnResourceDelivered;

    public int IsRunning { get; } = Animator.StringToHash(nameof(IsRunning));

    private void Start()
    {
        _idlePlace = _workersPlace.TakeSpot();
        _target = _idlePlace;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, _target) > _distanceTolerance)
        {
            transform.position = Vector3.MoveTowards(transform.position, _target, _speed * Time.deltaTime);
            Vector3 lookDirection = _target - transform.position;
            lookDirection.y = 0f;
            transform.forward = Vector3.MoveTowards(transform.forward, lookDirection, _rotationSpeed * Time.deltaTime);
            _animator.SetBool(IsRunning, true);
        }
        else
        {
            _animator.SetBool(IsRunning, false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Resource resource) && Vector3.Distance(resource.transform.position, _target) < _distanceTolerance)
        {
            CollectResource(resource);

            _target = _base.transform.position;
        }

        if (other.TryGetComponent(out Base _) && _carryingResource != null)
        {
            _target = _idlePlace;
            _ikResourceHandler.DropResource();
            _carryingResource.BeCollected();
            OnResourceDelivered?.Invoke(this, _carryingResource);
            _carryingResource = null;
        }
    }

    private void CollectResource(Resource resource)
    {
        _audio.PlayClip(resource.CollectedSoundEffect);
        _carryingResource = resource;
        _ikResourceHandler.GrabResource(_carryingResource);
        _carryingResource.BeCarried(this);
    }

    public void GoForResource(Resource resource)
    {
        _target = resource.transform.position;
    }
}
