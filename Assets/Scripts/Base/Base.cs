using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Base : MonoBehaviour
{
    [SerializeField] private ResourceScaner _scaner;
    [SerializeField] private Worker[] _workers;

    private AudioSource _audio;
    private Queue<Resource> _resourcesToCollect = new Queue<Resource>();
    private Queue<Worker> _freeWorkers;

    public event Action<Resource> OnResourceCollected;

    public int WoodCount { get; private set; } = 0;
    public int WorkerCount => _freeWorkers.Count;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
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
        FreeWorker(worker);

        switch (resource)
        {
            case Wood:
                WoodCount++;
                break;
        }

        _audio.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        _audio.PlayOneShot(resource.CarriedSoundEffect);

        OnResourceCollected?.Invoke(resource);

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

    private void FreeWorker(Worker worker)
    {
        _freeWorkers.Enqueue(worker);
    }
}
