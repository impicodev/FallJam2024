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
            health = value;
            // update healthbar UI
            Debug.Log("Player is at " + health + " health");
            if (hpBar) hpBar.normalizedValue = health / maxHealth;
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
            if (ammoText) ammoText.text = "Ammo: " + ammo.ToString() + " / " + maxAmmo.ToString();
        }
    }

    public Transform sprite;
    public Transform anchor;
    public bool clickToSwing = true;
    public Gun shotgun;
    public GameObject parryTool;
    public TMP_Text bigText, ammoText;
    public Slider hpBar;

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
    int ammo = 0;

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
        Health = maxHealth;
        Ammo = 0;
        pos = transform.position;
        rotation = transform.rotation;

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
            shotgun.Shoot(Ammo);
            Ammo = 0;
        }

        Move();
        Look();
        if(hasParry) {Parry();}
        transform.position = pos;
        anchor.rotation = rotation;
    }

    void Move() {
        pos.x += speed * Time.deltaTime * Input.GetAxisRaw("Horizontal");
        pos.y += speed * Time.deltaTime * Input.GetAxisRaw("Vertical");

        Vector3 scale = sprite.localScale;
        if (Input.GetAxisRaw("Horizontal") != 0)
            scale.x = Mathf.Abs(scale.x) * (Input.GetAxisRaw("Horizontal") > 0 ? 1 : -1);
        sprite.localScale = scale;

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

    public void BossDied()
    {
        bigText.text = "YOU WON\n(yippee!)";
        StartCoroutine(reloadScene(4));
    }

    private void Die()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
        bigText.text = "YOU DIED\n(womp womp)";
        StartCoroutine(reloadScene(4));
    }

    private IEnumerator reloadScene(float wait)
    {
        yield return new WaitForSeconds(wait);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

