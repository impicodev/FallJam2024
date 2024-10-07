using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Input;

public class Player : MonoBehaviour
{
    [Header("Constants")]
    [SerializeField] float speed = 1.0f;


    Vector2 pos;
    Quaternion rotation;

    Transform myTransform;
    float deltaTime;
    // Start is called before the first frame update
    void Start()
    {
        myTransform = GetComponent<Transform>();
        pos = myTransform.position;
        rotation = myTransform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime = Time.deltaTime;
        Move();
        Look();
        myTransform.position = pos;
        myTransform.rotation = rotation;
    }

    void Move() {
        pos.x += speed * deltaTime * Input.GetAxisRaw("Horizontal");
        pos.y += speed * deltaTime * Input.GetAxisRaw("Vertical");
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
}

