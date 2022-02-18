using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using EZCameraShake;

public class GameManager : MonoBehaviour
{
    [Header("Controller n` Managers")]
    public AStarManager aStarManager;
    public LvlManager lvlManager;
    public SoundManager soundManager;
    public CameraMotor cameraMotor;
    public UiManager UiManager;
    public ScoreManager scoreManager;
    public ItemsManager itemsManager;
    public DataManager dataManager;
    public StatsManager statsManager;

    public GlitchEffect glitchEffect;

    [Header("Player relative")]
    public PlayerController player;
    public PostProcessVolume standart;
    public PostProcessVolume rage;
    public PostProcessVolume slowMo;

    [Header("Reborn")]
    public GameObject angel;
    public ParticleSystem angelParticles;
    public AudioClip auraClip;

    [Header("Skills")]
    public bool isRage;
    public bool isSlowMo;
    public float rageTime;
    public float slowMoTime;

    [Header("Game info")]
    public bool isCurrentBattle;
    public bool isGameStarted = false;

    [Header("Controlls")]
    public float movementSensivity;
    public float aimingSensivity;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            soundManager.LowPassFrequencyCutOff();

        if (Input.GetKeyDown(KeyCode.X))
        {
            PlayerPrefs.DeleteAll();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            StartCoroutine(InterpolatePostProcess(standart, rage));
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(InterpolatePostProcess(rage, standart));
        }

    }

    private void Start()
    {
        ScanAstar();
    }

    #region A*

    private IEnumerator ScanAstarDelay()
    {
        yield return new WaitForEndOfFrame();
        aStarManager.AStar.Scan();
    }

    public void ScanAstar()
    {
        aStarManager.AStar.Scan();
    }

    #endregion

    #region Compute functions

    public int Chance(int a, int b)
    {
        int chance = Random.Range(a, b);
        return chance;
    }

    public Vector3 RandomVector(int a, int b)
    {
        a = Random.Range(a, b);
        b = Random.Range(a, b);
        Vector3 vector = new Vector2(a, b);
        return vector;
    }

    #endregion

    public void ShakeOnce()
    {
        CameraShaker.Instance.ShakeOnce(0.5f, 10f, .01f, .05f);
    }

    #region Post proces & Modes

    public IEnumerator InterpolatePostProcess(PostProcessVolume a, PostProcessVolume b)
    {
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
        soundManager._glitchSource.Play();
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
        soundManager._glitchSource.Stop();
        StartCoroutine(InterpolatePostProcess(rage, standart));
    }

    #endregion

    #region Game manage

    public void Restart(Transform restartPosition)
    {
        PlayerController.instance.Restart(restartPosition);
        lvlManager.ResetLvlManager();
        StartCoroutine(ScanAstarDelay());
        isGameStarted = false;
        isCurrentBattle = false;
        UiManager.HideGameOverCanavas();
        scoreManager.ResetScore();
        soundManager.PlayMenuOST();
    }

    public void GameOver()
    {
        scoreManager.SummarizeScore();
        UiManager.ShowGameOverCanavas();
        //soundManager.LowPassFrequencyCutOff();
        UiManager.playerLevelBarGameOver.UpdateBar();
        StartCoroutine(soundManager.SmoothOffAudio(soundManager._soundtrackAudioSource));

        statsManager.UpdateStats();
    }

    public void Reborn()
    {
        StartCoroutine(AngelSpawn());
        Instantiate(angelParticles, UiManager.mainUiCanvas.transform.position,
            Quaternion.identity);
        lvlManager.ClearLevel();
        soundManager._vfxAudioSource.PlayOneShot(auraClip);
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
