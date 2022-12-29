using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using EZCameraShake;
using FMODUnity;

public class GameManager : MonoBehaviour
{
    [Header("Controller n` Managers")]
    public AStarManager aStarManager;
    public LvlManager lvlManager;
    public SoundManager soundManager;
    public CameraMotor cameraMotor;
    public CameraShaker cameraShaker;
    public UiManager UiManager;
    public ScoreManager scoreManager;
    public ItemsManager itemsManager;
    public DataManager dataManager;
    public StatsManager statsManager;
    public UnlocksLog unlocksLog;
    public TutorialManager tutorialManager;

    public GlitchEffect glitchEffect;

    [Header("Player relative")]
    public PlayerController player;
    public PostProcessVolume standart;
    public PostProcessVolume rage;
    public PostProcessVolume slowMo;

    [Header("Reborn")]
    public GameObject angel;
    public ParticleSystem angelParticles;
    public EventReference auraClip;

    [Header("Skills")]
    public bool isRage;
    public bool isSlowMo;
    public float rageTime;
    public float slowMoTime;

    [Header("Game info")]
    public bool isCurrentBattle;
    public bool isGameStarted = false;
    public int IsFirstTime { get; set; }
    

    public static GameManager instance;

    public delegate void GameHandler();
    public event GameHandler OnGameOver; 
    public event GameHandler OnRestart; 

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q))
            soundManager.SwitchLowPassFrequency();

        if (Input.GetKeyDown(KeyCode.X)) {
            PlayerPrefs.DeleteAll();
        }
#endif
    }

    private void Start() {
        ScanAstar();
        CheckIsPlayingFirstTime();
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
    }

    public void ShakeOnce(Vector4 force)  => cameraShaker.ShakeOnce(force.x, force.y, force.z, force.w);
    public void ShakeScreen (float force) => cameraShaker.ShakeOnce(force, 4f, 0.1f, 0.3F);
    
    #region A*

    private IEnumerator ScanAstarDelay() {
        yield return new WaitForEndOfFrame();
        aStarManager.AStar.Scan();
    }

    public void ScanAstar() {
        aStarManager.AStar.Scan();
    }

    #endregion

    #region Compute functions

    public int Chance(int a, int b) {
        int chance = Random.Range(a, b);
        return chance;
    }
    public float RandomFloat(float a, float b) {
        var randNumb = Random.Range(a, b);
        return randNumb;
    }

    public Vector3 RandomVector(int a, int b) {
        a = Random.Range(a, b);
        b = Random.Range(a, b);
        Vector3 vector = new Vector2(a, b);
        return vector;
    }

    #endregion


    #region Post proces & Modes

    public IEnumerator InterpolatePostProcess(PostProcessVolume a, PostProcessVolume b) {
        StartCoroutine(InterpolateUp(b));
        StartCoroutine(InterpolateDown(a));
        yield return null;
    }

    private IEnumerator InterpolateUp(PostProcessVolume volume)
    {
        while (volume.weight < 1)
        {
            volume.weight += 1 * Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator InterpolateDown(PostProcessVolume volume)
    {
        while (volume.weight > 0)
        {
            volume.weight -= 1 * Time.deltaTime;
            yield return null;
        }
    }

    public void EnterSlowMo()
    {
        if (isSlowMo || isRage)
            return;

        StartCoroutine(InterpolatePostProcess(standart, slowMo));
        StartCoroutine(stopTime());
        isSlowMo = true;
        StartCoroutine(ExitSlowMo(slowMoTime));
    }

    public IEnumerator ExitSlowMo(float delay)
    {
        yield return new WaitForSeconds(delay);
        isSlowMo = false;
        StartCoroutine(InterpolatePostProcess(slowMo, standart));
        StartCoroutine(resetTime());
    }

    private IEnumerator stopTime()
    {
        while(Time.timeScale > 0.6)
        {
            Time.timeScale -= 1 * Time.deltaTime;
            yield return null;
        }
    }
    
    private IEnumerator resetTime()
    {
        while(Time.timeScale < 1)
        {
            Time.timeScale += 1 * Time.deltaTime;
            yield return null;
        }
    }

    public void EnterRageMode()
    {
        if (isRage || isSlowMo)
            return;

        StartCoroutine(InterpolatePostProcess(standart, rage));
        player.EnterRageMode();
        glitchEffect.enabled = true;
        // SoundManager._glitchSource.Play();
        isRage = true;

        StartCoroutine(ExitRageMode(rageTime));
    }

    public IEnumerator ExitRageMode(float delay)
    {
        yield return new WaitForSeconds(1f);
        glitchEffect.enabled = false;

        yield return new WaitForSeconds(delay);
        isRage = false;
        player.ExitRageMode();
        // soundManager._glitchSource.Stop();
        StartCoroutine(InterpolatePostProcess(rage, standart));
    }

    #endregion

    #region Game manage

    private void CheckIsPlayingFirstTime() {
        IsFirstTime = PlayerPrefs.HasKey("Is playing first time") ? PlayerPrefs.GetInt("Is playing first time") : 1;
        if (IsFirstTime == 0) return;
        tutorialManager.StartTutorial();
        DialogueManager.instance.StartIntroDialogue();
        PlayerController.instance.shootingScript.DestroyWeapon();
        PlayerController.instance.transform.position = new Vector3(-100, 50);
        IsFirstTime = 0;
        PlayerPrefs.SetInt("Is playing first time", IsFirstTime);
        
    }

    public void Restart(Transform restartPosition)
    {
        PlayerController.instance.Restart(restartPosition);
        lvlManager.ResetLvlManager();
        StartCoroutine(ScanAstarDelay());
        isGameStarted = false;
        isCurrentBattle = false;
        UiManager.HideGameOverCanavas();
        scoreManager.ResetScore();
        OnRestart?.Invoke();
    }

    public void SetPlayerPosition (Transform spawnPoint) => player.transform.position = spawnPoint.position;

    public void GameOver()
    {
        scoreManager.SummarizeScore();
        soundManager.SwitchLowPassFrequency();
        UiManager.ShowGameOverCanavas();
        UiManager.playerLevelBarGameOver.UpdateBar();
        statsManager.UpdateStats();
        OnGameOver?.Invoke();
    }

    public void Reborn()
    {
        StartCoroutine(AngelSpawn());
        Instantiate(angelParticles, UiManager.mainUiCanvas.transform.position,
            Quaternion.identity);
        lvlManager.ClearLevel();
        SoundManager.PlayOneShot(auraClip);
    }

    private IEnumerator AngelSpawn()
    {
        yield return new WaitForSeconds(0.3f);
        GameObject angelInst = Instantiate(angel, UiManager.mainUiCanvas.transform.position,
            Quaternion.identity, UiManager.mainUiCanvas.transform);

        yield return new WaitForSeconds(5f);
        Destroy(angelInst);
    }

    #endregion
}
