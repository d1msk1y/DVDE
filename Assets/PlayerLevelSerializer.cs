using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelSerializer : MonoBehaviour
{
	[Header("Bar components")]
	[SerializeField] private Image _fillBar;
	[Space(10)]
	[SerializeField] private Text _levelText;
	[SerializeField] private Text _neededScoreText;
	[SerializeField] private Text _totalScoreText;

	private void Start()
	{
		GameManager.instance.OnRestart += UpdateBar;
		UpdateBar();
	}

	private void UpdateBar()
	{
		SetFillAmount();
		SetTexts();
	}
	
	private void SetTexts()
	{
		_neededScoreText.text = GameManager.instance.scoreManager.neededScore + " XP";
		_totalScoreText.text = GameManager.instance.scoreManager.totalScore + " XP";
		_levelText.text = "LVL " + GameManager.instance.scoreManager.CurrentLevel;
	}
	
	private void SetFillAmount()
	{
		var neededScore = (float)GameManager.instance.scoreManager.neededScore - GameManager.instance.scoreManager.lastNeededScore;
		var currentScore = (float)GameManager.instance.scoreManager.totalScore - GameManager.instance.scoreManager.lastNeededScore;
		_fillBar.fillAmount = currentScore/neededScore;
	}
}