using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] monsterPrefabs; // 몬스터 프리팹 배열
    public float spawnInterval = 2f;    // 몬스터 스폰 간격
    public float spawnRadius = 10f;     // 스폰 위치 반경
    public Transform player;            // 플레이어의 Transform 컴포넌트
    


    // Start is called before the first frame update
    void Start()
    {
        // 시작 시 스폰 코루틴 시작
        StartCoroutine(SpawnMonster());
      
    }

    // 코루틴으로 몬스터 스폰
    IEnumerator SpawnMonster()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // 랜덤한 위치 계산 (플레이어 주변에서)
            Vector3 spawnPosition = GetRandomSpawnPosition();

            // 랜덤한 몬스터 프리팹 선택
            GameObject monsterPrefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Length)];

            // 몬스터 생성
            Instantiate(monsterPrefab, spawnPosition, Quaternion.identity, transform);
        }
    }

    // 랜덤한 스폰 위치 계산 (플레이어 주변에서)
    Vector3 GetRandomSpawnPosition()
    {
        // 플레이어가 바라보는 방향에 랜덤한 각도를 추가 (예: -30도에서 +30도 사이)
        float randomAngleOffset = Random.Range(-60f, 60f);
        Vector3 playerForward = player.forward;
        Quaternion spawnRotation = Quaternion.Euler(0, randomAngleOffset, 0);
        Vector3 spawnDir = spawnRotation * playerForward;

        // 플레이어의 위치를 기준으로 스폰 위치 조정
        float spawnDistance = spawnRadius;
        Vector3 spawnPosition = player.position + spawnDir * spawnDistance;

        return spawnPosition;
    }
}