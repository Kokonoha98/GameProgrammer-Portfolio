using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // GameManagerをシングルトン化

    [Header("参照")]
    public FlagSpawner flagSpawner;
    public GameObject scoreInputPanel;
    public GameObject notScoreInputPanel;
    public GameObject nameEntryPanel;
    public GameObject continuePanel;
    public GameObject pausePanel;

    public RankingAPI rankingAPI;

    [Header("モバイル")]
    public MobileJoystick mobileJoystick;

    public PlayerSpawner playerSpawner;

    [Header("プレイヤー")]
    private PlayerInput playerInput;
    private StarterAssetsInputs starterInputs;

    private float timer;
    private bool isPlaying;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 時間の進行速度を通常に設定
        Time.timeScale = 1f;

        // 各UIを非表示
        pausePanel.SetActive(false);
        scoreInputPanel.SetActive(false);
        notScoreInputPanel.SetActive(false);
        nameEntryPanel.SetActive(false);
        continuePanel.SetActive(false);
    }

    private void Update()
    {
        // Escapeキーの入力を確認
        if (Keyboard.current.escapeKey.wasPressedThisFrame && isPlaying)
        {
            // ゲームを一時停止
            Pause();
        }

        if (isPlaying)
        {
            // timer += Time.deltaTime;
            timer += Time.deltaTime;
        }
    }

    // プレイヤーの入力を取得してゲームを開始
    public void InitializePlayer()
    {
        playerInput = PlayerManager.Instance.PlayerInput;
        starterInputs = PlayerManager.Instance.StarterInputs;

        Restart();
    }

    // ゴール時の処理
    public void Goal()
    {
        Debug.Log("Goal!");

        // プレイ状態を終了
        isPlaying = false;

        // UIモードへ切り替え
        SetUIMode();

        int score = CalculateScore(timer); // タイムからスコアを計算

        Debug.Log("Score = " + score);

        ScoreManager.Instance.SetCurrentScore(score); // 現在のスコアを設定

        Debug.Log("HighScore Check");

        // MySQLを使ってランキング入りを確認
        StartCoroutine(rankingAPI.CheckScore(score, OnScoreCheckComplete));
    }

    // ゲームを一時停止
    public void Pause()
    {
        // ゲーム時間を停止
        Time.timeScale = 0f;

        // ポーズUIを表示
        pausePanel.SetActive(true);

        SetUIMode();
    }

    // ゲームを再開
    public void Resume()
    {
        // ゲーム時間を再開
        Time.timeScale = 1f;

        // ポーズUIを非表示
        pausePanel.SetActive(false);

        SetPlayMode();
    }

    // ゲームをリスタート
    public void Restart()
    {
        timer = 0f; // タイマーをリセット
        isPlaying = true; // プレイ状態を開始

        // 旗をスポーン
        flagSpawner.SpawnFlag();

        SetPlayMode();
    }

    // 次のステージへ進む
    public void OnClickContinue()
    {
        continuePanel.SetActive(false);

        StageManager.Instance.LoadNextStage();
    }

    // プレイ操作モードに切り替え
    public void SetPlayMode()
    {
        // プレイ開始時に入力状態をリセット
        starterInputs.move = Vector2.zero;
        starterInputs.look = Vector2.zero;
        starterInputs.jump = false;
        starterInputs.jumpHeld = false;
        starterInputs.sprint = false;
        starterInputs.mobileJumpRequested = false;

        // モバイル側の入力状態もリセット
        if (mobileJoystick != null)
        {
            mobileJoystick.ResetMobileInput();
        }

        Cursor.lockState = CursorLockMode.Locked; // カーソルを画面中央に固定
        Cursor.visible = false; // カーソル非表示

        playerInput.enabled = true; // プレイヤー入力を有効化
    }

    // UI操作モードに切り替え
    public void SetUIMode()
    {
        // 入力値をリセット
        starterInputs.move = Vector2.zero; // 移動入力をリセット
        starterInputs.look = Vector2.zero; // 視点入力をリセット
        starterInputs.jump = false; // ジャンプ入力をリセット
        starterInputs.sprint = false; // ダッシュ入力をリセット

        // プレイヤー入力を無効化
        playerInput.enabled = false;

        Cursor.lockState = CursorLockMode.None; // カーソルの固定を解除
        Cursor.visible = true; // カーソルを表示
    }

    // プレイ時間からスコアを計算
    private int CalculateScore(float time)
    {
        // スコアが0未満にならないようにする
        return Mathf.Max(0, 10000 - Mathf.RoundToInt(time * 100));
    }

    private void OnScoreCheckComplete(bool isHighScore)
    {
        if (isHighScore)
        {
            // ランキング入り
            scoreInputPanel.SetActive(true);
        }
        else
        {
            // ランキング外
            notScoreInputPanel.SetActive(true);
        }
    }


    // 現在のタイマーを取得
    public float GetTime()
    {
        return timer;
    }
}
