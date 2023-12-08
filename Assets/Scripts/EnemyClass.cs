using System.Collections;
using UnityEngine;
public class EnemyClass : MonoBehaviour
{
    public Transform target;
    private Rigidbody rb;
    public float moveSpeed;
    public MonsterType monsterType; // 몬스터의 종류
    private Color originalColor;
    private Renderer enemyRenderer;
    private Coroutine revertColorCoroutine; // 코루틴을 참조하기 위한 변수
    private float enemyHp = 3;
    public GameObject expPrefab;
    public GameObject hitEffectPrefab;
    public GameObject magnet;


    void Start()
    {
        // 플레이어를 찾아서 타겟으로 설정

        GameObject playerObj = GameObject.Find("Player");

        if (playerObj != null)
        {
            target = playerObj.transform;
        }
        else
        {
            Debug.Log("Player GameObject not found!");
        }
        rb = GetComponent<Rigidbody>();
        enemyRenderer = GetComponentInChildren<Renderer>();
        originalColor = enemyRenderer.material.color;
    }

    void FixedUpdate()
    {
        // 몬스터의 종류에 따라 다른 이동 및 회전 로직 호출
        MoveBasedOnMonsterType();
        RotateTowardsDirection();

        if (enemyHp <= 0)
        {
            Vector3 expPos = new Vector3(transform.position.x, 0.5f, transform.position.z);
            Instantiate(expPrefab, expPos, Quaternion.identity);

            int chance = Random.Range(0, 100);
            if (chance < 3)
            {
                Instantiate(magnet, expPos, Quaternion.identity);
            }
            gameObject.SetActive(false);
        }
    }

    void MoveBasedOnMonsterType()
    {
        switch (monsterType)
        {
            case MonsterType.Yokai:
                YokaiMove();
                break;

            case MonsterType.Gimp:
                GimpMove();
                break;

            // 다른 몬스터 종류에 대한 처리 추가 가능

            default:
                break;
        }
    }
    public void OnHit()
    {
        if (revertColorCoroutine != null)
        {
            StopCoroutine(revertColorCoroutine); // 현재 실행 중인 코루틴을 멈춤
        }
        enemyRenderer.material.color = Color.red; // 색상을 빨간색으로 명시적으로 설정
        revertColorCoroutine = StartCoroutine(RevertColor()); // 코루틴을 다시 시작

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, new Vector3(transform.position.x, 1, transform.position.z), Quaternion.identity);
        }

        enemyHp--;
    }
    // 색상을 원래대로 돌리는 코루틴 메서드
    IEnumerator RevertColor()
    {
        float elapsed = 0f;
        float duration = 0.2f;
        Color currentColor = Color.red; // 현재 색상을 빨간색으로 설정

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            enemyRenderer.material.color = Color.Lerp(currentColor, originalColor, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        enemyRenderer.material.color = originalColor;
    }
    void YokaiMove()
    {
        // Yokai 몬스터의 이동 로직 구현
        float circleRadius = 1.5f; // 원의 크기를 조절하여 원형 경로의 반지름 조정
        if (target != null)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            Vector3 circularDirection = Vector3.Cross(directionToTarget, Vector3.up).normalized; // XZ 평면에서의 움직임을 가정

            Vector3 combinedDirection = (directionToTarget + circularDirection * circleRadius).normalized;
            rb.velocity = combinedDirection * moveSpeed;
        }
        else
        {
            Debug.Log("Player GameObject not found!");
        }

    }

    void GimpMove()
    {
        // Butcher 몬스터의 이동 로직 구현
        // 예: 다른 패턴으로 이동
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
        }
        else
        {
            Debug.Log("Player GameObject not found!");
        }


    }

    void RotateTowardsDirection()
    {
        if (rb.velocity != Vector3.zero)
        {
            // Look at the direction of movement
            Quaternion targetRotation = Quaternion.LookRotation(rb.velocity);
            transform.rotation = targetRotation;
        }
    }

    public enum MonsterType
    {
        Yokai,
        Gimp,
        Type3,
        Type4,
        Type5,
        // 추가적인 몬스터 종류 정의 가능
    }
}