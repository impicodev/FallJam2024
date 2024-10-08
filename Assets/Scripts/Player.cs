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
            // update ammo UI
        }
    }

    public Gun shotgun;

    [Header("Constants")]
    [SerializeField] int maxAmmo = 8;
    [SerializeField] float speed = 1.0f;
    [SerializeField] float maxHealth = 100;

    Vector2 pos;
    Quaternion rotation;
    float health;
    int ammo = 0;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        health = maxHealth;
        pos = transform.position;
        rotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // TESTING PURPOSES ONLY
            Ammo += 1;

        if (Input.GetMouseButtonDown(0) && Ammo > 0)
        {
            shotgun.Shoot(Ammo);
            Ammo = 0;
        }

        Move();
        Look();
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

    private void Die()
    {
        Destroy(gameObject);
    }
}

