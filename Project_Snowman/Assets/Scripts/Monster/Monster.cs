using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    public Transform target;
    public StatSO enemyData;
    public Slider EnemyHpSlider;

    private int enemyDamage;
    private float curEnemyHP;
    private bool isDead = false;

    private PlayerController playerController;
    

    private NavMeshAgent agent;


    

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.Find("Player").transform;
        curEnemyHP = enemyData.hp;
        EnemyHpSlider.maxValue = curEnemyHP;
        EnemyHpSlider.value = curEnemyHP;

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        
        playerController = FindAnyObjectByType<PlayerController>();
        enemyDamage = enemyData.damage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(playerController.damage); // 총알 한 발당 데미지 1
        }
    }

    void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);

            Vector3 direction = (target.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }
        
        
    }

    private float _pointTime = 1.0f; //1초마다 실행
    private float _nextTime = 0.0f; //다음번 실행할 시간

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time > _nextTime)
        {
            _nextTime = Time.time + _pointTime; //다음번 실행할 시간

            Attack();
        }

    }

    void Attack()
    {
        // "Player"라는 레이어를 감지할 마스크 가져오기
        int playerLayer = LayerMask.GetMask("Player");

        // 플레이어가 NPC 주변 2f 반경 안에 있는지 확인 (구체 범위로 충돌체 탐지)
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f, playerLayer);

        // 탐지된 플레이어가 하나라도 있으면 true
        bool hasPlayer = colliders.Length > 0;

        // 플레이어가 근처에 있을 때만 대화 가능
        if (hasPlayer)
        {
            playerController.TakeDamage(enemyDamage);
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

        StageManager.Instance.OnEnemyKilled();
        Destroy(gameObject);
        Debug.Log("몬스터 처치");
    }
}
