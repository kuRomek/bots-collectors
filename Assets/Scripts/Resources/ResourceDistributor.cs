using System.Collections.Generic;
using UnityEngine;

public class ResourceDistributor : MonoBehaviour
{
    private List<ResourceScaner> _scaners = new List<ResourceScaner>();

    public void AddScaner(ResourceScaner scaner)
    {
        _scaners.Add(scaner);
    }

    public void CalculateClosestScaner(Resource resource)
    {
        float minDistance = float.MaxValue;
        float distance;

        ResourceScaner closestScaner = null;

        foreach (ResourceScaner scaner in _scaners)
        {
            distance = Vector3.SqrMagnitude(scaner.transform.position - resource.transform.position);

            if (Vector3.SqrMagnitude(scaner.transform.position - resource.transform.position) < minDistance)
            {
                minDistance = distance;
                closestScaner = scaner;
            }
        }

        closestScaner.GetComponentInParent<Base>().ResourceDistributor.AddResourceToQueue(resource);

        _scaners.Clear();
    }
}