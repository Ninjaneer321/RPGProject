using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGeneration : MonoBehaviour
{

    [SerializeField] List<GameObject> enemySpawnersInScene;
    [SerializeField] GameObject npcsInScene;

    public List<GameObject> enemiesToLoad;
    public List<GameObject> enemiesToUnload;

    [SerializeField] float terrainCullingDistance = 100f;

    public List<float> distances;

    // Update is called once per frame
    void Update()
    {
        UpdateTerrains();
    }


    void UpdateTerrains()
    {
        ClearTerrainLists();
        UpdateTerrainDistances();
        LoadAndUnloadTerrain();
    }

    void ClearTerrainLists()
    {
        distances.Clear();
        enemiesToUnload.Clear();
        enemiesToLoad.Clear();
    }

    void UpdateTerrainDistances()
    {
        foreach (var enemySpawner in enemySpawnersInScene)
        {

            float distance = Vector3.Distance(transform.position, enemySpawner.transform.position);
            distances.Add(distance);

            if (distance >= terrainCullingDistance)
            {
                enemiesToUnload.Add(enemySpawner.transform.GetChild(1).gameObject);
            }
            else
            {
                enemiesToLoad.Add(enemySpawner.transform.GetChild(1).gameObject);
            }
        }

        foreach (Transform child in npcsInScene.transform)
        {

            float distance = Vector3.Distance(transform.position, child.transform.position);
            distances.Add(distance);

            if (distance >= terrainCullingDistance)
            {
                enemiesToUnload.Add(child.gameObject);
            }
            else
            {
                enemiesToLoad.Add(child.gameObject);
            }
        }
    }

    void LoadAndUnloadTerrain()
    {
        foreach (var item in enemiesToLoad)
        {
            item.SetActive(true);
        }

        foreach (var item in enemiesToUnload)
        {
            item.SetActive(false);
        }
    }
}
