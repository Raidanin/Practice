using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public GameObject closestEnemy;

    // Update is called once per frame
    void Update()
    {
        closestEnemy = FindClosestEnemyWithTag("Enemy");
    }

    GameObject FindClosestEnemyWithTag(string tag)
    {
        GameObject[] enemies;
        enemies = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (GameObject enemy in enemies)
        {
            Vector3 directionToEnemy = enemy.transform.position - currentPosition;
            float distanceSqrToEnemy = directionToEnemy.sqrMagnitude;

            if (distanceSqrToEnemy < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqrToEnemy;
                closest = enemy;
            }
        }

        return closest;
    }
}
