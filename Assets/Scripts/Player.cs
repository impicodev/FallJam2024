using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public float Health
    {
        get { return health; }
        set
        {   
            if(isInvuln) {
                return;
            }
            // only do this stuff if the player isn't going to die
            if(value - health < 0 && value > 0) {
                isInvuln = true;
                StartCoroutine(TempInvlun(hitInvulnTime));
                StartCoroutine(FlashSprite(hitInvulnTime));
                audioSource.clip = hurtSound;
                audioSource.Play();

            }
            health = value;
            // update healthbar UI
            Debug.Log("Player is at " + health + " health");
            //if (hpBar) hpBar.normalizedValue = health / maxHealth;
            manager.DisplayPlayerHealth(health / maxHealth);
            if (health <= 0)
                Die();
        }
    }
    public int Ammo
    {
        get { return ammo; }
        set
        {
            if(ammo - value < 0) {
                audioSource.clip = gunLoadSound;
                audioSource.Play();
            }
            ammo = Mathf.Min(maxAmmo, value);
            //Debug.Log("Player has " + ammo + " ammos");
            // update ammo UI
            manager.DisplayAmmo(ammo);
        }
    }

    public BossSceneManager manager;
    public Animator animator;
    public Transform sprite;
    public Transform anchor;
    public bool clickToSwing = true;
    public Gun shotgun;
    public GameObject parryTool;
    public TMP_Text bigText, ammoText;
    public Slider hpBar;

    private AudioSource audioSource;
    public AudioClip gunLoadSound;
    public AudioClip gunFireSound;
    public AudioClip hurtSound;
    public AudioClip deathSound;

    [Header("Constants")]
    [SerializeField] int maxAmmo = 8;
    [SerializeField] float speed = 1.0f;
    [SerializeField] float maxHealth = 100;
    [SerializeField] float hitInvulnTime = 0.3f;

    [Header("Parry Swing")]
    [SerializeField] float swingStartAngle = -23.0f;
    [SerializeField] float swingEndAngle = 23.0f;
    [SerializeField] float maxSwingTime = 0.3f;
    [SerializeField] float swingCooldown = 5.0f;

    public static int aliveMinions = 0;


    Quaternion rotation;
    float health;
    int ammo = 0;
    [System.NonSerialized] public Rigidbody2D rb;

    private bool hasParry = true;
    private bool hasGun = true;
    private bool isInvuln = false;
    private bool isFrozen = false;

    private Quaternion swingStartRotation;
    private Quaternion swingEndRotation;

    private float swingTime = 0.0f;
    private float swingCooldownTimer = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        ResetTimeScale();
        Instance = this;
        Health = maxHealth;
        Ammo = 0;
        rotation = transform.rotation;
        rb = GetComponent<Rigidbody2D>();

        audioSource = GetComponent<AudioSource>();

        if (!parryTool) {
            Debug.LogWarning("Could not find the ParryTool gameobject!");
            hasParry = false;
        }

        if(!shotgun) {
            Debug.LogWarning("Could not find the Gun component!");
            hasGun = false;
        }

        swingStartRotation = Quaternion.Euler(0, 0, swingStartAngle);
        swingEndRotation = Quaternion.Euler(0, 0, swingEndAngle);

        parryTool.SetActive(!clickToSwing);
        shotgun.transform.GetChild(0).gameObject.SetActive(clickToSwing);
    }

    // Update is called once per frame
    void Update()
    {
        if (isFrozen) return;
        
        if (!clickToSwing)
        {
            shotgun.transform.position = parryTool.transform.GetChild(0).position;
            shotgun.transform.up = parryTool.transform.right;
        }

        if (Input.GetKeyDown(KeyCode.Space)) // TESTING PURPOSES ONLY
            Ammo += 1;
        // rmb for now
        if (Input.GetMouseButtonDown(1) && Ammo > 0 && hasGun)
        {
            audioSource.PlayOneShot(gunFireSound);
            shotgun.Shoot(Ammo);
            Ammo = 0;
        }

        // kills the player for testing
        if(Input.GetKeyDown("k")) {
            Health = 0;
        }

        Move();
        Look();
        if(hasParry) {Parry();}
        anchor.rotation = rotation;
    }

    void Move() {
        rb.velocity = new Vector2(
            speed * Input.GetAxisRaw("Horizontal"),
            speed * Input.GetAxisRaw("Vertical"));
        //animator.SetBool("Walking", rb.velocity.x != 0);
        animator.SetBool("Walking", rb.velocity.x != 0 || rb.velocity.y != 0);

        Vector3 scale = sprite.localScale;
        if (Input.GetAxisRaw("Horizontal") != 0)
            scale.x = Mathf.Abs(scale.x) * (Input.GetAxisRaw("Horizontal") > 0 ? -1 : 1);
        sprite.localScale = scale;

        Vector3 corner = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, corner.x, -corner.x);
        pos.y = Mathf.Clamp(pos.y, corner.y, -corner.y);
        transform.position = pos;
    }

    void Look() {
        Vector2 mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float deltax = mousePosWorld.x - transform.position.x;
        float deltay = mousePosWorld.y - transform.position.y;
        float angle = Mathf.Atan(deltay / deltax) * Mathf.Rad2Deg;
        if(deltax < 0) {
            angle -= 180.0f;
        }

        rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Parry() {
        // this feels evil
        if (!clickToSwing) return;
        if(Input.GetMouseButtonDown(0) && swingCooldownTimer >= swingCooldown) {
            swingTime += Time.deltaTime;
            parryTool.SetActive(true);
            shotgun.gameObject.SetActive(false);
            swingCooldownTimer = 0.0f;

            int sign = Camera.main.ScreenToWorldPoint(Input.mousePosition).x >= transform.position.x ? 1 : -1;
            swingStartRotation = Quaternion.Euler(0, 0, swingStartAngle * sign);
            swingEndRotation = Quaternion.Euler(0, 0, swingEndAngle * sign);

            Vector3 scale = parryTool.transform.GetChild(0).localScale;
            scale.y = Mathf.Abs(scale.y) * sign;
            parryTool.transform.GetChild(0).localScale = scale;
        }
        parryTool.transform.rotation = Quaternion.Slerp(swingStartRotation * rotation, swingEndRotation * rotation, swingTime / maxSwingTime);
        if(swingTime > 0.0f) {
            swingTime += Time.deltaTime;
        }
        if(swingCooldownTimer > 0.0f) { 
            swingCooldownTimer += Time.deltaTime;
        }
        if(swingTime > maxSwingTime) {
            swingTime = 0.0f;
            parryTool.transform.rotation = swingStartRotation;
            parryTool.SetActive(false);
            shotgun.gameObject.SetActive(true);
            swingCooldownTimer += Time.deltaTime;
        }
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        isInvuln = true; // hack to stop the player from taking more damage once dead
        audioSource.clip = deathSound;
        audioSource.Play();
        SloMo(3.0f);
        StopCoroutine("FlashSprite");
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);

        manager.PlayerDied();
    }

    private void SloMo(float seconds) {
        Time.timeScale = 0.5f;
        Invoke("ResetTimeScale", seconds * 0.5f); // i think this function /is/ affected by the timescale
    }
    private void ResetTimeScale() {
        Time.timeScale = 1.0f;
    }

    public void Freeze()
    {
        isInvuln = true;
        isFrozen = true;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        animator.SetBool("Walking", false);
    }

    private IEnumerator TempInvlun(float seconds) {
        yield return new WaitForSecondsRealtime(seconds);
        isInvuln = false;
    }

    private IEnumerator FlashSprite(float seconds) {
        int flashTimes = 4;
        for(int i = 0; i < flashTimes; i++) {
            sprite.gameObject.SetActive(false);
            yield return new WaitForSecondsRealtime(seconds / (flashTimes * 2));
            sprite.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(seconds / (flashTimes * 2));
        }
    }
}

