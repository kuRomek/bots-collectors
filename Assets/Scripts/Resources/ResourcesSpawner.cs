using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ResourcesSpawner : MonoBehaviour
{
    [SerializeField, Min(0.3f)] private float _minRandomTimeToSpawn;
    [SerializeField, Min(0.3f)] private float _maxRandomTimeToSpawn;
    [SerializeField] private float _spawnAcceleration;
    [SerializeField] private ResourcePool _pool;

    private float _minTimeToSpawn = 0.3f;
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
        if (_minRandomTimeToSpawn > _minTimeToSpawn)
        {
            _minRandomTimeToSpawn -= _spawnAcceleration * Time.deltaTime;
            _maxRandomTimeToSpawn -= _spawnAcceleration * Time.deltaTime;
        }
    }

    private IEnumerator BeginSpawning()
    {
        bool isRunning = true;

        while (isRunning)
        {
            yield return new WaitForSeconds(Random.Range(_minRandomTimeToSpawn, _maxRandomTimeToSpawn));

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
