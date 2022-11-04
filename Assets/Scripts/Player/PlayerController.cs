using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header("Essential references")]
    public ActorShooting shootingScript;
    public PlayerMovement playerMovement;
    public EntityHealth entityHealth;
    public ClothSlotController clothSlotController;

    public IAbility Ability { get; private set; }

    [Header("GFX")]
    public ParticleSystem deathParticle;
    public ParticleSystem hitParticle;
    private Color _startColor;

    [Space(10)]
    public bool isAlive = true;
    public bool isShooting;
    private bool _isSecondLifeUsed;
    private float _startModifier;

    [HideInInspector] public Rigidbody2D _rigidBody;
    public static PlayerController instance;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        instance = this;
    }

    private void Start()
    {
        GiveCloth();
        Ability = GetComponent<IAbility>();
        _startColor = GetComponent<SpriteRenderer>().color;
    }

    private void LateUpdate()
    {
        if (!isAlive)
            return;

#if UNITY_EDITOR
        if (Input.GetButton("Fire1"))
            Shot();
        if (Input.GetButtonUp("Fire1"))
        {
            shootingScript.ClearAllLines();
            ThrowGun();
        }
#endif

        if (GameManager.instance.UiManager.shootingJoystick.Vertical >= 0.6 || GameManager.instance.UiManager.shootingJoystick.Horizontal <= -0.6 ||
            GameManager.instance.UiManager.shootingJoystick.Vertical <= -0.6 || GameManager.instance.UiManager.shootingJoystick.Horizontal >= 0.6)
            Shot();

#if PLATFORM_ANDROID
        if (GameManager.instance.UiManager.shootingJoystick.Horizontal == 0 || GameManager.instance.UiManager.shootingJoystick.Vertical == 0)
        {
            ThrowGun();
            shootingScript.ClearAllLines();
        }
#endif

        if (isShooting)
            Shot();

        if (Input.GetButtonDown("Jump"))
        {
            Ability.TriggerAction();
        }

        if (entityHealth.Health <= 0)
            Die();


        if (Input.GetKeyDown(KeyCode.Q))
            GameManager.instance.dataManager.SetPlayerSpecs();
    }

    public void SetShooting (bool value) {
        isShooting = value;
    }
    
    private void Shot()
    {
        shootingScript.Shot(_rigidBody);
    }

    public void EnterRageMode()
    {
        _startModifier = shootingScript.damageModifier;
        shootingScript.damageModifier = 100000;
    }

    public void ExitRageMode() => shootingScript.damageModifier = _startModifier;

    private void ThrowGun() => shootingScript.ThrowGun(shootingScript.gunScript.gun2Throw);

    private void GiveCloth()
    {
        if (clothSlotController.glassesIndex == -1 && clothSlotController.hatIndex == -1)
        {
            return;
        }

        if (clothSlotController.glassesIndex == -1 && clothSlotController.hatIndex != -1)
        {
            clothSlotController.hatSlot.GiveCloth(GameManager.instance.itemsManager.hats[clothSlotController.hatIndex],
            clothSlotController.hatSlot.clotheIndex, null, ClothType.Hat);
            return;
        }

        if (clothSlotController.glassesIndex != -1 && clothSlotController.hatIndex == -1)
        {

            clothSlotController.glassesSlot.GiveCloth(GameManager.instance.itemsManager.glasses[clothSlotController.glassesIndex],
            clothSlotController.glassesSlot.clotheIndex, null, ClothType.Glasses);
            return;
        }

        clothSlotController.glassesSlot.GiveCloth(GameManager.instance.itemsManager.glasses[clothSlotController.glassesIndex],
            clothSlotController.glassesSlot.clotheIndex, null, ClothType.Glasses);

        clothSlotController.hatSlot.GiveCloth(GameManager.instance.itemsManager.hats[clothSlotController.hatIndex],
        clothSlotController.hatSlot.clotheIndex, null, ClothType.Hat);
    }

    private void Die()
    {
        Instantiate(deathParticle, transform.position, Quaternion.identity);
        StartCoroutine(GameManager.instance.ExitRageMode(0));
        StartCoroutine(GameManager.instance.ExitSlowMo(0));
        isAlive = false;
        gameObject.SetActive(false);
        CheckSecondLifeChance();
    }

    private void Reborn()
    {
        isAlive = true;
        _isSecondLifeUsed = true;
        gameObject.SetActive(true);
        entityHealth.ResetHealth();
        GameManager.instance.Reborn();
    }

    private void CheckSecondLifeChance()
    {
        int chanceStage = (int)PlayerPrefs.GetFloat(GameManager.instance.dataManager.skillsKeys[0]);
        int chance = Random.Range(1, chanceStage);
        if (chanceStage == 0)
            chance = 0;

        if (chance != 1 || _isSecondLifeUsed)
        {
            GameManager.instance.GameOver();
        }
        if (chance == 1 && !_isSecondLifeUsed)
        {
            Reborn();
        }
    }

    public void Restart(Transform restartPos)
    {
        gameObject.SetActive(true);
        isAlive = true;
        transform.position = restartPos.position;
        entityHealth.ResetHealth();
        shootingScript.GiveWeapon(shootingScript.defaultGun);
        GetComponent<SpriteRenderer>().color = _startColor;
        _isSecondLifeUsed = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            GameManager.instance.soundManager._vfxAudioSource.PlayOneShot(GameManager.instance.soundManager.hitWall);
        }
    }

}
