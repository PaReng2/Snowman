using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;
    float moveX;
    float moveZ;
    Animator animator;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();  
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
    }

    void Move()
    {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");
        
        Vector3 moveVec = new Vector3(moveX, 0, moveZ).normalized;
        transform.position += moveVec * moveSpeed * Time.deltaTime;

        animator.SetBool("isMove", moveVec != Vector3.zero);
        
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
        }
    }
    
}
