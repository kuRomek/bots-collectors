using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WorkerMaker), typeof(AudioSource))]
public class Base : MonoBehaviour
{
    [SerializeField] private int _maxWorkers;
    [SerializeField] private int _startWorkerCount;
    [SerializeField] private int _woodCount;
    [SerializeField] private int _buildingCost;
    [SerializeField] private float _buildingDuration;
    [SerializeField] private WorkersPlace _workersPlace;
    [SerializeField] private Flag _flag;
    [SerializeField] private ResourceDistributor _distributor;
    [SerializeField] private SelectionEffects _selectionEffects;
    [SerializeField] private AudioPlayer _audioGeneral;
    [SerializeField] private ProgressBar _progressBar;
    [SerializeField] private MeshRenderer _model;
    [SerializeField] private Material _underConstructionMaterial;

    private WorkerMaker _workerMaker;
    private Queue<Resource> _resourcesToCollect = new Queue<Resource>();
    private List<Worker> _allWorkers = new List<Worker>();
    private Queue<Worker> _freeWorkers = new Queue<Worker>();
    private Worker _builder;
    private Material _defaultMaterial;

    public event Action OnWoodCountChanged;
    public event Action OnWorkerCountChanged;

    public WorkersPlace WorkersPlace => _workersPlace;
    public int WoodCount => _woodCount;
    public int WorkerCount => _allWorkers.Count;
    public float BuildingDuration => _buildingDuration;
    public Flag Flag => _flag;
    public bool HasBuilder => _builder != null;
    public SelectionEffects SelectionEffects => _selectionEffects;
    public ProgressBar ProgressBar => _progressBar;
    public MeshRenderer Model => _model;
    public AudioPlayer AudioGeneral => _audioGeneral;
    public AudioSource AudioSource { get; private set; }

    private void Awake()
    {
        _workerMaker = GetComponent<WorkerMaker>();
        AudioSource = GetComponent<AudioSource>();
        _defaultMaterial = _model.material;
    }

    private void Start()
    {
        StartCoroutine(AddStartingWorkers());
    }

    private void OnEnable()
    {
        _distributor.OnResourceDistributed += AddResourceToQueue;
        _flag.OnInstalled += SendWorkerForBuilding;
        _flag.OnRemove += PrepareWorker;
        _workerMaker.OnWorkerMade += AddWorker;
    }

    private void OnDisable()
    {
        _distributor.OnResourceDistributed -= AddResourceToQueue;
        _flag.OnInstalled -= SendWorkerForBuilding;
        _flag.OnRemove -= PrepareWorker;
        _workerMaker.OnWorkerMade -= AddWorker;

        foreach (Worker worker in _allWorkers)
            worker.OnResourceDelivered -= GetResource;
    }

    private IEnumerator AddStartingWorkers()
    {
        float delay = 0.2f;
        WaitForSeconds waitForSeconds = new WaitForSeconds(delay);

        for (int i = 0; i < _startWorkerCount; i++)
        {
            AddWorker();
            yield return waitForSeconds;
        }
    }

    private void GetResource(Worker worker, Resource resource)
    {
        ReleaseWorker(worker);

        _audioGeneral.PlayClip(resource.DeliveredSoundEffect);

        _woodCount++;
        OnWoodCountChanged?.Invoke();

        if (Flag.HasBeenInstalled == false)
            PrepareWorker();

        DistributeWork();
    }

    private void DistributeWork()
    {
        if (Flag.HasBeenInstalled)
            SendWorkerForBuilding();

        if (_resourcesToCollect.Count > 0 && _freeWorkers.Count > 0)
            SendWorkerForResource();
    }

    private void PrepareWorker()
    {
        if (WoodCount >= _workerMaker.Cost && _allWorkers.Count < _maxWorkers)
        {
            _workerMaker.BeginRecruiting();
            _woodCount -= _workerMaker.Cost;
            OnWoodCountChanged?.Invoke();
        }
    }

    public void AddWorker()
    {
        Worker worker = _workerMaker.Recruit();
        AddWorker(worker);
    }

    public void AddWorker(Worker worker)
    {
        worker.OnResourceDelivered += GetResource;
        _allWorkers.Add(worker);
        _freeWorkers.Enqueue(worker);

        OnWorkerCountChanged?.Invoke();

        DistributeWork();
    }

    private void AddResourceToQueue(Base @base, Resource resource)
    {
        if (this == @base)
        {
            _resourcesToCollect.Enqueue(resource);

            if (_freeWorkers.Count > 0)
                SendWorkerForResource();
        }
    }

    private void SendWorkerForResource()
    {
        _freeWorkers.Dequeue().GoForResource(_resourcesToCollect.Dequeue());
    }

    private void SendWorkerForBuilding()
    {
        if (_builder == null && WoodCount >= _buildingCost && _freeWorkers.Count > 0 && _allWorkers.Count > 1)
        {
            _builder = _freeWorkers.Dequeue();
            _allWorkers.Remove(_builder);
            _builder.OnResourceDelivered -= GetResource;
            _builder.OnBaseBuilding += ResetBuilder;
            _builder.GoBuildBase(Flag.transform.position);

            OnWorkerCountChanged?.Invoke();

            _woodCount -= _buildingCost;
            OnWoodCountChanged?.Invoke();
        }
    }

    public void CancelBuilding()
    {
        _woodCount += _buildingCost;
        OnWoodCountChanged?.Invoke();

        _allWorkers.Add(_builder);
        _freeWorkers.Enqueue(_builder);
        _builder.OnBaseBuilding -= ResetBuilder;
        _builder.OnResourceDelivered += GetResource;
        _builder.CancelBuilding();
        _builder = null;
        OnWorkerCountChanged?.Invoke();
    }

    public void SetBuildingState()
    {
        _model.material = _underConstructionMaterial;
    }

    public void SetBuiltState()
    {
        _model.material = _defaultMaterial;
    }

    private void ReleaseWorker(Worker worker)
    {
        _freeWorkers.Enqueue(worker);
    }

    private void RemoveFlag()
    {
        Flag.Reset();
        Flag.gameObject.SetActive(false);
    }

    private void ResetBuilder()
    {
        RemoveFlag();
        _builder.OnBaseBuilding -= ResetBuilder;
        _builder = null;
    }

    public void Reset()
    {
        _model.material = _defaultMaterial;
        _startWorkerCount = 0;
        _woodCount = 0;
        OnWoodCountChanged?.Invoke();
        _resourcesToCollect.Clear();
        _allWorkers.Clear();
        _freeWorkers.Clear();
        Flag.Reset();
    }
}
