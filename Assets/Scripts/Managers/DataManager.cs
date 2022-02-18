using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    [Header("Player data")]
    public string[] specsKeys;
    public string[] skillsKeys;

    public PlayerController player;

    public void SetPlayerSpecs()
    {
        if(PlayerPrefs.HasKey(specsKeys[0]))
            player.entityHealth.maxHealth = (int)PlayerPrefs.GetFloat(specsKeys[0]);

        if (PlayerPrefs.HasKey(specsKeys[1]))
            player.entityHealth.maxShield = (int)PlayerPrefs.GetFloat(specsKeys[1]);
        StartCoroutine(UpdatePlayerHealthDelay(player.entityHealth, 1));

        if (PlayerPrefs.HasKey(specsKeys[2]))
            player.shootingScript.damageModifier = PlayerPrefs.GetFloat(specsKeys[2]);

        if (PlayerPrefs.HasKey(specsKeys[3]))
            player.shootingScript.extraAmmosPercent = PlayerPrefs.GetFloat(specsKeys[3]);
        player.shootingScript.extraAmmos += (int)(player.shootingScript.ammos * (player.shootingScript.extraAmmosPercent / 100));

        if (PlayerPrefs.HasKey(specsKeys[4]))
        {
            GameManager.instance.itemsManager.healUseful = (int)PlayerPrefs.GetFloat(specsKeys[4]);
            GameManager.instance.itemsManager.shieldUseful = (int)PlayerPrefs.GetFloat(specsKeys[4]);
        }

        if (PlayerPrefs.HasKey(specsKeys[5]))
            GameManager.instance.scoreManager.maxScoreMultiplier = (int)PlayerPrefs.GetFloat(specsKeys[5]);

        if (PlayerPrefs.HasKey(specsKeys[6]))
            player.shootingScript.criticalDamageChance = (int)PlayerPrefs.GetFloat(specsKeys[6]);

        if (PlayerPrefs.HasKey(skillsKeys[1]))
            GameManager.instance.rageTime = (int)PlayerPrefs.GetFloat(skillsKeys[1]);
        
        if (PlayerPrefs.HasKey(skillsKeys[2]))
            GameManager.instance.slowMoTime = (int)PlayerPrefs.GetFloat(skillsKeys[2]);
    }

    private IEnumerator UpdatePlayerHealthDelay(EntityHealth entityHealth, float delay)
    {
        yield return new WaitForSeconds(delay);
        entityHealth.shield = entityHealth.maxShield;
        entityHealth.health = entityHealth.maxHealth;
    }

}
