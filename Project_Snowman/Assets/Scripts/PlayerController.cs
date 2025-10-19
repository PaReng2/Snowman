using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Transform firePoint;
    public GameObject bulletPrefab;
    float moveX;
    float moveZ;
    Animator animator;
    float curLeftAttackTime;
    float AttackRate;
    public float attackRange;
    
    Vector3 moveVec;

    // 민감도
    public float rotationSpeed = 300f;

    // 마우스 입력 값
    private Vector2 lookInput;
    // Y 축 회전 각도 (수직 회전 제한을 위해 필요)
    private float cameraVerticalAngle = 0f;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();  
        rb = GetComponent<Rigidbody>();
        AttackRate = 2.3f;
        attackRange = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
        

        // curLeftAttackTime이 0보다 클 때만 감소시켜서 음수가 되는 것을 방지합니다.
        if (curLeftAttackTime > 0)
        {
            curLeftAttackTime -= Time.deltaTime;
        }

        // 이 아래는 원래 코드와 동일합니다.
        if (Input.GetMouseButtonDown(0))
        {
            if (curLeftAttackTime <= 0)
            {
                    Attack();
                    // 공격 후 쿨다운 시간을 AttackRate로 재설정합니다.
                    curLeftAttackTime = AttackRate;
            }
            else
            {
                Debug.Log("재정비중");
                return;
            }
        }


    }

    void Move()
    {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");
        
        moveVec = new Vector3(moveX, 0, moveZ).normalized;
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
    void Attack()
    {
        GameObject intantBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = firePoint.forward * 50;
    }
    
}
