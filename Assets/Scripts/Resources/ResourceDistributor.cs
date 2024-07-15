using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDistributor : MonoBehaviour
{
    private List<Base> _bases = new List<Base>();

    private Coroutine _waitingForScaners;

    public void AddBase(Base @base, Resource resource)
    {
        _bases.Add(@base);
        _waitingForScaners ??= StartCoroutine(WaitForScaners(resource));
    }

    private IEnumerator WaitForScaners(Resource resource)
    {
        yield return new WaitForFixedUpdate();

        CalculateClosestBase(resource);
        _waitingForScaners = null;
    }

    public void CalculateClosestBase(Resource resource)
    {
        float minDistance = float.MaxValue;
        float distance;

        Base closestBase = null;

        foreach (Base @base in _bases)
        {
            distance = Vector3.SqrMagnitude(@base.transform.position - resource.transform.position);

            if (Vector3.SqrMagnitude(@base.transform.position - resource.transform.position) < minDistance)
            {
                minDistance = distance;
                closestBase = @base;
            }
        }

        closestBase.ResourceDistributor.AddResourceToQueue(resource);

        _bases.Clear();
    }
}