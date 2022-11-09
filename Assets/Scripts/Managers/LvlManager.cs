using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvlManager : MonoBehaviour
{
    public int levelStageRange;
    public int weaponsStageRange;
    public int enemiesToSpawnStageRange;
    [Space(10)]
    public GameObject lvlPrefab;

    [Header("GFX")]
    public ParticleSystem bgParticle;
    public EtherealSpawner etherealSpawner;

    [Header("Info")]
    public LvlController lvlController;
    public int currentLevel;

    [Tooltip("Max enemies in action")]
    public int currentLevelStage;
    public int currentWeaponsStage;
    public int currentEnemiesToSpawnStage;

    public int enemies2Spawn;
    public int enemiesSpawned;

    private GameObject _newLvl;

    private int startLevelStage;
    private int startWeaponsStage;
    private int startEnemiesToSpawnStage;
    [HideInInspector] public GameObject[] currentEnemiesInAction;

    private void Start()
    {
        startLevelStage = currentLevelStage;
        startWeaponsStage = currentWeaponsStage;
        startEnemiesToSpawnStage = currentEnemiesToSpawnStage;
        
        if (lvlController == null && currentLevel == 0)
        {
            SpawnFirstLevel();
        }
    }

    private void Update()
    {
        if (lvlController == null && currentLevel == 0)
        {
            SpawnFirstLevel();
        }

        if (lvlController == null)
            return;

        enemies2Spawn = lvlController.enemies2Spawn;
        enemiesSpawned = lvlController.enemiesSpawned;
        
        if (enemiesSpawned < enemies2Spawn || lvlController.CurrentEnemiesInAction.Count > 0 || currentLevel != lvlController.lvlIndex) return;
        SpawnNewLevel();
        GameManager.instance.soundManager.SwitchLowPassFrequency();
        GameManager.instance.isCurrentBattle = false;
    }

    public void SwitchLevel()
    {
        currentLevel += 1;

        GameManager.instance.statsManager.passedRooms += 1;
        PlayerPrefs.SetInt(GameManager.instance.statsManager.keys[5], GameManager.instance.statsManager.passedRooms);

        GameManager.instance.aStarManager.AStar.transform.position += new Vector3(30, 0, 0);
        GameManager.instance.soundManager.SwitchLowPassFrequency();
        GameManager.instance.UiManager.ShowHUD();
        GameManager.instance.aStarManager.AStar.Scan();
        GameManager.instance.isCurrentBattle = true;
        GameManager.instance.isGameStarted = true;

    }

    private void SpawnNewLevel()
    {
        _newLvl = Instantiate(lvlPrefab, lvlController.transform.position + new Vector3(30, 0, 0), Quaternion.identity);
        enemies2Spawn = _newLvl.GetComponent<LvlController>().enemies2Spawn;
        enemiesSpawned = _newLvl.GetComponent<LvlController>().enemiesSpawned;

        lvlController.lvlIndex = currentLevel + 1;

        SetLevelComplexity();

        _newLvl.GetComponent<LvlController>().currentLevelStage = currentLevelStage;
        GameManager.instance.aStarManager.UpdateAstarPosition(new Vector3(_newLvl.transform.position.x, _newLvl.transform.position.y, 0));
        GameManager.instance.isGameStarted = true;
    }

    private void SetLevelComplexity()
    {
        currentLevelStage = (currentLevel / levelStageRange) + startLevelStage;
        currentWeaponsStage = (currentLevel / weaponsStageRange) + startWeaponsStage;

        currentEnemiesToSpawnStage = (currentLevel / enemiesToSpawnStageRange) + startEnemiesToSpawnStage;

    }

    private void SpawnFirstLevel()
    {
        ResetLvlManager();
        
        bgParticle = etherealSpawner.GetRandomParticle();
        _newLvl = Instantiate(lvlPrefab, Vector3.zero, Quaternion.identity);

        lvlController.lvlIndex = currentLevel + 1;

        _newLvl.GetComponent<LvlController>().currentLevelStage = startLevelStage;
        GameManager.instance.aStarManager.UpdateAstarPosition(new Vector3(_newLvl.transform.position.x, _newLvl.transform.position.y, 0));
        GameManager.instance.isGameStarted = false;
    }

    public void ResetLvlManager()
    {
        ResetManagerValues();
        DestroyAllLevels();
        DestroyAllEnemies();
        DestroyAllPickUpAble();
        DestroyAllObstacles();
        ResetLvlproperties();
    }

    #region Level reset functions

    private void ResetManagerValues()
    {
        currentLevel = 0;
        currentLevelStage = startLevelStage;
        lvlController = null;
    }

    public void ClearLevel()
    {
        lvlController.enemiesSpawned = lvlController.enemies2Spawn;
        DestroyAllEnemies();
    }
    private void DestroyAllLevels()
    {
        LvlController[] lvls = GameObject.FindObjectsOfType<LvlController>();
        foreach (LvlController lvlController in lvls)
        {
            Destroy(lvlController.gameObject);
        }
    }

    private void DestroyAllPickUpAble()
    {
        Interactable[] interactables = FindObjectsOfType<Interactable>();
        foreach (Interactable interactable in interactables)
        {
            if (interactable.gameObject.GetComponent<Interactable>().isDestroyable)
            {
                Destroy(interactable.gameObject);
            }
        }
    }

    private void DestroyAllEnemies()
    {
        EnemyController[] enemies = GameObject.FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
    }

    private void DestroyAllObstacles()
    {
        LevelGeneration[] spawners = GameObject.FindObjectsOfType<LevelGeneration>();
        foreach (LevelGeneration spawner in spawners)
        {
            Destroy(spawner.transform.parent.gameObject);
        }
    }

    public void ResetLvlproperties()
    {
        currentLevelStage = startLevelStage;
        currentWeaponsStage = startWeaponsStage;
        currentEnemiesToSpawnStage = startEnemiesToSpawnStage;
    }

    #endregion

}
