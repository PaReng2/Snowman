using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RangeMonster : MonoBehaviour
{
    // 몬스터의 공격 발사체(투사체) 프리팹
    public GameObject projectilePrefab; // ?? 에디터에서 설정할 발사체 프리팹

    // 공격 관련 설정
    public float attackRange = 10.0f; // 몬스터가 공격을 시작할 거리
    public float stopDistance = 8.0f; // 몬스터가 추적을 멈추고 공격을 준비할 거리 (NavMeshAgent의 stoppingDistance와 유사)

    public Transform target;
    public StatSO enemyData;
    public Slider EnemyHpSlider;

    private int enemyDamage;
    private float curEnemyHP;
    private bool isDead = false;

    private PlayerController playerController;

    // PlayerController는 더 이상 직접적인 근접 공격을 하지 않으므로 주석 처리하거나 제거 가능
    // private PlayerController playerController; 


    private NavMeshAgent agent;

    // 발사체의 생성 위치 (몬스터의 입이나 손 등)
    public Transform firePoint; //  에디터에서 설정할 발사 위치

    void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();

        agent = GetComponent<NavMeshAgent>();

        // "Player"라는 이름이 아닌 "Player" 태그를 가진 오브젝트를 찾는 것이 더 안정적입니다.
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object not found! Make sure the Player has the 'Player' tag.");
        }

        curEnemyHP = enemyData.hp;
        EnemyHpSlider.maxValue = curEnemyHP;
        EnemyHpSlider.value = curEnemyHP;

        // agent.updateRotation = false; // NavMeshAgent가 회전을 제어하도록 기본값으로 두거나,
        // agent.updateUpAxis = false;   // 2D 게임이 아닌 경우 이 설정을 제거합니다.

        // 몬스터가 공격 범위 내에서 완전히 멈추도록 설정
        agent.stoppingDistance = stopDistance;

        // playerController = FindAnyObjectByType<PlayerController>(); // 근접 공격 로직 제거
        enemyDamage = enemyData.damage;

        if (firePoint == null)
        {
            // 발사 지점이 설정되지 않았다면, 몬스터 자체의 위치를 사용
            firePoint = transform;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // 총알의 데미지 양을 Bullet 스크립트에서 가져오는 것이 좋지만, 
            // 여기서는 임시로 고정값 10을 유지합니다.
            TakeDamage(playerController.damage);
        }
    }

    void Update()
    {
        if (isDead || target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= attackRange)
        {
            //  공격 범위 안에 들어오면 추적을 멈춤
            agent.isStopped = true;

            // 몬스터가 플레이어를 바라보게 회전
            RotateTowardTarget();
        }
        else
        {
            // ?? 공격 범위 밖이면 플레이어를 추적
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
    }

    // Update에서 몬스터가 플레이어를 바라보게 하는 함수
    private void RotateTowardTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            // Y축 회전만 사용 (2D/3D 게임 설정에 따라 다름)
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }


    private float _pointTime = 1.0f; // 1초마다 실행
    private float _nextTime = 0.0f; // 다음번 실행할 시간

    void FixedUpdate()
    {
        if (isDead || target == null) return;

        // 공격 로직 실행 시간을 제어
        if (Time.time > _nextTime)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // 몬스터가 공격 범위(attackRange) 내에 있고,
            // 추적을 멈춘 상태(isStopped)일 때만 발사
            if (distanceToTarget <= attackRange && agent.isStopped)
            {
                Attack();
                _nextTime = Time.time + _pointTime; // 다음번 공격 시간 설정
            }
            // 범위 밖에 있다면, _nextTime을 지금으로 설정하여 다음 FixedUpdate에서 다시 검사 (또는 바로 다음 공격을 준비)
            else if (distanceToTarget > attackRange)
            {
                // 공격하지 않는 상태에서도 계속해서 시간 검사를 하도록 유지
                _nextTime = Time.time + _pointTime;
            }
        }
    }

    void Attack()
    {
        // ?? 투사체 생성 및 발사
        if (projectilePrefab != null && firePoint != null)
        {
            // 1. 투사체(프리팹)를 발사 위치에 생성
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            // 2. 투사체의 발사 방향 설정 (플레이어를 향하도록)
            Vector3 direction = (target.position - firePoint.position).normalized;

            // 3. 투사체에 데미지 정보와 발사 방향을 전달
            // (Projectile 스크립트가 필요하며, 여기서는 편의상 MonsterDamage 변수를 사용)
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                // Projectile 스크립트가 데미지, 속도 등을 처리하도록 설정
                projectileScript.Initialize(direction, enemyDamage);
            }
            else
            {
                // Projectile 스크립트가 없다면 직접 힘을 가해 발사
                Rigidbody rb = projectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    float projectileSpeed = 15f; // 임의의 발사 속도
                    rb.velocity = direction * projectileSpeed;
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        curEnemyHP -= damage;
        EnemyHpSlider.value = curEnemyHP;

        if (curEnemyHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnEnemyKilled();
        }

        // 몬스터 사망 시 네비게이션을 멈춤
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        Destroy(gameObject);
    }
}