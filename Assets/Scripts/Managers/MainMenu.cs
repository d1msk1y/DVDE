using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MainMenu : MonoBehaviour
{

    [Header("Navigation")]
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _newGameButton;
    
    [Header("Loading screen")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Text tipTxt;
    [SerializeField] private string[] tips;

    private void Start()
    {
        ValidateNavigation();
    }

    private void ValidateNavigation()
    {
        if (IsGameFromScratch())
        {
            _continueButton.interactable = false;
        }
    }

    private static bool IsGameFromScratch()
    {
        return !PlayerPrefs.HasKey("Is playing first time");
    }

    public void StartGame() {
        ShowTip();
        loadingScreen.SetActive(true);
        StartCoroutine(AsyncLoad());
    }

    public void StartNewGame()
    {
        PlayerPrefs.DeleteAll();
        StartGame();
    }
    
    private void ShowTip() => tipTxt.text = tips[Random.Range(0, tips.Length)];

    private static IEnumerator AsyncLoad() {
        yield return new WaitForSeconds(0.5f);
        var asyncLoad = SceneManager.LoadSceneAsync(1);
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }

}
