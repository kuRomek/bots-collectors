using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class WorkersPlace : MonoBehaviour 
{
    [SerializeField] private Base _base;

    private SphereCollider _collider;
    private IEnumerator<Vector3> _spotsIterator;

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        Queue<Vector3> spots = new Queue<Vector3>();

        for (int i = 0; i < _base.WorkerCount; i++)
            spots.Enqueue(_collider.radius * new Vector3(Mathf.Cos(i * 2 * Mathf.PI / _base.WorkerCount), 0f, Mathf.Sin(i * 2 * Mathf.PI / _base.WorkerCount)));

        _spotsIterator = spots.GetEnumerator();
    }

    public Vector3 TakeSpot()
    {
        if (_spotsIterator.MoveNext())
            return _spotsIterator.Current;
        else 
            return default;
    }
}
