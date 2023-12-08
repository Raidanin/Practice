using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public class TouchJoystickController : MonoBehaviour
{
    public GameObject player;
    Player playerScript;
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 10.0f;
    public Image joystickBackground;
    public Image joystickHandle;
    private Animator playerAnim;
    [SerializeField] GameObject stick;
    private OnScreenStick screenStick;

    private Vector2 clickStartPosition;
    private Vector2 clickDirection;

    private void Start()
    {
        playerAnim = GetComponent<Animator>();
        playerScript = player.GetComponent<Player>();
        screenStick = stick.GetComponent<OnScreenStick>();    
    }
    private void Update()
    {
        if (playerScript.isLevelUping == false)
        {

            if (Input.GetMouseButtonDown(0))
            {
                clickStartPosition = Input.mousePosition;
                joystickBackground.gameObject.SetActive(true);
                joystickBackground.transform.position = clickStartPosition;
                joystickHandle.transform.position = clickStartPosition;
            }
            if (Input.GetMouseButton(0))
            {
                Vector2 clickPosition = Input.mousePosition;
                clickDirection = clickPosition - clickStartPosition;
                clickDirection.Normalize();

                Vector3 movement = new Vector3(clickDirection.x * moveSpeed * Time.deltaTime, 0, clickDirection.y * moveSpeed * Time.deltaTime);
                player.transform.position += movement;

                if (movement != Vector3.zero)
                {
                    playerAnim.SetBool("isMoving", true);
                    Quaternion targetRotation = Quaternion.LookRotation(movement);
                    player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                }
                else
                    playerAnim.SetBool("isMoving", false);

                joystickHandle.transform.position = clickStartPosition + clickDirection * screenStick.movementRange; //
            }
            if (Input.GetMouseButtonUp(0))
            {
                playerAnim.SetBool("isMoving", false);
                joystickBackground.gameObject.SetActive(false);
                joystickHandle.transform.position = clickStartPosition;
            }
        }
    }
}