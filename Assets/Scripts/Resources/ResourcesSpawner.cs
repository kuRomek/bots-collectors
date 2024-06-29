using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ResourcesSpawner : MonoBehaviour
{
    [SerializeField] private ResourcePool _pool;

    private Collider _spawnField;
    private float _minTimeToSpawn = 1;
    private float _maxTimeToSpawn = 4;

    private void Awake()
    {
        _spawnField = GetComponent<Collider>();
    }

    private void Start()
    {
        StartCoroutine(BeginSpawning());
    }

    private IEnumerator BeginSpawning()
    {
        bool isRunning = true;

        while (isRunning)
        {
            yield return new WaitForSeconds(Random.Range(_minTimeToSpawn, _maxTimeToSpawn));

            Resource resource = _pool.Get();

            float xPosition = Mathf.Pow(-1, Random.Range(0, 2)) * Random.Range(2f, _spawnField.bounds.max.x);
            float zPosition = Mathf.Pow(-1, Random.Range(0, 2)) * Random.Range(2f, _spawnField.bounds.max.z);

            resource.transform.position = new Vector3(xPosition, 0f, zPosition);
        }
    }
}