using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameManager instance;

    [Header("Loading UI")]
    public GameObject loadingScreen;
    public Slider loadingBar;
    public TextMeshProUGUI loadingText;

    [Header("End Game UI")]
    public GameObject endGameScreen; // 골인 메시지 UI
    public TextMeshProUGUI endGameMessage; // 골인 메시지 텍스트

    [Header("Title UI")]
    public GameObject startButton; // 타이틀 화면의 시작 버튼

    private bool isGameOver = false; // 게임 종료 상태 확인용

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 중복된 인스턴스가 있을 경우 파괴
        }
    }

    // 게임 시작 시 타이틀 화면에서 호출
    public void StartGame()
    {
        // 타이틀 화면의 시작 버튼 비활성화
        if (startButton != null)
        {
            startButton.SetActive(false);
        }

        // 게임 로딩 시작
        StartCoroutine(LoadMainGame());
    }

    // 메인 게임 비동기 로딩
    private IEnumerator LoadMainGame()
    {
        // 로딩 UI 활성화
        loadingScreen.SetActive(true);

        // 메인 게임 씬을 비동기로 로드
        AsyncOperation operation = SceneManager.LoadSceneAsync("MainGameScene");

        // 로딩 진행 상황 업데이트
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.value = progress;
            loadingText.text = (progress * 100).ToString("F0") + "%";
            yield return null;
        }

        // 로딩 완료 후 로딩 UI 비활성화
        loadingScreen.SetActive(false);
    }

    // 골인 지점에 플레이어가 도착했을 때 호출
    public void PlayerReachedGoal()
    {
        isGameOver = true;
        endGameScreen.SetActive(true); // 골인 메시지 UI 활성화
        endGameMessage.text = "Congratulations! Press R to return to the Title Screen."; // 메시지 설정
    }

    private void Update()
    {
        // 게임 종료 후 R키 입력으로 타이틀 화면으로 돌아가기
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            ReturnToTitleScreen();
        }
    }

    // 타이틀 화면으로 돌아가기
    private void ReturnToTitleScreen()
    {
        // 타이틀 씬으로 이동
        SceneManager.LoadScene("TitleScene");

        // 게임 상태 초기화
        ResetGameState();
    }

    // 게임 상태 초기화 메서드
    private void ResetGameState()
    {
        // 골인 메시지 UI 비활성화
        if (endGameScreen != null)
        {
            endGameScreen.SetActive(false);
        }

        // 타이틀 화면의 시작 버튼 재활성화
        if (startButton != null)
        {
            startButton.SetActive(true);
        }

        // 다른 UI 요소 초기화
        if (loadingText != null)
        {
            loadingText.text = "";
        }

        isGameOver = false;
    }
}
