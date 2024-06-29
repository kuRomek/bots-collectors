using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ResourceScaner : MonoBehaviour
{
    public event Action<Resource> OnResourceFound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Resource resource))
            OnResourceFound?.Invoke(resource);
    }
}
