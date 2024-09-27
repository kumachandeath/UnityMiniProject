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
        StartCoroutine(LoadMainGame());
    }

    // 메인 게임 비동기 로딩
    private IEnumerator LoadMainGame()
    {
        // 로딩 UI 활성화
        loadingScreen.SetActive(true);
        Debug.Log("Loading UI 활성화됨.");

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

        // 로딩이 끝나면 로딩 UI 비활성화
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }
}