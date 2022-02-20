using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UpgradeType
{
    Skill,
    Specs
}

public class HeroUpgrade : PickupAble
{
    [Header("Upgrade props")]
    [SerializeField] private int _maxStage;// Max accessible upgrade level.
    [SerializeField] private float _priceModifier;// By this value cost will multiply each time when player buys a new level of upgrade.
    [SerializeField] private float[] _upgradeStages;

    [SerializeField] private SpriteRenderer[] _points;
    [SerializeField] private Sprite _square;

    public UpgradeType upgradeType;

    public int upgradePrefsIndex;
    public string stagePrefsKey;
    public string pricePrefsKey;

    [Header("Tip")]
    [Multiline]
    public string speech;
    public SpeechNPC speechNPC;

    private int _currentStage;// Current upgrade level.
    private bool _isSpeechBaloonActive;

    private void Awake()
    {
        if(PlayerPrefs.HasKey(pricePrefsKey))
            price = PlayerPrefs.GetInt(pricePrefsKey);
    }

    public new void Start()
    {
        base.Start();

        itemCanvas.GetComponentInChildren<Text>().text = price + "$";

        if (PlayerPrefs.HasKey(stagePrefsKey))
        {
            _currentStage = PlayerPrefs.GetInt(stagePrefsKey);
        }

        SetStage();

        GameManager.instance.dataManager.SetPlayerSpecs();

        _points = itemCanvas.GetComponentsInChildren<SpriteRenderer>();

        //Dispaying bought upgradestages.
        for (int i = 0; i < _currentStage + 1; i++)
        {
            _points[i].sprite = _square;
        }

        if(_currentStage >= _maxStage)
        {
            Destroy(itemCanvas.GetComponentInChildren<Text>());
        }
    }

    private void SetStage()
    {
        if (upgradeType == UpgradeType.Specs)
        {
            PlayerPrefs.SetFloat(GameManager.instance.dataManager.specsKeys[upgradePrefsIndex],
                _upgradeStages[_currentStage]);
        }
        else if (upgradeType == UpgradeType.Skill)
        {
            PlayerPrefs.SetFloat(GameManager.instance.dataManager.skillsKeys[upgradePrefsIndex],
               _upgradeStages[_currentStage]);
        }
    }

    public override void PickUp()
    {
        if (_currentStage >= _maxStage)
            return;

        base.PickUp();
    }

    public override void OnReachZoneEnter()
    {
        base.OnReachZoneEnter();

        if (_isSpeechBaloonActive)
            return;

        _isSpeechBaloonActive = true;
        speechNPC.Say(speech);
    }

    public override void OnReachZoneExit()
    {
        base.OnReachZoneExit();

        if (!_isSpeechBaloonActive)
            return;

        _isSpeechBaloonActive = false;

        speechNPC.speechBallon.PopDown();
    }

    public override void Buy()
    {
        GameManager.instance.scoreManager.totalCoins -= price;
        _currentStage += 1;

        PlayerPrefs.SetInt(stagePrefsKey, _currentStage);//Memorize stage visual

        SetStage();

        GameManager.instance.UiManager.UpdateCostTxts();

        PlayerPrefs.SetInt("Total coins", GameManager.instance.scoreManager.totalCoins);

        // The way to multiply object price (int).
        float costTransition = (float)price * (float)_priceModifier;
        price = (int)costTransition;
        PlayerPrefs.SetInt(pricePrefsKey, price);

        for (int i = 0; i < _currentStage+1; i++)
        {
            _points[i].sprite = _square;
        }

        itemCanvas.GetComponentInChildren<Text>().text = price + "$";
        GameManager.instance.dataManager.SetPlayerSpecs();
        Instantiate(GameManager.instance.itemsManager.buyParticle, transform.position, Quaternion.identity);

        if (_currentStage >= _maxStage)
        {
            Destroy(itemCanvas.GetComponentInChildren<Text>());
        }
    }
}
