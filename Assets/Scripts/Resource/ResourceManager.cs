using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    private List<ResourceNode> _activeResources = new List<ResourceNode>();
    private Coroutine _spawnRoutine;

    private void Awake()
    {
        Instance = this;
    }

    public void StartSpawning(GameObject prefab, float interval)
    {
        if (_spawnRoutine != null)
            StopCoroutine(_spawnRoutine);
        _spawnRoutine = StartCoroutine(SpawnRoutine(prefab, interval));
    }

    public void UpdateSpawnInterval(GameObject prefab, float newInterval)
    {
        StartSpawning(prefab, newInterval);
    }

    public void SpawnResource(GameObject prefab)
    {
        Vector3 pos = Vector3.zero;
        NavMeshHit hit;
        bool found = false;
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPos = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            if (NavMesh.SamplePosition(randomPos, out hit, 2.0f, NavMesh.AllAreas))
            {
                pos = hit.position;
                found = true;
                break;
            }
        }
        if (found)
        {
            var obj = Instantiate(prefab, pos, Quaternion.identity);
            _activeResources.Add(obj.GetComponent<ResourceNode>());
        }
    }

    private IEnumerator SpawnRoutine(GameObject prefab, float interval)
    {
        while (true)
        {
            SpawnResource(prefab);
            yield return new WaitForSeconds(interval);
        }
    }

    public ResourceNode FindClosestAvailable(Vector3 from, Team team)
    {
        return _activeResources
            .Where(r => !r.IsReservedByTeam(team))
            .OrderBy(r => Vector3.Distance(from, r.transform.position))
            .FirstOrDefault();
    }

    public void CollectResource(ResourceNode node)
    {
        Destroy(node.gameObject);
    }

    public void OccupyResource(ResourceNode node)
    {
        _activeResources.Remove(node);
    }

    public bool ContainsResource(ResourceNode node)
    {
        return _activeResources.Contains(node);
    }
}
