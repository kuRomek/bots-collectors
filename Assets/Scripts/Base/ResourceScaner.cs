using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ResourceScaner : MonoBehaviour
{
    [SerializeField] private Base _base;
    [SerializeField] private ResourceDistributor _resourceDistributor;

    private void OnTriggerEnter(Collider other)
    {
        if (_base.IsBuilding == false && other.TryGetComponent(out Resource resource))
            _resourceDistributor.AddBase(_base, resource);
    }
}
