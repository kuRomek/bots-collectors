using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ResourcesSpawner : MonoBehaviour
{
    [SerializeField] private float _minTimeToSpawn;
    [SerializeField] private float _maxTimeToSpawn;
    [SerializeField] private float _spawnAcceleration;
    [SerializeField] private ResourcePool _pool;

    private Collider _spawnField;

    private void Awake()
    {
        _spawnField = GetComponent<Collider>();
    }

    private void Start()
    {
        StartCoroutine(BeginSpawning());
    }

    private void Update()
    {
        _minTimeToSpawn -= _spawnAcceleration * Time.deltaTime;
        _maxTimeToSpawn -= _spawnAcceleration * Time.deltaTime;
    }

    private IEnumerator BeginSpawning()
    {
        bool isRunning = true;

        while (isRunning)
        {
            yield return new WaitForSeconds(Random.Range(_minTimeToSpawn, _maxTimeToSpawn));

            Resource resource = _pool.Get();
            Vector3 spawnPoint;
            Collider[] hits;

            bool containsBase;

            do
            {
                spawnPoint = new Vector3(Random.Range(_spawnField.bounds.min.x, _spawnField.bounds.max.z),
                                         0f,
                                         Random.Range(_spawnField.bounds.min.x, _spawnField.bounds.max.z));

                hits = Physics.OverlapSphere(spawnPoint, 1f);

                containsBase = false;

                foreach (Collider hit in hits)
                {
                    if (hit.TryGetComponent(out Base _))
                    {
                        containsBase = true;
                        break;
                    }
                }
            }
            while (containsBase);
            
            resource.transform.position = spawnPoint;
        }
    }
}
