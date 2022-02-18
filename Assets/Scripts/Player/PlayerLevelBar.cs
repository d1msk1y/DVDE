using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelBar : MonoBehaviour
{
    [Header("Lvl bar properties")]
    [SerializeField] private float barSpeedMultiplier;
    [SerializeField] private float minBarSpeed;
    [SerializeField] private float particleEmisionMultiplier;
    [SerializeField] private float maxParticleEmision;
    [SerializeField] private float minParticleEmision;

    [Header("Lvl bar info")]
    [SerializeField] private int levelIndex;
    [SerializeField] private int neededScore;

    [Header("UI/VFX relative")]
    [SerializeField] private Text currentScoreTxt;
    [SerializeField] private Text neededScoreTxt;
    [SerializeField] private Text levelIndexTxt;
    [SerializeField] private Image bar;

    [SerializeField] private ParticleSystem particles;

    private float neededBarAmount;
    private float currentScoreTrans;
    private float prevScore;
    private float lastScore;
    private float barSpeed;
    [SerializeField]private int prevNeededScore;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Needed score"))
        {
            neededScore = PlayerPrefs.GetInt("Needed score");
        }
        if (PlayerPrefs.HasKey("Last total score"))
        {
            prevScore = PlayerPrefs.GetInt("Last total score");
        }
        if (PlayerPrefs.HasKey("Total score"))
        {
            lastScore = PlayerPrefs.GetInt("Total score");
        }
        if (PlayerPrefs.HasKey(GameManager.instance.statsManager.keys[8]))
        {
            levelIndex = PlayerPrefs.GetInt("Player lvl index");
            Debug.Log(levelIndex);
        }
        if (PlayerPrefs.HasKey("Prev needed score"))
        {
            prevNeededScore = PlayerPrefs.GetInt("Prev needed score");
        }
        currentScoreTrans = prevScore;

        var emission = particles.emission;
        emission.rateOverTime = 1000;

        StartCoroutine(UpdateBarScore());
        StartCoroutine(UpdateBarAmount());
        levelIndexTxt.text = "LVL " + levelIndex;
    }

    public void UpdateBar()
    {
        if (GameManager.instance.scoreManager.totalScore == lastScore)
            return;

        float particleEmision = GameManager.instance.scoreManager.receivedScore * particleEmisionMultiplier;
        if (particleEmision > maxParticleEmision)
            particleEmision = maxParticleEmision;
        else if (particleEmision < minParticleEmision)
            particleEmision = minParticleEmision;

        var emission = particles.emission;

        emission.rateOverTime = particleEmision;

        StartCoroutine(UpdateBarScore());
        StartCoroutine(UpdateBarAmount());
    }

    private IEnumerator UpdateBarAmount()
    {
        particles.gameObject.SetActive(true);
        particles.enableEmission = true;

        while (currentScoreTrans < GameManager.instance.scoreManager.totalScore)
        {
            float scoreDiff = currentScoreTrans - prevNeededScore;
            float neededScoreDiff = neededScore - prevNeededScore;
            neededBarAmount =  scoreDiff / neededScoreDiff;
            particles.GetComponent<RectTransform>().localPosition = new Vector3(bar.fillAmount * 100, 0, 0);

            bar.fillAmount = neededBarAmount;
            yield return null;
        }
    }

    private IEnumerator UpdateBarScore()
    {
        neededScoreTxt.text = neededScore.ToString();
        while (currentScoreTrans < GameManager.instance.scoreManager.totalScore)
        {
            float barSpeedTrans = GameManager.instance.scoreManager.totalScore - prevScore;
            if (barSpeedTrans < minBarSpeed)
                barSpeedTrans = minBarSpeed;
            currentScoreTrans += 1 * barSpeedTrans * barSpeedMultiplier * Time.deltaTime;
            currentScoreTxt.text = Mathf.Round(currentScoreTrans).ToString();
            if (currentScoreTrans >= neededScore)
            {
                NextLevel();
            }
            yield return null;
        }
        if(currentScoreTrans > GameManager.instance.scoreManager.totalScore)
        {
            currentScoreTrans = GameManager.instance.scoreManager.totalScore;
            particles.enableEmission = false;
            currentScoreTxt.text = Mathf.Round(currentScoreTrans).ToString();
        }
    }

    public void NextLevel()
    {
        levelIndex += 1;
        levelIndexTxt.GetComponentInParent<Animator>().Play("Pop");

        float neededScoreTransition = neededScore * 1.25f;
        neededScoreTransition = Mathf.Round(neededScoreTransition / 10) * 10;
        prevNeededScore = neededScore;
        PlayerPrefs.SetInt("Prev needed score", prevNeededScore);
        neededScore = (int)neededScoreTransition;

        PlayerPrefs.SetInt("Needed score", neededScore);
        PlayerPrefs.SetInt(GameManager.instance.statsManager.keys[8], levelIndex);
        neededScoreTxt.text = neededScore.ToString();

        levelIndexTxt.text = "LVL " + levelIndex;

        if (currentScoreTrans > neededScore)
        {
            UpdateBar();
        }
        Debug.Log("Lvl up!");
    }

}