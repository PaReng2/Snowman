using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public float attackRange = 0.1f;
    public StatSO playerStatus;
    public Slider hpSlider;
    private int currentHp;


    public Camera followCamera;

    Vector3 moveVec;

    // 민감도
    public float rotationSpeed = 300f;

   

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        hpSlider.maxValue = playerStatus.hp;
        currentHp = playerStatus.hp;
        hpSlider.value = playerStatus.hp;
        animator = GetComponentInChildren<Animator>();  
        rb = GetComponent<Rigidbody>();
        AttackRate = 2.3f;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
        Turn();

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
        bulletRigid.velocity = firePoint.forward * 10;
    }

    void Turn()
    {
        //transform.LookAt(transform.position + moveVec);


        Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, 100))
        {
            Vector3 nextVec = rayHit.point - transform.position;
            nextVec.y = transform.position.y;
            transform.LookAt(transform.position + nextVec);
        }
    }
    
}
