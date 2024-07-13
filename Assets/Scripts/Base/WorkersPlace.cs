using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class WorkersPlace : MonoBehaviour 
{
    [SerializeField] private Base _base;

    private IEnumerator<Vector3> _spots;
    private SphereCollider _collider;

    public event Action SpotsCalculated;

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();

        CalculateSpots();
    }

    private void OnEnable()
    {
        _base.WorkersBehavior.WorkerCountChanged += CalculateSpots;
    }

    private void OnDisable()
    {
        _base.WorkersBehavior.WorkerCountChanged -= CalculateSpots;
    }

    private void CalculateSpots()
    {
        Queue<Vector3> spots = new Queue<Vector3>();

        for (int i = 0; i < _base.WorkerCount; i++)
            spots.Enqueue(_collider.radius * new Vector3(Mathf.Cos(i * 2 * Mathf.PI / _base.WorkerCount), 
                                                         0f, 
                                                         Mathf.Sin(i * 2 * Mathf.PI / _base.WorkerCount)) +
                          _collider.bounds.center);

        _spots = spots.GetEnumerator();

        SpotsCalculated?.Invoke();
    }

    public Vector3 TakeSpot()
    {
        return _spots.MoveNext() ? _spots.Current : default;
    }
}
