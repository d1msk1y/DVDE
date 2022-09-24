using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [Header("Main")]
    public Image backGround;
    public GameObject gameOverCanvas;

    [Header("HUD")]
    public Canvas mainUiCanvas;
    public GameObject hud;
    [SerializeField] private Text _ammoTxt;
    [SerializeField] private Text _gunNameTxt;

    [Header("Controlls")]
    public Joystick movementJoystick;
    public Joystick shootingJoystick;
    public Button fireButton;
    public Button pickUpButton;
    public PickupAble toPickup;
    public Button specialAttackButton;

    [Header("Score UI game over")]
    public Text receivedScoreGameOverTxt;
    public Text maxReceivedScoreGameOverTxt;
    public Text totalScoreGameOverTxt;
    public Text enemiesKilledGameOverTxt;
    public Text coinsReceivedTxt;
    public PlayerLevelBar playerLevelBarGameOver;

    [Header("Shop UI")]
    public Text[] coinsTXTs;

    private void Start()
    {
        UpdateCostTxts();
    }

    #region Main

    private void ShowBG()
    {
        backGround.GetComponent<Animator>().Play("BG on");
    }

    private void HideBG()
    {
        backGround.GetComponent<Animator>().Play("BG off");
    }

    public void ShowGameOverCanavas()
    {
        gameOverCanvas.SetActive(true);
        playerLevelBarGameOver.UpdateBar();
        HideHUD();
        ShowBG();
    }
    public void HideGameOverCanavas()
    {
        gameOverCanvas.SetActive(false);
        HideBG();
    }

    #endregion

    #region HUD
    public void HideHUD()
    {
        hud.SetActive(false);
    }
    public void ShowHUD()
    {
        hud.SetActive(true);
    }
    #endregion

    public void UpdateCostTxts()
    {
        foreach (Text text in coinsTXTs)
        {
            text.text = "$" + GameManager.instance.scoreManager.TotalCoins;
        }
    }

    #region Gun relative
    public void SetAmmoStats(int currentAmmo, int maxAmmo)
    {
        _ammoTxt.text = "AMMO: " + currentAmmo + "/" + maxAmmo;
    }

    public void SetGunName(string name)
    {
        _gunNameTxt.text = name;
    }
    #endregion

    #region Controlls

    public void PickUpButton()
    {
        toPickup.PickUp();
    }

    #endregion

    #region Score relative

    public void UpdateScoreStats()
    {
        receivedScoreGameOverTxt.text = "Score: " + GameManager.instance.scoreManager.receivedScore;
        maxReceivedScoreGameOverTxt.text = "HighScore " + GameManager.instance.scoreManager.maxReceivedScore;
    }

    #endregion
}
