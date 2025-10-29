using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    private int damage;

    // RangeMonster 스크립트에서 호출될 초기화 함수
    public void Initialize(Vector3 direction, int monsterDamage)
    {
        damage = monsterDamage;
        // Rigidbody를 사용하여 투사체 발사
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * speed * Time.deltaTime;
        }

        // 일정 시간 후 스스로 파괴되도록 설정 (벽에 닿지 않을 경우)
        Destroy(gameObject, 3f);
    }

    // Is Trigger가 체크된 충돌체 감지
    private void OnTriggerEnter(Collider other)
    {
        //  플레이어와 충돌했는지 확인
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }

            // 투사체 파괴
            Destroy(gameObject);
        }
        //  플레이어가 아닌 다른 오브젝트(벽 등)와 충돌 시 파괴
        else if (!other.CompareTag("enemy") && !other.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
    }
}