using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Base))]
public class WorkersBehavior : MonoBehaviour
{
    [SerializeField] private int _maxWorkers;
    [SerializeField] private int _startWorkerCount;
    [SerializeField] private WorkerMaker _workerMaker;
    [SerializeField] private BuildingLogic _buildingLogic;

    private Base _base;
    private List<Worker> _allWorkers = new List<Worker>();
    private Queue<Worker> _freeWorkers = new Queue<Worker>();

    public event Action WorkerCountChanged;

    public int WorkerCount => _allWorkers.Count;
    public int FreeWorkerCount => _freeWorkers.Count;

    private void Awake()
    {
        _base = GetComponent<Base>();
    }

    private void Start()
    {
        StartCoroutine(AddStartingWorkers());
    }

    private void OnDisable()
    {
        foreach (Worker worker in _allWorkers)
            worker.OnResourceDelivered -= _base.ResourceDistributor.GetResource;
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

    public void AddWorker()
    {
        Worker worker = _workerMaker.Recruit();
        AddWorker(worker);
    }

    public void AddWorker(Worker worker)
    {
        worker.OnResourceDelivered += _base.ResourceDistributor.GetResource;
        _allWorkers.Add(worker);
        _freeWorkers.Enqueue(worker);

        WorkerCountChanged?.Invoke();

        DistributeWork();
    }

    public void DistributeWork()
    {
        if (_base.Flag.HasBeenInstalled)
            SendWorkerForBuilding();

        if (_base.ResourceDistributor.ResourcesToCollect.Count > 0 && _freeWorkers.Count > 0)
            SendWorkerForResource();
    }

    public void SendWorkerForResource()
    {
        _freeWorkers.Dequeue().GoForResource(_base.ResourceDistributor.TakeAvailableResource());
    }

    public void SendWorkerForBuilding()
    {
        if (_buildingLogic.HasBuilder == false && _base.ResourceDistributor.WoodCount >= _buildingLogic.BuildingCost && FreeWorkerCount > 0 && WorkerCount > 1)
        {
            Worker builder = _freeWorkers.Dequeue();

            _buildingLogic.SetBuilder(builder);
            _allWorkers.Remove(builder);
            WorkerCountChanged?.Invoke();

            _base.ResourceDistributor.DecrementWoodCount(_buildingLogic.BuildingCost);
        }
    }

    public void PrepareWorker()
    {
        if (_base.ResourceDistributor.WoodCount >= _workerMaker.Cost && _allWorkers.Count < _maxWorkers)
        {
            _workerMaker.BeginRecruiting();
            _base.ResourceDistributor.DecrementWoodCount(_workerMaker.Cost);
        }
    }

    public void CancelBuilding()
    {
        _base.ResourceDistributor.IncrementWoodCount(_buildingLogic.BuildingCost);

        _allWorkers.Add(_buildingLogic.Builder);
        _freeWorkers.Enqueue(_buildingLogic.Builder);
        _buildingLogic.Builder.OnBaseBuilding -= _buildingLogic.RemoveBuilder;
        _buildingLogic.Builder.OnResourceDelivered += _base.ResourceDistributor.GetResource;
        _buildingLogic.Builder.CancelBuilding();
    }

    public void ReleaseWorker(Worker worker)
    {
        _freeWorkers.Enqueue(worker);
    }

    public void Reset()
    {
        _startWorkerCount = 0;
        _allWorkers.Clear();
        _freeWorkers.Clear();
    }
}
