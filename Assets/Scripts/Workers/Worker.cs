using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Worker : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private WorkersPlace _workersPlace;
    [SerializeField] private Base _base;
    [SerializeField] private Animator _animator;

    private float _rotationSpeed = 15f;
    private AudioSource _audio;
    private Resource _carryingResource;
    private Vector3 _target;
    private Vector3 _idlePlace;

    public event Action<Worker, Resource> OnResourceDelivered;

    public int IsRunning { get; } = Animator.StringToHash(nameof(IsRunning));

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _idlePlace = _workersPlace.TakeSpot();
        _target = _idlePlace;
    }

    private void Update()
    {
        if (_target.x != transform.position.x && _target.z != transform.position.z)
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
        if (other.TryGetComponent(out Resource resource) && resource.transform.position == _target)
        {
            _audio.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
            _audio.PlayOneShot(resource.CollectedSoundEffect);
            resource.GetCollected();
            _carryingResource = resource;
            
            _target = _base.transform.position;
        }

        if (other.TryGetComponent(out Base _) && _carryingResource != null)
        {
            _target = _idlePlace;
            OnResourceDelivered?.Invoke(this, _carryingResource);
            _carryingResource = null;
        }
    }

    public void GoForResource(Resource resource)
    {
        _target = resource.transform.position;
    }
}
