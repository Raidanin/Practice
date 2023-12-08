using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider exp;
    private GameObject player;
    private Player playerScripts;
    public Button uiButton;
    [SerializeField] GameObject levelUpCardUI;




    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScripts = player.GetComponent<Player>();

        uiButton.onClick.AddListener(OnRestartButtonClicked);
        uiButton.gameObject.SetActive(false);

        Button[] buttons = levelUpCardUI.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(CloseCardUI);
        }

    }

    // Update is called once per frame
    public void Update()
    {
        exp.value = playerScripts.currentExp / playerScripts.maxExp;

        if (!player.gameObject.activeInHierarchy)
        {
            uiButton.gameObject.SetActive(true);
        }

        if (playerScripts.isLevelUping)
        {
            levelUpCardUI.SetActive(true);
            Time.timeScale = 0f;
        }

        print(playerScripts.isLevelUping);
    }

    public void OnRestartButtonClicked()
    {
        // 씬을 다시 로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // 시간 스케일을 다시 1로 설정
        Time.timeScale = 1f;
    }

    public void CloseCardUI()
    {
        levelUpCardUI.SetActive(false); 
        playerScripts.isLevelUping = false;
        Time.timeScale = 1f;

        playerScripts.PlayLevelUpEffect(); 
    }
}
