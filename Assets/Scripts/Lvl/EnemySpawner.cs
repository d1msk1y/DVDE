using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public LvlController lvlController;

    public GameObject[] enemies;

    public int maxEnemy;

    public float spawnRate;
    private float _currentSpawnRate;

    private void Start()
    {
        _currentSpawnRate = spawnRate;
    }

    private void Update()
    {
        SpawnEnemy();

        if(_currentSpawnRate >= 0)
            _currentSpawnRate -= 1 * Time.deltaTime;
    }

    void SpawnEnemy()
    {
        if (_currentSpawnRate > 0 || 
            lvlController.CurrentEnemiesInAction.Count >= lvlController.maxEnemiesInAction || 
            lvlController.enemiesSpawned >= lvlController.enemies2Spawn ||
            GameManager.instance.lvlManager.currentLevel < lvlController.lvlIndex)
            return;

        lvlController.enemiesSpawned += 1;

        int randEnemy = Random.Range(0, maxEnemy);
        if(randEnemy > enemies.Length)
        {
            randEnemy = enemies.Length;
        }
        GameObject object2Spawn = enemies[randEnemy];

        GameObject wall = Instantiate(object2Spawn, transform.position, Quaternion.identity, GameManager.instance.lvlManager.lvlController.transform);
        wall.transform.parent = lvlController.transform.parent;
        _currentSpawnRate = spawnRate;

        //GameManager.instance.lvlManager.currentEnemiesInAction = GameObject.FindGameObjectsWithTag("Enemy");
    }

}
