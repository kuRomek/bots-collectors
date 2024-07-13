using System;
using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
    [SerializeField] private Base _homeBase;
    [SerializeField] private AudioClip _constructionSound;
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioPlayer _audio;
    [SerializeField] private IkResourceHandler _ikResourceHandler;
    [SerializeField] private NavMeshAgent _navMeshAgent;

    private Resource _carryingResource;
    private Vector3 _idlePlace;
    private bool _isBuilding = false;

    public event Action<Worker, Resource> OnResourceDelivered;
    public event Action OnBaseBuilding;

    protected float DistanceTolerance { get; private set; } = 0.001f;
    public int IsRunning { get; } = Animator.StringToHash(nameof(IsRunning));

    private void Start()
    {
        _navMeshAgent.SetDestination(_idlePlace);
    }

    private void OnEnable()
    {
        _homeBase.WorkersPlace.OnSpotsCalculated += TakeSpot;
    }

    private void OnDisable()
    {
        _homeBase.WorkersPlace.OnSpotsCalculated -= TakeSpot;
    }

    private void Update()
    {
        if (Vector3.SqrMagnitude(_navMeshAgent.destination - transform.position) < DistanceTolerance)
            _animator.SetBool(IsRunning, false);
        else
            _animator.SetBool(IsRunning, true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Resource resource) && Vector3.SqrMagnitude(_navMeshAgent.destination - resource.transform.position) < DistanceTolerance)
        {
            CollectResource(resource);

            _navMeshAgent.SetDestination(_homeBase.transform.position);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_carryingResource != null && other.TryGetComponent(out Base @base) && @base == _homeBase)
        {
            _navMeshAgent.SetDestination(_idlePlace);
            _ikResourceHandler.DropResource();
            _carryingResource.Collect();
            OnResourceDelivered?.Invoke(this, _carryingResource);
            _carryingResource = null;
        }

        if (other.TryGetComponent(out Flag flag) && Vector3.SqrMagnitude(_navMeshAgent.destination - flag.transform.position) < DistanceTolerance)
            BuildBase();
    }

    private void TakeSpot()
    {
        if (_isBuilding == false)
        {
            if (Vector3.SqrMagnitude(_navMeshAgent.destination - _idlePlace) < DistanceTolerance)
            {
                _idlePlace = _homeBase.WorkersPlace.TakeSpot();
                _navMeshAgent.SetDestination(_idlePlace);
            }
            else
            {
                _idlePlace = _homeBase.WorkersPlace.TakeSpot();
            }
        }
    }

    private void CollectResource(Resource resource)
    {
        _audio.PlayClip(resource.CollectedSoundEffect);
        _carryingResource = resource;
        _ikResourceHandler.GrabResource(_carryingResource);
        _carryingResource.Carry(this);
    }

    public void GoForResource(Resource resource)
    {
        _navMeshAgent.SetDestination(resource.transform.position);
    }

    public void GoBuildBase(Vector3 point)
    {
        _isBuilding = true;
        _navMeshAgent.SetDestination(point);
    }

    public void CancelBuilding()
    {
        _isBuilding = false;
        _navMeshAgent.SetDestination(_idlePlace);
    }

    private void BuildBase()
    {
        gameObject.SetActive(false);

        OnBaseBuilding?.Invoke();
        _homeBase.SelectionEffects.RemoveOutline();
        _homeBase = Instantiate(_homeBase, transform.position, Quaternion.identity);
        _homeBase.Reset();
        _homeBase.BuildingLogic.SetBuildingState();
        _homeBase.AudioSource.PlayOneShot(_constructionSound);

        _homeBase.ProgressBar.BeginMaking(ProgressBar.Mode.Building, _homeBase.BuildingLogic.BuildingDuration);
        _homeBase.ProgressBar.Progressing.onComplete += FinishBuilding;
    }

    private void FinishBuilding()
    {
        _homeBase.ProgressBar.Progressing.onComplete -= FinishBuilding;
        _homeBase.BuildingLogic.SetBuiltState();
        _isBuilding = false;
        transform.position = _homeBase.Flag.transform.position;
        gameObject.SetActive(true);
        _homeBase.WorkersBehavior.AddWorker(this);
    }
}
