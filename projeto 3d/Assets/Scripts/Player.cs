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
    public float damage = 20;

    private Animator anim;

    private Transform cam;

    Vector3 moveDirection;

    private bool isWalking;

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

                if (!anim.GetBool("attacking"))
                {
                    float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                    float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref turnSmoothVelocity, smoothRotTime);
            
                    transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
                
                    moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * speed;
                
                    anim.SetInteger("transition", 1);

                    isWalking = true;
                }
                else
                {
                    anim.SetBool("walking", false);
                    moveDirection = Vector3.zero;
                }
               
                
            }
            else if(isWalking)
            {
                anim.SetBool("walking", false);
                anim.SetInteger("transition", 0);
                moveDirection = Vector3.zero;

                isWalking = false;
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

                if (anim.GetBool("walking"))
                {
                    anim.SetBool("walking", false);
                    anim.SetInteger("transition", 0);
                }

                if (!anim.GetBool("walking"))
                {
                    StartCoroutine("Attack");
                }
                
                
            }
        }
    }

    IEnumerator Attack()
    {
        anim.SetBool("attacking", true);
        anim.SetInteger("transition", 2);
            
        yield return new WaitForSeconds(0.4f);
        
        GetEnemiesList();

        foreach (Transform e in enemyList)
        {
            CombatEnemy enemy = e.GetComponent<CombatEnemy>();

            if (enemy != null)
            {
                enemy.GetHit(damage);
            }
        }

        yield return new WaitForSeconds(1f);
        
        anim.SetInteger("transition", 0);
        anim.SetBool("attacking", false);
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
