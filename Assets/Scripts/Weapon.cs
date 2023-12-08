using System;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject weaponprefab;
    public WeaponType weaponType;
    public float attackSpeed = 1f;
    public float damage;
    public GameObject[] bulletPrefab;
    public GameObject player;
    private Scanner scanner;
    public Transform firepos;
    public int weaponLevel = 1;
    private bool isCrossbowCoroutineRunning = false;  // 플래그 추가
    private bool isShurikenCoroutineRunning = false;  // 플래그 추가
    private bool isSwordAttack = false;  // 플래그 추가
    private int swordCount = 3;
    public GameObject[] swords;
    Player playerScript;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<Player>();
        scanner = player.GetComponent<Scanner>();
        playerScript.LevelUpEvent += HandleLevelUpEvent;

    }

    void Update()
    {
        AttackBaseOnWeaponType();

        int playerLevel = player.GetComponent<Player>().level;
        attackSpeed = Mathf.Max(0.2f, 1f - (playerLevel * 0.05f));

    }
    void HandleLevelUpEvent()
    {
        AddSword();
    }

    void AddSword()
    {
        GameObject newSword = Instantiate(bulletPrefab[0], player.transform.position, Quaternion.identity);
        Array.Resize(ref swords, swords.Length + 1);
        swords[swords.Length - 1] = newSword;

        // Bullets 스크립트의 검 정보도 업데이트
        Bullets bulletsScript = FindObjectOfType<Bullets>();
        bulletsScript.UpdateSwords();
    }

    private void LookEnemy()
    {
        if (scanner.closestEnemy != null)
        {
            Vector3 targetDirection = scanner.closestEnemy.transform.position - transform.position;
            targetDirection.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = targetRotation * Quaternion.Euler(-90f, 180f, 0);
        }
    }

    void AttackBaseOnWeaponType()
    {
        switch (weaponType)
        {
            case WeaponType.crossbow:
                CrossbowAttack();
                LookEnemy();
                break;
            case WeaponType.sword:
                SwordAttack();
                break;
            case WeaponType.shuriken:
                ShurikenAttack();
                break;
        }
    }

    void CrossbowAttack()
    {
        if (!isCrossbowCoroutineRunning)  // 플래그 체크
        {
            StartCoroutine(StartCrossbowAttack());
        }
    }

    void SwordAttack()
    {
        if (!isSwordAttack)
        {
            swords = new GameObject[swordCount];

            for (int i = 0; i < swordCount; i++)
            {
                swords[i] = Instantiate(bulletPrefab[0], player.transform.position, Quaternion.identity);
                Bullets bulletsScript = FindObjectOfType<Bullets>();
                bulletsScript.UpdateSwords();
            }

            isSwordAttack = true;
        }
    }

    IEnumerator StartCrossbowAttack()
    {
        isCrossbowCoroutineRunning = true;  // 플래그 설정

        while (true)
        {
            Instantiate(bulletPrefab[0], firepos.transform.position, firepos.rotation);
            yield return new WaitForSeconds(attackSpeed);
        }

    }

    void ShurikenAttack()
    {
        if (!isShurikenCoroutineRunning)  // 플래그 체크
        {
            StartCoroutine(StartShurikenAttack());
        }
    }

    IEnumerator StartShurikenAttack()
    {
        isShurikenCoroutineRunning = true;  // 플래그 설정

        while (true)
        {
            Instantiate(bulletPrefab[0], firepos.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(attackSpeed / 5);
        }
    }

    public enum WeaponType
    {
        crossbow,
        sword,
        shuriken,
    }
}