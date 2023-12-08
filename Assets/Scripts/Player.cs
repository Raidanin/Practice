using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float currentExp = 0;
    public float maxExp;
    public int level = 1;
    public float maxHp = 3;
    private float currentHp;
    private Animator anim;
    private float pullForce = 10f;
    private List<GameObject> expObjects = new List<GameObject>();
    public bool isLevelUping = false;
    public GameObject levelUpEffect;
    private Coroutine levelUpEffectCoroutine;
    public event Action LevelUpEvent;
    private float effectTime = 0.5f;


    void Start()
    {
        currentExp = 0;
        level = 1;
        currentHp = maxHp;
        anim = GetComponent<Animator>(); // 애니메이터 컴포넌트를 가져옵니다.
    }

    void Update()
    {
        Dead();
        LevelUp();
        PullExpObjects();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentHp > 0)
        {
            currentHp--;
            // 여기에 히트 애니메이션 재생 로직 추가
        }
    }

    public void LevelUp()
    {
        if (currentExp >= maxExp)
        {
            isLevelUping = true;
            currentExp -= maxExp;
            level++;
            LevelUpEvent?.Invoke();
        }
    }


    public void PlayLevelUpEffect()
    {
        if (levelUpEffectCoroutine != null)
        {
            StopCoroutine(levelUpEffectCoroutine); // 이미 실행 중인 코루틴이 있다면 중단
        }
        levelUpEffectCoroutine = StartCoroutine(ActivateLevelUpEffect());
    }

    private IEnumerator ActivateLevelUpEffect()
    {
        levelUpEffect.SetActive(true); // 이펙트 활성화
        yield return new WaitForSeconds(effectTime); // 1초 기다림
        levelUpEffect.SetActive(false); // 이펙트 비활성화
    }


    void Dead()
    {
        if (currentHp <= 0)
        {
            // 사망 애니메이션 재생 로직 추가
            anim.SetTrigger("Die"); // 예시: 사망 애니메이션 재생
            gameObject.SetActive(false);
            // Time.timeScale = 0.01f; // 게임 멈춤 로직 대신 사망 UI 표시 로직 추가
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Magnet"))
        {
            // 'Exp' 태그가 붙은 모든 게임 오브젝트를 찾아 리스트에 추가합니다.
            expObjects.AddRange(GameObject.FindGameObjectsWithTag("Exp"));
            other.gameObject.SetActive(false);
        }

        if (other.CompareTag("Exp"))
        {
            expObjects.Remove(other.gameObject);

            currentExp += other.GetComponent<Exp>().expValue;
            other.gameObject.SetActive(false);
        }
    }

    private void PullExpObjects()
    {
        foreach (GameObject expObject in expObjects)
        {
            // 플레이어의 위치로 오브젝트를 부드럽게 이동시킵니다.
            expObject.transform.position = Vector3.MoveTowards(expObject.transform.position, transform.position, pullForce * Time.deltaTime);
        }
    }
}