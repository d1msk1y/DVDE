using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    [Header("Bullet settings")]
    public float lifeTime;
    public float bulletSpeed;
    [SerializeField]private bool _isFractioned;
    [SerializeField]private float _fractionModifier;
    [HideInInspector]public int damage;
    public int criticalDamageChance;

    [Header("GFX")]
    public ParticleSystem explosionParticle;

    [Header("Other")]
    public GameObject parent;
    public ActorShooting shootingScript;
    public GameObject damageStats;
    public GameObject criticalDamageTxt;
    public LayerMask whatIsSolid;
    public SpriteRenderer spriteRenderer;
    public TrailRenderer trailRenderer;

    [HideInInspector]public Rigidbody2D _rigidBody;
    public static Bullet instance;

    private void Awake()
    {
        instance = this;
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (_isFractioned)
        {
            _rigidBody.AddForce(new Vector2(0, Random.Range(-1, 1) * _fractionModifier), ForceMode2D.Impulse);
            bulletSpeed = RandomizeInt(0.8f, 1.1f, bulletSpeed);
        }
    }

    private void OnEnable()
    {
        if (_isFractioned)
        {
            _rigidBody.AddForce(new Vector2(0, Random.Range(-1, 1) * _fractionModifier), ForceMode2D.Impulse);
        }
    }

    private void Update()
    {
        //---Destroy bullet if timer is out---//
        lifeTime -= 1 * Time.deltaTime;//Timer
        if (lifeTime <= 0)
            BulletHit(explosionParticle,null);
        //---Destroy bullet if timer is out---//

        transform.Translate(Vector2.right * bulletSpeed * Time.deltaTime);//Bullet move
    }

    private void BulletHit(ParticleSystem explosion, Collider2D collision)//On bullet collide with something function
    {       
        if (collision == null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity).startColor = spriteRenderer.color;
            Destroy(gameObject);
            if (_isFractioned)
                Destroy(gameObject.transform.parent.gameObject);
            return;
        }       

        damage = RandomizeInt(0.8f, 1.2f, damage);// Randomizing bullet damage to make it look THICC

        int criticalDamageChanceRand = Random.Range(1, criticalDamageChance);
        if(criticalDamageChanceRand == 1)
        {
            damage *= 2;
        }
        if (parent.tag != "Player")
            criticalDamageChanceRand = 0;//If bullet shot by non plyer - critical damage chance equals to zero.

        collision.GetComponent<EntityHealth>().TakeDamage(damage, collision.GetComponent<SpriteRenderer>());

        SpawnDamageStats(collision, criticalDamageChanceRand);//Damage stats after bullet colide with an actor
        if(explosion != null)
            Instantiate(explosion, transform.position, Quaternion.identity);//Particles instantiate
        if (parent.tag == "Player")
        {
            if (shootingScript.gunScript.weaponType != WeaponType.LaserGun)
                Destroy(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private int RandomizeInt(float a, float b, float toRandomize)
    {
        float transVar = toRandomize;
        transVar *= Random.Range(a, b);

        return (int)transVar;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bullet" || collision.tag == "EnemyBullet" || collision.gameObject.layer == 0)
            return;

        #region Enemy decection

        if (collision.tag == "Enemy")
        {
            //---if bullet's parent equals to null bullet do nothing---//
            if (parent == null)
            {
                return;
            }
            else if (parent.tag == "Enemy")
            {
                damage /= 5;
                collision.GetComponent<EntityHealth>().TakeDamage(damage, collision.GetComponent<SpriteRenderer>());
                return;
            }
;
            BulletHit(collision.GetComponent<EnemyController>().hitParticle, collision);
            GameManager.instance.statsManager.givenDamage += damage;
            PlayerPrefs.SetInt(GameManager.instance.statsManager.keys[3], GameManager.instance.statsManager.givenDamage);
        }

        #endregion

        if (collision.tag == "Player")
        {
            if (parent == null || parent.tag == "Player")
                return;
;
            BulletHit(collision.GetComponent<PlayerController>().hitParticle, collision);
            GameManager.instance.statsManager.receivedDamage += damage;
            PlayerPrefs.SetInt(GameManager.instance.statsManager.keys[4], GameManager.instance.statsManager.receivedDamage);
        }

        if(collision.tag == "Destructible")
        {
            collision.GetComponent<Box>().TakeDamage(damage);
            BulletHit(explosionParticle, null);
        }
        if (collision.tag != "Player" || collision.tag != "Enemy")
            BulletHit(explosionParticle, null);
    }

    private void SpawnDamageStats(Collider2D collision, int critChance)
    {
        if (parent == null)
            return;

        GameObject curDamage = Instantiate(damageStats, transform.position, Quaternion.identity, collision.gameObject.GetComponentInChildren<Canvas>().transform);//Get reference to instantiated damage stats object.
        curDamage.GetComponentInChildren<Text>().text = "- " + damage;//Setting a damage info to instantiated damage stats object.
        curDamage.GetComponentInChildren<Text>().color = parent.GetComponent<SpriteRenderer>().color;//Setting a damage info to instantiated damage stats object.

        GameObject critTXT = null;
        if(critChance == 1)
            critTXT = Instantiate(criticalDamageTxt, transform.position, Quaternion.identity, GameManager.instance.UiManager.mainUiCanvas.transform);//Critical damage instantiate

        Destroy(curDamage, 1.5f);
        Destroy(critTXT, 1.5f);
    }

}
