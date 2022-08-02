using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarImg;
    public Image shieldBarImg;
    public GameObject shieldBar;
    public Animator shieldBarAnimator;

    public EntityHealth entityHealth;

    private bool isShieldBarActive = false;

    private bool isShieldBarPopDowned = false;

    private void OnEnable()
    {
        entityHealth.onDamageEvent += UpdateHealthBar;
    }

    private void Update()
    {
        float shieldBarAmount = (float)entityHealth.shield / entityHealth.maxShield;
        shieldBarImg.fillAmount = Mathf.Lerp(shieldBarImg.fillAmount, shieldBarAmount, 0.3f);

        float healthBarAmount = (float)entityHealth.Health / entityHealth.maxHealth;
        healthBarImg.fillAmount = Mathf.Lerp(healthBarImg.fillAmount, healthBarAmount, 0.3f);
    }

    private void UpdateHealthBar()
    {
        if (entityHealth.shield <= 0)
        {
            StartCoroutine(ShieldBarPopDown());
        }

        if (entityHealth.shield != entityHealth.startShield || entityHealth.shield > 0 && isShieldBarPopDowned)
        {
            if (entityHealth.shield <= 0)
                return;
            ShieldBarPopUp();
        }
    }

    private IEnumerator ShieldBarPopDown()
    {
        isShieldBarPopDowned = true;
        if (!isShieldBarActive && !isShieldBarPopDowned)
            yield return null;
        isShieldBarActive = false;
#pragma warning disable CS0618 // Type or member is obsolete
        if (shieldBar.gameObject.active)
#pragma warning restore CS0618 // Type or member is obsolete
            shieldBarAnimator.Play("ShieldBarPopDown");
    }

    private IEnumerator HideShieldBar()
    {
        yield return new WaitForSeconds(1f);
        shieldBar.SetActive(false);
    }

    private void ShieldBarPopUp()
    {
        isShieldBarPopDowned = false;
        if (isShieldBarActive)
            return;
        shieldBar.SetActive(true);
        isShieldBarActive = true;
        shieldBarAnimator.Play("ShieldBarPopUp");
    }
}
