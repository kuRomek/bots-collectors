using System;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private ResourceScaner _scaner;
    [SerializeField] private Worker[] _workers;
    [SerializeField] private AudioPlayer _audio;

    private Queue<Resource> _resourcesToCollect = new Queue<Resource>();
    private Queue<Worker> _freeWorkers;

    public event Action OnWoodCollected;

    public int WoodCount { get; private set; } = 0;
    public int WorkerCount => _freeWorkers.Count;

    private void Awake()
    {
        _freeWorkers = new Queue<Worker>(_workers);
    }

    private void OnEnable()
    {
        _scaner.OnResourceFound += AddResourceToQueue;

        foreach (Worker worker in _freeWorkers)
            worker.OnResourceDelivered += GetResource;
    }

    private void OnDisable()
    {
        _scaner.OnResourceFound -= AddResourceToQueue;

        foreach (Worker worker in _freeWorkers)
            worker.OnResourceDelivered -= GetResource;
    }

    private void GetResource(Worker worker, Resource resource)
    {
        ReleaseWorker(worker);

        _audio.PlayClip(resource.CarriedSoundEffect);

        WoodCount++;
        OnWoodCollected?.Invoke();

        if (_resourcesToCollect.Count > 0)
            SendWorkerForResource();
    }

    private void AddResourceToQueue(Resource resource)
    {
        _resourcesToCollect.Enqueue(resource);

        if (_freeWorkers.Count > 0)
            SendWorkerForResource();
    }

    private void SendWorkerForResource()
    {
        _freeWorkers.Dequeue().GoForResource(_resourcesToCollect.Dequeue());
    }

    private void ReleaseWorker(Worker worker)
    {
        _freeWorkers.Enqueue(worker);
    }
}
