using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<Type> : MonoBehaviour where Type : PooledObject
{
    [SerializeField] private Type _object;

    private Queue<Type> _pool = new Queue<Type>();

    public virtual Type Get()
    {
        Type newObject;

        if (_pool.Count == 0)
            newObject = Instantiate(_object);
        else
            newObject = _pool.Dequeue();

        newObject.transform.rotation = Quaternion.identity;

        return newObject;
    }

    public virtual void Release(Type objectToRelease)
    {
        _pool.Enqueue(objectToRelease);
    }

    public virtual void Reset()
    {
        foreach (Type freeObject in _pool)
            Destroy(freeObject.gameObject);

        _pool.Clear();
    }
}