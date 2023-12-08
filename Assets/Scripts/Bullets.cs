using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    public BulletType bullettype;
    private GameObject player;
    private Scanner scanner;
    public float bulletSpeed = 10;
    private Vector3 enemyDirection;
    private Vector3 enemyPos;
    private float[] swordAngles;

    public float disableDelay = 1f;
    private Camera mainCamera;
    public float pushForce = 10f;
    private GameObject[] swords;
    [SerializeField] private AnimationCurve positionCurve;
    public AnimationCurve noiseCurve;
    public float yOffset = 1f;
    public Vector2 minNoise = new Vector2(-3f, -0.25f);
    public Vector2 maxNoise = new Vector2(3f, 1f);

    private Coroutine homingCoroutine;
    private GameObject initialShurikenTarget;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        scanner = player.GetComponent<Scanner>();
        mainCamera = Camera.main;
        swords = GameObject.FindGameObjectsWithTag("Sword");
        swordAngles = new float[swords.Length];
        initialShurikenTarget = scanner.closestEnemy;

        for (int i = 0; i < swords.Length; i++)
        {
            swordAngles[i] = i * (360.0f / swords.Length);
        }

        if (scanner.closestEnemy != null)
        {
            enemyPos = new Vector3(scanner.closestEnemy.transform.position.x, transform.position.y, scanner.closestEnemy.transform.position.z);
            enemyDirection = (enemyPos - transform.position).normalized;
        }
        else
        {
            enemyDirection = player.transform.forward;
            enemyDirection.y = 0f;
        }
        if (bullettype == BulletType.shuriken)
        {
            if (initialShurikenTarget == null)
            {
                DisableObject();
            }
            else
            {
                StartCoroutine(ShurikenMove());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (bullettype != BulletType.shuriken)
        {
            FlyBaseBulletType();
        }
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);
        DisableBullet(viewportPosition);
    }

    private void DisableBullet(Vector3 viewportPosition)
    {
        if (viewportPosition.x < 0 || viewportPosition.x > 1 || viewportPosition.y < 0 || viewportPosition.y > 1)
        {
            Invoke("DisableObject", disableDelay);
        }
    }

    
    void DisableObject()
    {
        this.gameObject.SetActive(false);
    }

    void FlyBaseBulletType()
    {
        switch (bullettype)
        {
            case BulletType.crossbowArrow:
                CrossBowArrowMove();
                break;
            case BulletType.sword:
                SwordMove();
                break;


        }
    }

    void CrossBowArrowMove()
    {
        transform.Translate(enemyDirection * bulletSpeed * Time.deltaTime, Space.World);
        Quaternion rotation = Quaternion.LookRotation(enemyDirection);
        transform.rotation = rotation * Quaternion.Euler(90f, 0, 0);
    }

    void BulletHit(Collider other)
    {
        Rigidbody otherRigidbody = other.gameObject.GetComponent<Rigidbody>();
        if (otherRigidbody != null)
        {
            Vector3 pushDirection = (other.transform.position - player.transform.position).normalized;
            otherRigidbody.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }

        EnemyClass enemyScript = other.gameObject.GetComponent<EnemyClass>();
        if (enemyScript != null)
        {
            enemyScript.OnHit();
        }
    }

    void SwordMove()
    {
        float distanceFromPlayer = 1.5f;
        float rotationSpeed = 90.0f;

        for (int i = 0; i < swords.Length; i++)
        {
            swordAngles[i] += rotationSpeed * Time.deltaTime;
            Vector3 offset = new Vector3(Mathf.Sin(swordAngles[i] * Mathf.Deg2Rad), 1, Mathf.Cos(swordAngles[i] * Mathf.Deg2Rad)) * distanceFromPlayer;
            swords[i].transform.position = player.transform.position + offset;
            swords[i].transform.LookAt(player.transform.position);
            swords[i].transform.localRotation *= Quaternion.Euler(-135, 0, 0);
        }
    }

    IEnumerator ShurikenMove()
    {
        if (initialShurikenTarget == null)
        {
            DisableObject();
            yield break; // 코루틴 종료
        }
        positionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        Vector3 startPosition = transform.position;
        Vector2 noise = new Vector2(
            Random.Range(minNoise.x, maxNoise.x),
            Random.Range(minNoise.y, maxNoise.y)
        );
        float time = 0f;
        float rotationSpeed = 720f; // 초당 회전 속도 (도 단위)
        float rotation = 0f; // 초기 각도

        while (time < 1f)
        {
            if (initialShurikenTarget == null || !initialShurikenTarget.activeInHierarchy)
            {
                DisableObject();
                yield break; // 코루틴 종료
            }

            Vector3 targetPosition = initialShurikenTarget.transform.position + new Vector3(0, yOffset, 0);
            Vector3 direction = (targetPosition - startPosition).normalized;
            Vector3 horizontalNoiseDirection = Vector3.Cross(direction, Vector3.up).normalized;

            float noisePosition = noiseCurve.Evaluate(time);
            Vector3 noiseVector = new Vector3(
                horizontalNoiseDirection.x * noisePosition * noise.x,
                noise.y * noisePosition,
                horizontalNoiseDirection.z * noisePosition * noise.x
            );

            transform.position = Vector3.Lerp(startPosition, targetPosition, positionCurve.Evaluate(time)) + noiseVector;

            rotation += rotationSpeed * Time.deltaTime; // 회전 각도 누적
            transform.rotation = Quaternion.Euler(0, rotation, 90); // 누적된 회전 각도 적용

            time += Time.deltaTime * bulletSpeed;

            yield return null;
        }

        DisableObject();
    }

    private void OnTriggerEnter(Collider other)
    {
        BulletHit(other);
    }

    public void UpdateSwords()
    {
        swords = GameObject.FindGameObjectsWithTag("Sword");
        swordAngles = new float[swords.Length];
        for (int i = 0; i < swords.Length; i++)
        {
            swordAngles[i] = i * (360.0f / swords.Length);
        }
    }

    public enum BulletType
    {
        crossbowArrow,
        sword,
        shuriken,
    }
}