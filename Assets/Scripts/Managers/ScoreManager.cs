using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [Header("Score")]
    public int receivedScore;
    public int maxReceivedScore;
    public int totalScore;
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

    [Header("Coins")]
    public int receivedCoins;
    public int totalCoins;

    [Header("Player stats")]
    public int enemiesKilled;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(GameManager.instance.statsManager.keys[6]))
        {
            maxReceivedScore = PlayerPrefs.GetInt(GameManager.instance.statsManager.keys[6]);
        }
        if (PlayerPrefs.HasKey("Total score"))
        {
            totalScore = PlayerPrefs.GetInt("Total score");
        }
        if (PlayerPrefs.HasKey("Total coins"))
        {
            totalCoins = PlayerPrefs.GetInt("Total coins");
        }
        if (PlayerPrefs.HasKey("Last total score"))
        {
            lastScore = PlayerPrefs.GetInt("Last total score");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            AddComboPoint(1);
    }

    public void AddScore(int score)
    {
        receivedScore += score * _currentScoreMultiplier;
    }

    #region Combo

    public void AddComboPoint(int value)
    {
        if(_currentScoreMultiplier < maxScoreMultiplier)
            _currentScoreMultiplier += value;

        if(_randomPos == Vector3.zero)
            _randomPos = new Vector3(Random.Range(0, 10), Random.Range(0, 5));

        if(_scoreMultiplierTxt != null)
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
                ResetScoreModifier(_scoreMultiplierTxt.transform.parent.gameObject);
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
        if(_scoreMultiplierTxt != null)
            Destroy(_scoreMultiplierTxt.transform.parent.gameObject);
        _scoreMultiplierTxt = Instantiate(comboText, popPos, Quaternion.identity, GameManager.instance.UiManager.hud.transform.parent).GetComponentInChildren<Text>();

        if(_currentScoreMultiplier > 2)
        {
            StartCoroutine(InstantiateParticles(scoreMultiplierParticles, _scoreMultiplierTxt.transform, 0));
            _scoreMultiplierTxt.GetComponent<Animator>().Play("ScoreModifierBump");
        }
        else if(_currentScoreMultiplier <= 2)
        {
            StartCoroutine(InstantiateParticles(scoreMultiplierParticles, _scoreMultiplierTxt.transform, 0.1f));
            _scoreMultiplierTxt.GetComponent<Animator>().Play("ScoreModifierPopup");
        }

        _scoreMultiplierTxt.text = "X" + _currentScoreMultiplier;
    }
    private void ResetScoreModifier(GameObject todestroy)
    {
        _currentScoreMultiplier = 1;
        _randomPos = Vector3.zero;
        if(todestroy != null)
            todestroy.GetComponentInChildren<Animator>().Play("ScoreModifierPopdown"); Destroy(todestroy, 0.5f);
    }

    private IEnumerator InstantiateParticles(ParticleSystem instance, Transform parent, float delay)
    {
        yield return new WaitForSeconds(delay);
        Instantiate(instance, parent.position, Quaternion.identity, parent);
    }

    public void CheckDoubleKill()
    {
        if(killedDoubles < 1)
            StartCoroutine(resetDoubleKill());

        killedDoubles += 1;
        GameObject doubleKillTextInst = null;

        if(killedDoubles >= 2)
        {
            doubleKillTextInst = Instantiate(doubleKillText, GameManager.instance.UiManager.mainUiCanvas.transform.position + new Vector3(Random.Range(0, 10), Random.Range(0, 5)),
                Quaternion.identity, GameManager.instance.UiManager.mainUiCanvas.transform);

            CheckSkill();
        }
        if(killedDoubles == 2)
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

        if (receivedScore > maxReceivedScore)
            maxReceivedScore = receivedScore;

        if (receivedScore > 0)
            lastScore = totalScore;

        totalScore += receivedScore;
        GameManager.instance.statsManager.totalEnemiesKilled += enemiesKilled;

        GameManager.instance.UiManager.coinsReceivedTxt.text = "COINS RECEIVED: 0";
        receivedCoins = receivedScore / 10;
        GameManager.instance.statsManager.earnedCoins += receivedCoins;
        StartCoroutine(countCoins());

        totalCoins += receivedCoins;

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
            coinsCountedTrans += 75f * Time.deltaTime;
            coinsCounted = (int)coinsCountedTrans;

            GameManager.instance.UiManager.coinsReceivedTxt.text = "COINS RECEIVED: " + coinsCounted.ToString();
            yield return null;
        }
    }

    private IEnumerator resetDoubleKill()
    {
        _timer = doubleKillLifeTime;
        while(_timer > 0)
        {
            _timer -= 1 * Time.deltaTime;
            if(_timer <= 0)
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
        PlayerPrefs.SetInt("Total coins", totalCoins);
        PlayerPrefs.SetInt("Last total score", lastScore);
    }

    private void UpdateGameOverTxt()
    {
        GameManager.instance.UiManager.receivedScoreGameOverTxt.text = "SCORE: " + receivedScore;
        GameManager.instance.UiManager.maxReceivedScoreGameOverTxt.text = "HIGHSCORE: " + maxReceivedScore;
        GameManager.instance.UiManager.totalScoreGameOverTxt.text = "TOTAL SCORE: " + totalScore;
        GameManager.instance.UiManager.enemiesKilledGameOverTxt.text = "ENEMIES KILLED: " + enemiesKilled;
    }

    public void ResetScore()
    {
        enemiesKilled = 0;
        receivedScore = 0;
        receivedCoins = 0;
    }
}
