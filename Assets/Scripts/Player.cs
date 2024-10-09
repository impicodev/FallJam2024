using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Input;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public float Health
    {
        get { return health; }
        set
        {
            health = value;
            // update healthbar UI
            Debug.Log("Player is at " + health + " health");
            if (health <= 0)
                Die();
        }
    }
    public int Ammo
    {
        get { return ammo; }
        set
        {
            ammo = Mathf.Min(maxAmmo, value);
            Debug.Log("Player has " + ammo + " ammos");
            // update ammo UI
        }
    }

    [Header("Constants")]
    [SerializeField] int maxAmmo = 8;
    [SerializeField] float speed = 1.0f;
    [SerializeField] float maxHealth = 100;

    [Header("Parry Swing")]
    [SerializeField] float swingStartAngle = -23.0f;
    [SerializeField] float swingEndAngle = 23.0f;
    [SerializeField] float maxSwingTime = 0.3f;
    [SerializeField] float swingCooldown = 5.0f;




    Vector2 pos;
    Quaternion rotation;
    float health;
    int ammo;

    GameObject parryTool;
    public Gun shotgun;

    private bool hasParry = true;
    private bool hasGun = true;

    private Quaternion swingStartRotation;
    private Quaternion swingEndRotation;

    private float swingTime = 0.0f;
    private float swingCooldownTimer = 5.0f;



    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        health = maxHealth;
        ammo = 0;
        pos = transform.position;
        rotation = transform.rotation;

        Transform parryToolTransform = Instance.transform.Find("ParryTool");
        if(parryToolTransform != null) {
            parryTool = parryToolTransform.gameObject;
        }
        else {
            Debug.LogWarning("Could not find the ParryTool gameobject!");
            hasParry = false;
        }

        if(!Instance.transform.Find("Shotgun").TryGetComponent(out shotgun)) {
            Debug.LogWarning("Could not find the Gun component!");
            hasGun = false;
        }

        swingStartRotation = Quaternion.Euler(0, 0, swingStartAngle);
        swingEndRotation = Quaternion.Euler(0, 0, swingEndAngle);

        parryTool.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // TESTING PURPOSES ONLY
            Ammo += 1;
        // rmb for now
        if (Input.GetAxisRaw("Fire2") > 0.0f && Ammo > 0 && hasGun)
        {
            shotgun.Shoot(Ammo);
            Ammo = 0;
        }

        Move();
        Look();
        if(hasParry) {Parry();}
        transform.position = pos;
        transform.rotation = rotation;
    }

    void Move() {
        pos.x += speed * Time.deltaTime * Input.GetAxisRaw("Horizontal");
        pos.y += speed * Time.deltaTime * Input.GetAxisRaw("Vertical");

        Vector3 corner = Camera.main.ScreenToWorldPoint(Vector3.zero);
        pos.x = Mathf.Clamp(pos.x, corner.x, -corner.x);
        pos.y = Mathf.Clamp(pos.y, corner.y, -corner.y);
    }

    void Look() {
        Vector2 mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float deltax = mousePosWorld.x - pos.x;
        float deltay = mousePosWorld.y - pos.y;
        float angle = Mathf.Atan(deltay / deltax) * Mathf.Rad2Deg;
        if(deltax < 0) {
            angle -= 180.0f;
        }

        rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Parry() {
        // this feels evil
        if(Input.GetAxisRaw("Fire1") > 0 && swingCooldownTimer >= swingCooldown) {
            swingTime += Time.deltaTime;
            parryTool.SetActive(true);
            swingCooldownTimer = 0.0f;
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
            swingCooldownTimer += Time.deltaTime;
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}

