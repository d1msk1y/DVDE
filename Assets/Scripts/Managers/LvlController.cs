using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LvlController : MonoBehaviour
{
    [Header("Lvl parts references")]
    public EnemySpawner[] spawners;
    public Door l_Door;
    public Door r_Door;

    [Header("Info")]
    public int lvlIndex;
    public int currentLevelStage;

    [Header("Lvl properties n' complexity")]
    public float spawnRate;
    public int maxEnemiesInAction;
    public int enemies2Spawn;
    public int enemiesSpawned;

    [Header("Lvl canvas")]
    public Text lvlStageTXT;

    public GameObject[] enemies;
    public GameObject doorTrigger;

    public GameObject[] currentEnemiesInAction;
    private void OnEnable()
    {
        GameManager.instance.lvlManager.lvlController = this;
        SetUp();
    }
    void Start()
    {
        GameManager.instance.lvlManager.lvlController = this;
        SetUp();
    }
    
    void SetUp()
    {
        enemies2Spawn = GameManager.instance.lvlManager.currentEnemiesToSpawnStage;
        lvlStageTXT.text = "STAGE " + lvlIndex;
        foreach (EnemySpawner spawner in spawners)
        {
            spawner.enemies = enemies;
            spawner.spawnRate = spawnRate;
            spawner.lvlController = GameManager.instance.lvlManager.lvlController;
            spawner.maxEnemy = GameManager.instance.lvlManager.currentLevelStage;
        }
    }

    void Update()
    {

        int lvlDiff = GameManager.instance.lvlManager.currentLevel - lvlIndex;
        if(lvlDiff > 2)
        {
            Destroy(gameObject);
        }

        if (GameManager.instance.lvlManager.lvlController != this)
            return;

        currentEnemiesInAction = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemiesSpawned < enemies2Spawn && currentEnemiesInAction.Length <= 0)
        {
            if (lvlIndex > GameManager.instance.lvlManager.currentLevel)
            {
                StartCoroutine(r_Door.GetComponent<Door>().CloseTheDoor());//Right door close.
                StartCoroutine(l_Door.GetComponent<Door>().OpenTheDoor());//Left door open.
            }

        }
        if (lvlIndex == GameManager.instance.lvlManager.currentLevel && enemiesSpawned >= enemies2Spawn && currentEnemiesInAction.Length <= 0)
        {
            StartCoroutine(r_Door.GetComponent<Door>().OpenTheDoor());;//Right door open.
            StartCoroutine(l_Door.GetComponent<Door>().CloseTheDoor());//Left door close.
        }

        if (lvlIndex == GameManager.instance.lvlManager.currentLevel)
        {
            StartCoroutine(l_Door.GetComponent<Door>().CloseTheDoor());//Left door close.
        }


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (GameManager.instance.lvlManager.currentLevel >= lvlIndex)
                return;
            GameManager.instance.lvlManager.SwitchLevel();
        }
    }
}
