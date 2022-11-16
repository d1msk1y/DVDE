using UnityEngine;
using Pathfinding;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy components refercences")]
    [SerializeField] private AstarPath _astar;
    [SerializeField] private AIPath _aIPath;
    [SerializeField] private AIDestinationSetter _aIDestinationSetter;
    [SerializeField] private EntityHealth _entityHealth;
    [SerializeField] private EnemyShooting _enemyShooting;
    [SerializeField] private ClothSlotController clothSlotController;

    [Header("Enemy drop")]
    [SerializeField] private int score;
    [SerializeField] private float _itemDropForce;
    [SerializeField] private GameObject _healItem;
    [SerializeField] private GameObject _shieldItem;
    [SerializeField] private GameObject _xpItem;

    [Header("Enemy AI settings")]
    [SerializeField] private LayerMask _ignoreLayer;
    [SerializeField] private float _lookRadius;
    [SerializeField] private float _enemyVisionMemory;

    [Header("Enemy vision")]
    private bool _isPlayerVisible;
    private bool _isPlayerWithinReach;
    private float _currentVisionMemory = 0;
    private float _distanceToPlayer;

    [Header("GFX")]
    [SerializeField] private ParticleSystem _deathParticle;
    public ParticleSystem hitParticle;
    private SpriteRenderer _spriteRenderer;

    [Header("Other")]
    [HideInInspector] public Rigidbody2D _rigidBody;
    [HideInInspector] public Rigidbody2D _playerRigidBody;
    private RaycastHit2D _hit;
    private GameObject transXpItem;

    public static EnemyController instance;

    internal virtual void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (_playerRigidBody == null)
            return;
        _aIDestinationSetter.target = _playerRigidBody.gameObject.transform;
        SetReferences();

    }

    private void OnEnable()
    {
        SetReferences();
        GameManager.instance.lvlManager.lvlController.CurrentEnemiesInAction.Add(gameObject);
    }

    private void SetReferences()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _astar = GetComponent<AstarPath>();
        _aIPath = GetComponent<AIPath>();
        _aIDestinationSetter = GetComponent<AIDestinationSetter>();
        _entityHealth = GetComponent<EntityHealth>();
        _enemyShooting = GetComponent<EnemyShooting>();
        _playerRigidBody = PlayerController.instance._rigidBody;
        _astar = GameManager.instance.aStarManager.AStar;
        _currentVisionMemory = _enemyVisionMemory;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_entityHealth.Health <= 0)
        {
            Die();
        }

        if (!PlayerController.instance.isAlive)
            return;

        #region Player search

        _distanceToPlayer = Vector2.Distance(_rigidBody.position, _playerRigidBody.position);
        Vector2 dir = (_playerRigidBody.position - _rigidBody.position).normalized;

        if (_distanceToPlayer <= _lookRadius)
        {
            _isPlayerWithinReach = true;
            _hit = Physics2D.Raycast(_rigidBody.position, dir, _lookRadius, ~_ignoreLayer);
        }
        else
        {
            _isPlayerWithinReach = false;
        }

        if (_hit.collider != null)
        {
            if (_hit.collider.GetComponent<PlayerController>() != null)
            {
                _isPlayerVisible = true;
                _currentVisionMemory = _enemyVisionMemory;
            }
            else
                _isPlayerVisible = false;

            if (_isPlayerWithinReach && !_isPlayerVisible)
                _aIPath.whenCloseToDestination = CloseToDestinationMode.ContinueToExactDestination;
            else
                _aIPath.whenCloseToDestination = CloseToDestinationMode.Stop;

            if (_isPlayerWithinReach && _isPlayerVisible || _currentVisionMemory > 0)
            {
                _aIPath.canMove = true;
                if (_currentVisionMemory >= 0)
                    _currentVisionMemory -= 1 * Time.deltaTime;
            }
            else if (!_isPlayerWithinReach || !_isPlayerVisible || _currentVisionMemory < 0)
                _aIPath.canMove = false;

            //---Shooting---//
            if (_isPlayerWithinReach && _isPlayerVisible)
                _enemyShooting.Shot();
        }

        #endregion
    }

    internal virtual void Die()
    {
        Instantiate(_deathParticle, transform.position, Quaternion.identity);
        GameObject healItem = Instantiate(_healItem, transform.position, Quaternion.identity, GameManager.instance.lvlManager.lvlController.transform);
        InstantiateXp();

        int shieldChance = Chance(0, 2);
        if (shieldChance == 1)
            Instantiate(_shieldItem, transform.position, Quaternion.identity, GameManager.instance.lvlManager.lvlController.transform);

        GunPickUp gunInst = Instantiate(_enemyShooting.gunScript.gun2PickUp, transform.position,
            Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360))), GameManager.instance.lvlManager.lvlController.transform);

        gunInst.GetComponent<Rigidbody2D>().AddForce
            (GameManager.instance.RandomVector(-1, 1) * _itemDropForce);
        healItem.GetComponent<Rigidbody2D>().AddForce
            (GameManager.instance.RandomVector(-1, 1) * _itemDropForce);
        transXpItem.GetComponent<Rigidbody2D>().AddForce
            (GameManager.instance.RandomVector(-1, 1) * _itemDropForce);

        GameManager.instance.scoreManager.enemiesKilled += 1;
        GameManager.instance.scoreManager.AddComboPoint(1);

        GameManager.instance.scoreManager.CheckDoubleKill();
        
        GameManager.instance.lvlManager.lvlController.CurrentEnemiesInAction.Remove(gameObject);
        SoundManager.PlayOneShot(SoundManager.instance.actorDeath);
        Destroy(gameObject);
    }

    private IEnumerator RemoveEnemyFromList()
    {
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.lvlManager.lvlController.CurrentEnemiesInAction.Remove(gameObject);
    }

    private void InstantiateXp()
    {
        float randomScore = score * _distanceToPlayer * 0.15f;
        score = (int)randomScore;

        transXpItem = Instantiate(_xpItem, transform.position, Quaternion.identity, GameManager.instance.lvlManager.lvlController.transform);

        float scaleModifier = 0.005f * score;

        if (scaleModifier >= 1.5f)
            scaleModifier = 1.5f;
        else if (scaleModifier <= 0.5f)
            scaleModifier = 0.5f;

        //transXpItem.transform.localScale *= scaleModifier;
        HealItem xpItemProp = transXpItem.GetComponent<HealItem>();
        xpItemProp.useFulAmount = score;
    }

    private int Chance(int a, int b)
    {
        int chance = Random.Range(a, b);
        return chance;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _lookRadius);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Gun")
        {
            Die();
            Destroy(collision.gameObject);
        }
        if (collision.collider.tag == "Knife") {
            _entityHealth.TakeDamage(collision.gameObject.GetComponentInChildren<Gun>().damage, _spriteRenderer);
        }
    }
}