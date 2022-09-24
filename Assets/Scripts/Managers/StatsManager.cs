using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{
    [Header("UI")]
    public Text totalEnemiesKilledTxt;
    public Text earnedCoinsTxt;
    public Text spentCoinsTxt;
    public Text givenDamageTxt;
    public Text receivedDamageTxt;
    public Text passedRoomsTxt;
    public Text highScoreTxt;
    public Text totalScoreTxt;
    public Text playerLevelTxt;

    [Header("Values")]
    public int totalEnemiesKilled;
    public int earnedCoins;
    public int spentCoins;
    public int givenDamage;
    public int receivedDamage;
    public int passedRooms;
    public int highScore;
    public int totalScore;
    public int playerLevel;

    [Header("Player prefs")]
    public string[] keys;

    private void Start()
    {
        UpdateStats();
        GameManager.instance.scoreManager.onLevelUp += UpdateStats;
    }
    public void UpdateStats()
    {
        SetAllStats();
        SetTexts();
    }
    
    private void SetAllStats()
    {
        if (PlayerPrefs.HasKey(keys[0])) totalEnemiesKilled = PlayerPrefs.GetInt(keys[0]);
        if (PlayerPrefs.HasKey(keys[1])) earnedCoins = PlayerPrefs.GetInt(keys[1]);
        if (PlayerPrefs.HasKey(keys[2])) spentCoins = PlayerPrefs.GetInt(keys[2]);
        if (PlayerPrefs.HasKey(keys[3])) givenDamage = PlayerPrefs.GetInt(keys[3]);
        if (PlayerPrefs.HasKey(keys[4])) receivedDamage = PlayerPrefs.GetInt(keys[4]);
        if (PlayerPrefs.HasKey(keys[5])) passedRooms = PlayerPrefs.GetInt(keys[5]);
        if (PlayerPrefs.HasKey(keys[6])) highScore = PlayerPrefs.GetInt(keys[6]);
        if (PlayerPrefs.HasKey(keys[7])) totalScore = PlayerPrefs.GetInt(keys[7]);
        if (PlayerPrefs.HasKey(keys[8])) playerLevel = PlayerPrefs.GetInt(keys[8]);
    }
    private void SetTexts()
    {
        if (totalEnemiesKilledTxt != null) totalEnemiesKilledTxt.text = "Total enemies killed: " + totalEnemiesKilled;
        if (earnedCoinsTxt != null) earnedCoinsTxt.text = "Earned coins: " + earnedCoins;
        if (spentCoinsTxt != null) spentCoinsTxt.text = "Spent coins: " + spentCoins;
        if (givenDamageTxt != null) givenDamageTxt.text = "Given damage: " + givenDamage;
        if (receivedDamageTxt != null) receivedDamageTxt.text = "Received damage: " + receivedDamage;
        if (passedRoomsTxt != null) passedRoomsTxt.text = "Passed rooms: " + passedRooms;
        if (highScoreTxt != null) highScoreTxt.text = "High score: " + highScore;
        if (totalScoreTxt != null) totalScoreTxt.text = "Total score: " + totalScore;
        if (playerLevelTxt != null) playerLevelTxt.text = "Player LVL: " + playerLevel;
    }
}