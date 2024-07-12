using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ResourceScaner : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Resource resource))
            resource.Detect(this);
    }
}
