using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [field: Header("Player Level")]
    public int CurrentLevel {
        get => _currentLevel;
        set {
            if (value - _currentLevel > 0) onLevelUp?.Invoke();
            _currentLevel = value;
            PlayerPrefs.SetInt(GameManager.instance.statsManager.keys[8], value); //Set level index
           
        }
    }

    public int initialLevel;//Level on game start
    private int _currentLevel;

    [Header("Score")]
    [SerializeField] private float _scoreMultiplier;
    public int receivedScore;
    public int maxReceivedScore;
    public int totalScore;
    public int lastNeededScore;
    public float neededScore;
    public int lastScore;

    [Header("Combo")]
    public int maxScoreMultiplier;
    public float scoreMultiplierLifetime;
    public float startShiverOn;
    public float doubleKillLifeTime;

    public int killedDoubles;

    public GameObject comboText;
    public GameObject doubleKillText;
    private int _currentScoreMultiplier = 1;
    private float _timer;
    private Text _scoreMultiplierTxt;

    [SerializeField] private ParticleSystem scoreMultiplierParticles;
    [SerializeField] private ParticleSystem doubleKillParticles;

    private Vector3 _randomPos;

    public delegate void ScoreHandler();
    public event ScoreHandler onLevelUp;

    [Header("Coins")]
    public int receivedCoins;
    [SerializeField] private int _totalCoins;
    public event ScoreHandler onCoinsChange;
    public int TotalCoins {
        get => _totalCoins;
        set {
            _totalCoins = value;
            onCoinsChange?.Invoke();
        }
    }
    public int ReceivedScore {
        get => receivedScore;

        private set {
            receivedScore = (int)(value*_scoreMultiplier);
            UiManager.instance.SetPtsCount(receivedScore);
        }
    }

    [Header("Player stats")]
    public int enemiesKilled;

    private void Awake()
    {
        GameManager.instance.OnRestart += SetInitialLevel;
        GameManager.instance.OnGameOver += ResetScoreModifier;
        GetPlayerKeys();
        GameManager.instance.OnRestart += GetPlayerKeys;
        SetInitialLevel();
    }
    private void GetPlayerKeys()
    {

        if (PlayerPrefs.HasKey(GameManager.instance.statsManager.keys[6]))
            maxReceivedScore = PlayerPrefs.GetInt(GameManager.instance.statsManager.keys[6]);

        if (PlayerPrefs.HasKey("Total score"))
            totalScore = PlayerPrefs.GetInt("Total score");

        if (PlayerPrefs.HasKey("Needed score"))
            neededScore = PlayerPrefs.GetInt("Needed score");

        if (PlayerPrefs.HasKey("Prev needed score"))
            lastNeededScore = PlayerPrefs.GetInt("Prev needed score");

        if (PlayerPrefs.HasKey("Total coins")) {
            TotalCoins = PlayerPrefs.GetInt("Total coins");
            GameManager.instance.UiManager.UpdateCostTxts();
        }
        if (PlayerPrefs.HasKey("Last total score"))
            lastScore = PlayerPrefs.GetInt("Last total score");
        if (PlayerPrefs.HasKey(GameManager.instance.statsManager.keys[8])) {
            CurrentLevel = PlayerPrefs.GetInt(GameManager.instance.statsManager.keys[8]); //Get level index
        } else CurrentLevel = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            AddComboPoint(1);
    }

    private void SetInitialLevel() => initialLevel = CurrentLevel;

    public void AddScore(int score)
    {
        ReceivedScore += score * _currentScoreMultiplier;
    }

    #region Combo

    public void AddComboPoint(int value)
    {
        if (_currentScoreMultiplier < maxScoreMultiplier)
            _currentScoreMultiplier += value;

        if (_randomPos == Vector3.zero)
            _randomPos = new Vector3(Random.Range(0, 10), Random.Range(0, 5));

        if (_scoreMultiplierTxt != null)
            Destroy(_scoreMultiplierTxt.transform.parent.gameObject);

        PopUpTXT(GameManager.instance.UiManager.mainUiCanvas.transform.position + _randomPos);

        StartCoroutine(ComboTimer());

    }

    private IEnumerator ComboTimer()
    {
        float timer = scoreMultiplierLifetime;
        Text txtToDestroy = _scoreMultiplierTxt;

        bool isShaved = false;

        while (timer > 0)
        {
            timer -= 0.1f;

            if (timer <= 0 && txtToDestroy != null)
                ResetScoreModifier();
            if (timer < startShiverOn && txtToDestroy != null)
            {
                if (isShaved != true)
                {
                    _scoreMultiplierTxt.GetComponent<Animator>().Play("ScoreModifierToShiver");
                    isShaved = true;
                }
            }

            yield return null;
        }
    }

    private void PopUpTXT(Vector3 popPos)
    {
        if (_scoreMultiplierTxt != null)
            Destroy(_scoreMultiplierTxt.transform.parent.gameObject);
        _scoreMultiplierTxt = Instantiate(comboText, popPos, Quaternion.identity, GameManager.instance.UiManager.hud.transform.parent).GetComponentInChildren<Text>();

        if (_currentScoreMultiplier > 2)
        {
            StartCoroutine(InstantiateParticles(scoreMultiplierParticles, _scoreMultiplierTxt.transform, 0));
            _scoreMultiplierTxt.GetComponent<Animator>().Play("ScoreModifierBump");
        }
        else if (_currentScoreMultiplier <= 2)
        {
            StartCoroutine(InstantiateParticles(scoreMultiplierParticles, _scoreMultiplierTxt.transform, 0.1f));
            _scoreMultiplierTxt.GetComponent<Animator>().Play("ScoreModifierPopup");
        }

        _scoreMultiplierTxt.text = "X" + _currentScoreMultiplier;
    }
    private void ResetScoreModifier() {
        GameObject todestroy = _scoreMultiplierTxt.transform.parent.gameObject;
        _currentScoreMultiplier = 1;
        _randomPos = Vector3.zero;
        if (todestroy != null)
            todestroy.GetComponentInChildren<Animator>().Play("ScoreModifierPopdown"); Destroy(todestroy, 0.5f);
    }

    private IEnumerator InstantiateParticles(ParticleSystem instance, Transform parent, float delay)
    {
        yield return new WaitForSeconds(delay);
        if(parent != null)Instantiate(instance, parent.position, Quaternion.identity, parent);
    }

    public void CheckDoubleKill()
    {
        if (killedDoubles < 1)
            StartCoroutine(resetDoubleKill());

        killedDoubles += 1;
        GameObject doubleKillTextInst = null;

        if (killedDoubles >= 2)
        {
            doubleKillTextInst = Instantiate(doubleKillText, GameManager.instance.UiManager.mainUiCanvas.transform.position + new Vector3(Random.Range(0, 10), Random.Range(0, 5)),
                Quaternion.identity, GameManager.instance.UiManager.mainUiCanvas.transform);

            //CheckSkill();
        }
        if (killedDoubles == 2)
        {
            doubleKillTextInst.GetComponentInChildren<Text>().text = "DOUBLE KILL!";
        }
        if (killedDoubles == 3)
        {
            doubleKillTextInst.GetComponentInChildren<Text>().text = "TRIPPLE KILL!";
        }
        if (killedDoubles == 4)
        {
            doubleKillTextInst.GetComponentInChildren<Text>().text = "QUADRO KILL!";
        }

        _timer = doubleKillLifeTime;
    }

    #endregion

    private void CheckSkill()
    {
        int chance = Random.Range(0, 5);
        if (chance == 1)
        {
            ChooseSkill();
        }
    }

    private void ChooseSkill()
    {
        int chance = Random.Range(0, 2);
        Debug.Log(chance);
        if (chance == 1)
        {
            GameManager.instance.EnterSlowMo();
        }
        if (chance == 0)
        {
            GameManager.instance.EnterRageMode();
        }
    }

    public void SummarizeScore()
    {
        

        if (ReceivedScore > maxReceivedScore)
            maxReceivedScore = ReceivedScore;

        if (ReceivedScore > 0)
            lastScore = totalScore;

        totalScore += ReceivedScore;
        GameManager.instance.statsManager.totalEnemiesKilled += enemiesKilled;

        GameManager.instance.UiManager.coinsReceivedTxt.text = "COINS RECEIVED: 0";
        receivedCoins = (ReceivedScore / 10) * 3;
        GameManager.instance.statsManager.earnedCoins += receivedCoins;
        StartCoroutine(countCoins());

        TotalCoins += receivedCoins;

        UpdateGameOverTxt();
        SaveScoreData();

        GameManager.instance.UiManager.UpdateCostTxts();
    }

    private IEnumerator countCoins()
    {
        int coinsCounted = 0;
        float coinsCountedTrans = 0;
        while (coinsCounted < receivedCoins)
        {
            coinsCountedTrans += 750f * Time.deltaTime;
            coinsCounted = (int)coinsCountedTrans;

            GameManager.instance.UiManager.coinsReceivedTxt.text = "COINS RECEIVED: " + coinsCounted.ToString();
            yield return null;
        }
    }

    private IEnumerator resetDoubleKill()
    {
        _timer = doubleKillLifeTime;
        while (_timer > 0)
        {
            _timer -= 1 * Time.deltaTime;
            if (_timer <= 0)
                killedDoubles = 0;

            yield return null;
        }
    }

    private void SaveScoreData()
    {
        PlayerPrefs.SetInt(GameManager.instance.statsManager.keys[0], GameManager.instance.statsManager.totalEnemiesKilled);
        PlayerPrefs.SetInt(GameManager.instance.statsManager.keys[1], GameManager.instance.statsManager.earnedCoins);
        PlayerPrefs.SetInt(GameManager.instance.statsManager.keys[6], maxReceivedScore);
        PlayerPrefs.SetInt(GameManager.instance.statsManager.keys[7], totalScore);
        PlayerPrefs.SetInt("Total coins", TotalCoins);
        PlayerPrefs.SetInt("Last total score", lastScore);
    }

    private void UpdateGameOverTxt()
    {
        GameManager.instance.UiManager.receivedScoreGameOverTxt.text = "SCORE: " + ReceivedScore;
        GameManager.instance.UiManager.maxReceivedScoreGameOverTxt.text = "HIGHSCORE: " + maxReceivedScore;
        GameManager.instance.UiManager.totalScoreGameOverTxt.text = "TOTAL SCORE: " + totalScore;
        GameManager.instance.UiManager.enemiesKilledGameOverTxt.text = "ENEMIES KILLED: " + enemiesKilled;
    }

    public void ResetScore()
    {
        enemiesKilled = 0;
        ReceivedScore = 0;
        receivedCoins = 0;
    }
}
