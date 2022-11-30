using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    [Header("Loading screen")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Text tipTxt;
    [SerializeField] private string[] tips;
    
    public void StartGame() {
        SendTip();
        loadingScreen.SetActive(true);
        StartCoroutine(AsyncLoad());
    }
    
    private void SendTip() => tipTxt.text = tips[Random.Range(0, tips.Length)];

    private static IEnumerator AsyncLoad() {
        yield return new WaitForSeconds(0.5f);
        var asyncLoad = SceneManager.LoadSceneAsync(1);
        while (!asyncLoad.isDone) {
            yield return null;
        }
    }

}
