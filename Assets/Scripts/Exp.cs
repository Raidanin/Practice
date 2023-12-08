using UnityEngine;

public class Exp : MonoBehaviour
{
    public float expValue = 1f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(Vector3.up, 90f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
            GameObject thisGameObject = this.gameObject;
            GameObject otherGameObject = other.gameObject;
        if (other.CompareTag("Exp") && this.gameObject.activeInHierarchy)
        {

            Exp otherExp = other.GetComponent<Exp>();

            // expValue 더하기
            expValue += otherExp.expValue;

            // 자신이 아직 활성 상태라면 다른 객체를 비활성화
            otherGameObject.SetActive(false);
        }
    }
}
 