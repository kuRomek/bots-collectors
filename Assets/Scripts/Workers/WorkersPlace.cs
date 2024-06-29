using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class WorkersPlace : MonoBehaviour 
{
    [SerializeField] private Base _base;

    private Queue<Vector3> _spots = new Queue<Vector3>();
    private IEnumerator<Vector3> _spotsIterator;
    private SphereCollider _collider;

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();

        for (int i = 0; i < _base.WorkerCount; i++)
            _spots.Enqueue(_collider.radius * new Vector3(Mathf.Cos(i * 2 * Mathf.PI / _base.WorkerCount), 0f, Mathf.Sin(i * 2 * Mathf.PI / _base.WorkerCount)));

        _spotsIterator = _spots.GetEnumerator();
    }

    public Vector3 TakeSpot()
    {
        if (_spotsIterator.MoveNext())
            return _spotsIterator.Current;
        else 
            return default;
    }
}
