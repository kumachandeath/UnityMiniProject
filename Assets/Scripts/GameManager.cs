using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static GameManager instance;

    [Header("Loading UI")]
    public GameObject loadingScreen;
    public Slider loadingBar;
    public TextMeshProUGUI loadingText;

    [Header("End Game UI")]
    public GameObject endGameScreen; // ���� �޽��� UI
    public TextMeshProUGUI endGameMessage; // ���� �޽��� �ؽ�Ʈ

    [Header("Title UI")]
    public GameObject startButton; // Ÿ��Ʋ ȭ���� ���� ��ư

    private bool isGameOver = false; // ���� ���� ���� Ȯ�ο�

    private void Awake()
    {
        // �̱��� ���� ����
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject); // �ߺ��� �ν��Ͻ��� ���� ��� �ı�
        }
    }

    // ���� ���� �� Ÿ��Ʋ ȭ�鿡�� ȣ��
    public void StartGame()
    {
        // Ÿ��Ʋ ȭ���� ���� ��ư ��Ȱ��ȭ
        if (startButton != null)
        {
            startButton.SetActive(false);
        }

        // ���� �ε� ����
        StartCoroutine(LoadMainGame());
    }

    // ���� ���� �񵿱� �ε�
    private IEnumerator LoadMainGame()
    {
        // �ε� UI Ȱ��ȭ
        loadingScreen.SetActive(true);

        // ���� ���� ���� �񵿱�� �ε�
        AsyncOperation operation = SceneManager.LoadSceneAsync("MainGameScene");

        // �ε� ���� ��Ȳ ������Ʈ
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.value = progress;
            loadingText.text = (progress * 100).ToString("F0") + "%";
            yield return null;
        }

        // �ε� �Ϸ� �� �ε� UI ��Ȱ��ȭ
        loadingScreen.SetActive(false);
    }

    // ���� ������ �÷��̾ �������� �� ȣ��
    public void PlayerReachedGoal()
    {
        isGameOver = true;
        endGameScreen.SetActive(true); // ���� �޽��� UI Ȱ��ȭ
        endGameMessage.text = "Congratulations! Press R to return to the Title Screen."; // �޽��� ����
    }

    private void Update()
    {
        // ���� ���� �� RŰ �Է����� Ÿ��Ʋ ȭ������ ���ư���
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            ReturnToTitleScreen();
        }
    }

    // Ÿ��Ʋ ȭ������ ���ư���
    private void ReturnToTitleScreen()
    {
        // Ÿ��Ʋ ������ �̵�
        SceneManager.LoadScene("TitleScene");

        // ���� ���� �ʱ�ȭ
        ResetGameState();
    }

    // ���� ���� �ʱ�ȭ �޼���
    private void ResetGameState()
    {
        // ���� �޽��� UI ��Ȱ��ȭ
        if (endGameScreen != null)
        {
            endGameScreen.SetActive(false);
        }

        // Ÿ��Ʋ ȭ���� ���� ��ư ��Ȱ��ȭ
        if (startButton != null)
        {
            startButton.SetActive(true);
        }

        // �ٸ� UI ��� �ʱ�ȭ
        if (loadingText != null)
        {
            loadingText.text = "";
        }

        isGameOver = false;
    }
}
