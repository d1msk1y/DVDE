using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarImg;
    public Image shieldBarImg;
    public GameObject shieldBar;
    public Animator shieldBarAnimator;

    public EntityHealth entityHealth;

    private bool _isShieldBarActive;
    private bool _isShieldBarPopDowned;

    private void OnEnable()
    {
        entityHealth.onDamageEvent += UpdateHealthBar;
        entityHealth.onHealEvent += UpdateHealthBar;
    }

    private void Update()
    {
        var shieldBarAmount = (float)entityHealth.shield / entityHealth.maxShield;
        shieldBarImg.fillAmount = Mathf.Lerp(shieldBarImg.fillAmount, shieldBarAmount, 0.3f);

        var healthBarAmount = (float)entityHealth.Health / entityHealth.maxHealth;
        healthBarImg.fillAmount = Mathf.Lerp(healthBarImg.fillAmount, healthBarAmount, 0.3f);
    }

    private void UpdateHealthBar()
    {
        if (!gameObject.activeSelf) return;
        if (entityHealth.shield <= 0)
        {
            StartCoroutine(ShieldBarPopDown());
        }

        if (entityHealth.shield != entityHealth.startShield || entityHealth.shield > 0 && _isShieldBarPopDowned)
        {
            if (entityHealth.shield <= 0)
                return;
            ShieldBarPopUp();
        }
    }

    private IEnumerator ShieldBarPopDown()
    {
        _isShieldBarPopDowned = true;
        if (!_isShieldBarActive && !_isShieldBarPopDowned)
            yield return null;
        _isShieldBarActive = false;
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
        _isShieldBarPopDowned = false;
        if (_isShieldBarActive)
            return;
        shieldBar.SetActive(true);
        _isShieldBarActive = true;
        shieldBarAnimator.Play("ShieldBarPopUp");
    }
}
