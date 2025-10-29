using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Transform firePoint;
    public GameObject snowBallPrefab;
    public GameObject iceBallPrefab;

    float moveX;
    float moveZ;
    Animator animator;
    float curLeftAttackTime;
    float AttackRate;
    public StatSO playerStatus;
    public Slider hpSlider;
    public Text hpText;
    public Text curWeapon;

    public int maxHp;
    public int damage;

    private int currentHp;
    private bool isGrounded;

    public Camera followCamera;
    public ParticleSystem dashEffect;

    Vector3 moveVec;

    private bool isDead;

    // 민감도
    public float rotationSpeed = 300f;
    Rigidbody rb;



    public enum Weapon
    {
        snowBall,
        iceBall
    }

    public Weapon currentWeaopn;

    
    // Start is called before the first frame update
    void Start()
    {
        hpSlider.maxValue = playerStatus.hp;
        currentHp = playerStatus.hp;
        hpSlider.value = playerStatus.hp;
        animator = GetComponentInChildren<Animator>();  
        rb = GetComponent<Rigidbody>();
        AttackRate = 2.3f;
        currentWeaopn = Weapon.snowBall;
        isDead = false;
        damage = playerStatus.damage;
        maxHp = playerStatus.hp;
    }

    // Update is called once per frame
    void Update()
    {
        hpText.text = $"{currentHp} / {maxHp}";
        curWeapon.text = $"{currentWeaopn}";
        Move();
        Jump();
        Turn();
        
        Dash();

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

    void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            rb.AddForce(moveVec * 5, ForceMode.Impulse);

            dashEffect.Play();
        }
        else
        {
            dashEffect.Stop();
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }

    void Attack()
    {
        if (currentWeaopn == Weapon.snowBall)
        {
            GameObject intantBullet = Instantiate(snowBallPrefab, firePoint.position, firePoint.rotation);
            Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
            bulletRigid.velocity = firePoint.forward * 30;

            AttackRate = 2.3f;
        }

        if (currentWeaopn == Weapon.iceBall)  //추후에 추가할 예정
        {
            GameObject intantBullet = Instantiate(iceBallPrefab, firePoint.position, firePoint.rotation);
            Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
            bulletRigid.velocity = firePoint.forward * 13;

            AttackRate = 0.7f;
        }

    }

    void Turn()
    {
        // ray 변수를 선언한 곳과 followCamera의 선언/할당에 문제가 없다고 가정합니다.
        Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;

        if (Physics.Raycast(ray, out rayHit, 100))
        {
            // 1. 목표 지점까지의 방향 벡터를 계산합니다. (nextVec 대신 targetDirection을 사용했습니다.)
            Vector3 targetDirection = rayHit.point - transform.position;

            // 2. 캐릭터의 높이 변화(y축)를 무시하기 위해 방향 벡터의 y 성분을 0으로 만듭니다.
            targetDirection.y = 0;

            // 3. 방향 벡터가 유효한지 확인합니다. (Vector3.zero일 경우 LookRotation 에러 방지)
            if (targetDirection != Vector3.zero)
            {
                // 4. LookRotation으로 목표 방향으로의 회전을 계산합니다.
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

                transform.rotation = targetRotation;
            }
        }
    }

    

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHp -= damage;
        hpSlider.value = currentHp;

        if (currentHp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        SceneManager.LoadScene(0);
    }
}
