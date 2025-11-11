using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject moscaPrefab;
    [SerializeField]
    private GameObject moscaGrandePrefab;

    [SerializeField]
    private float moscaInterval = 3.5f;
    [SerializeField]
    private float moscaGrandeInterval = 10f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(spawnEnemy(moscaInterval, moscaPrefab));
        StartCoroutine(spawnEnemy(moscaGrandeInterval, moscaGrandePrefab));
    }

    private IEnumerator spawnEnemy(float interval, GameObject enemy)
    {
        yield return new WaitForSeconds(interval);
        GameObject newEnemy = Instantiate(enemy, new Vector3(Random.Range(-5f, 5), Random.Range(-6f, 6f), 0), Quaternion.identity);
        StartCoroutine(spawnEnemy(interval, enemy));
    }
}
