using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Base))]
public class BaseResourceDistributor : MonoBehaviour
{
    [SerializeField] private AudioPlayer _audioGeneral;

    private Base _base;
    private int _woodCount;
    private Queue<Resource> _resourcesToCollect = new Queue<Resource>();

    public event Action WoodCountChanged;

    public int WoodCount => _woodCount;
    public Queue<Resource> ResourcesToCollect => _resourcesToCollect;

    private void Awake()
    {
        _base = GetComponent<Base>();
    }

    public void GetResource(Worker worker, Resource resource)
    {
        _base.WorkersBehavior.ReleaseWorker(worker);
        _audioGeneral.PlayClip(resource.DeliveredSoundEffect);
        IncrementWoodCount(1);

        if (_base.Flag.HasBeenInstalled == false)
            _base.WorkersBehavior.PrepareWorker();

        _base.WorkersBehavior.DistributeWork();
    }

    public void AddResourceToQueue(Resource resource)
    {
        _resourcesToCollect.Enqueue(resource);

        if (_base.WorkersBehavior.FreeWorkerCount > 0)
            _base.WorkersBehavior.SendWorkerForResource();
    }

    public Resource TakeAvailableResource()
    {
        return _resourcesToCollect.Dequeue();
    }

    public void DecrementWoodCount(int amount)
    {
        _woodCount -= amount;
        WoodCountChanged?.Invoke();
    }

    public void IncrementWoodCount(int amount)
    {
        _woodCount += amount;
        WoodCountChanged?.Invoke();
    }

    public void Reset()
    {
        _woodCount = 0;
        WoodCountChanged?.Invoke();
        _resourcesToCollect.Clear();
    }
}
