using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController controller;

    public float speed;
    public float gravity;

    private Animator anim;

    private Transform cam;

    Vector3 moveDirection;

    public float smoothRotTime;
    private float turnSmoothVelocity;

    public float colliderRadius;
    public List<Transform> enemyList = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        GetMouseInput();
    }

    void Move()
    {
        if (controller.isGrounded)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(horizontal, 0f, vertical);

            if (direction.magnitude > 0)
            {
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref turnSmoothVelocity, smoothRotTime);
            
                transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
                
                moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * speed;
                
                anim.SetInteger("transition", 1);
                
            }
            else
            {
                anim.SetInteger("transition", 0);
                moveDirection = Vector3.zero;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        
        controller.Move(moveDirection * Time.deltaTime);

    }

    void GetMouseInput()
    {
        if (controller.isGrounded)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine("Attack");
            }
        }
    }

    IEnumerator Attack()
    {
        anim.SetInteger("transition", 2);
            
        yield return new WaitForSeconds(0.4f);
        
        GetEnemiesList();

        foreach (Transform e in enemyList)
        {
            Debug.Log(e.name);
        }

        yield return new WaitForSeconds(1f);
        
        anim.SetInteger("transition", 0);
    }

    void GetEnemiesList()
    {
        enemyList.Clear();
        foreach (Collider c in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius))
        {
            if(c.gameObject.CompareTag("Enemy"))
            {
                enemyList.Add(c.transform);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, colliderRadius);
    }
}
