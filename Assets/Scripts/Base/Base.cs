using UnityEngine;

[RequireComponent(typeof(WorkerMaker), typeof(AudioSource))]
[RequireComponent(typeof(BaseResourceDistributor), typeof(BuildingLogic), typeof(WorkersBehavior))]
public class Base : MonoBehaviour
{
    [SerializeField] private BaseResourceDistributor _baseResourceDistributor;
    [SerializeField] private BuildingLogic _buildingLogic;
    [SerializeField] private WorkersBehavior _workersBehavior;
    [SerializeField] private WorkersPlace _workersPlace;
    [SerializeField] private Flag _flag;
    [SerializeField] private SelectionEffects _selectionEffects;
    [SerializeField] private ProgressBar _progressBar;
    [SerializeField] private MeshRenderer _model;

    private WorkerMaker _workerMaker;

    public WorkersPlace WorkersPlace => _workersPlace;
    public Flag Flag => _flag;
    public SelectionEffects SelectionEffects => _selectionEffects;
    public ProgressBar ProgressBar => _progressBar;
    public MeshRenderer Model => _model;
    public WorkersBehavior WorkersBehavior => _workersBehavior;
    public BaseResourceDistributor ResourceDistributor => _baseResourceDistributor;
    public BuildingLogic BuildingLogic => _buildingLogic;
    public int WorkerCount => WorkersBehavior.WorkerCount;
    public int WoodCount => ResourceDistributor.WoodCount;
    public bool HasBuilder => BuildingLogic.HasBuilder;
    public bool IsBuilding => BuildingLogic.IsBuilding;
    public AudioSource AudioSource { get; private set; }

    private void Awake()
    {
        _workerMaker = GetComponent<WorkerMaker>();
        AudioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        _flag.Installed += WorkersBehavior.SendWorkerForBuilding;
        _flag.Removed += WorkersBehavior.PrepareWorker;
        _workerMaker.WorkerMade += WorkersBehavior.AddWorker;
    }

    private void OnDisable()
    {
        _flag.Installed -= WorkersBehavior.SendWorkerForBuilding;
        _flag.Removed -= WorkersBehavior.PrepareWorker;
        _workerMaker.WorkerMade -= WorkersBehavior.AddWorker;
    }

    public void Reset()
    {
        ResourceDistributor.Reset();
        WorkersBehavior.Reset();
        BuildingLogic.Reset();
        _flag.Reset();
    }
}
