using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    private List<ResourceNode> _activeResources = new List<ResourceNode>();

    private void Awake()
    {
        Instance = this;
    }

    public void StartSpawning(GameObject prefab, float interval)
    {
        StartCoroutine(SpawnRoutine(prefab, interval));
    }

    private IEnumerator SpawnRoutine(GameObject prefab, float interval)
    {
        while (true)
        {
            Vector3 pos = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            var obj = Instantiate(prefab, pos, Quaternion.identity);
            _activeResources.Add(obj.GetComponent<ResourceNode>());
            yield return new WaitForSeconds(interval);
        }
    }

    public ResourceNode FindClosestAvailable(Vector3 from)
    {
        return _activeResources
            .Where(r => !r.IsReserved)
            .OrderBy(r => Vector3.Distance(from, r.transform.position))
            .FirstOrDefault();
    }

    public void CollectResource(ResourceNode node)
    {
        _activeResources.Remove(node);
        Destroy(node.gameObject);
    }
}
