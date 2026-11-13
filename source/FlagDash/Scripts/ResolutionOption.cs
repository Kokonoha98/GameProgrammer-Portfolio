using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResolutionManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown screenModeDropdown; // 画面モード選択用Dropdownを取得

    [SerializeField]
    private TMP_Dropdown resolutionDropdown; // 解像度選択用Dropdownを取得

    private readonly List<Resolution> resolutions = new(); // 使用可能な解像度を保存

    private const string ScreenModeKey = "ScreenMode"; // 画面モードの保存キー
    private const string ResolutionKey = "ResolutionIndex"; // 解像度の保存キー

    // 画面設定を初期化
    private void Awake()
    {
#if UNITY_ANDROID || UNITY_IOS

        if (screenModeDropdown != null)
            screenModeDropdown.gameObject.SetActive(false);

        // モバイルでは解像度変更しない
        if (resolutionDropdown != null)
            resolutionDropdown.gameObject.SetActive(false);

        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.orientation = ScreenOrientation.AutoRotation; // モバイル画面を横向きに設定

#else
        if (screenModeDropdown == null)
        {
            Debug.LogError("ScreenModeDropdownが設定されていません。");
            return;
        }

        if (resolutionDropdown == null)
        {
            Debug.LogError("ResolutionDropdownが設定されていません。");
            return;
        }

        InitializeScreenMode(); // 画面モード設定を初期化
        InitializeResolutionDropdown(); // 解像度設定を初期化
#endif
    }

#if !UNITY_ANDROID && !UNITY_IOS

    //画面モード初期化
    private void InitializeScreenMode()
    {
        screenModeDropdown.ClearOptions(); // Dropdownの項目を削除

        screenModeDropdown.AddOptions(new List<string> // Dropdownに項目を追加
    { 
        "Full Screen",
        "Window"
    });

        int saved = PlayerPrefs.GetInt(ScreenModeKey, 0); // 保存した設定を読み込む

        screenModeDropdown.SetValueWithoutNotify(saved); // Dropdownの選択状態を設定

        screenModeDropdown.onValueChanged.AddListener(ApplyScreenMode); // Dropdown変更時の処理を登録
    }

    //解像度一覧作成
    private void InitializeResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();

        resolutions.Clear(); // 解像度リストを初期化

        List<string> options = new();
        HashSet<string> added = new();

        Resolution[] all = Screen.resolutions;

        int currentIndex = 0;

        foreach (Resolution res in all)
        {
            // 16:9・16:10のみ
            if (!IsSupportedAspect(res.width, res.height)) // 対応するアスペクト比か確認
                continue;

            string key = $"{res.width}x{res.height}";

            if (added.Contains(key)) // 同じ解像度があるか確認
                continue;

            added.Add(key);

            resolutions.Add(res); // 解像度をリストに追加
        }

        // 解像度が高い順に並べる
        resolutions.Sort((a, b) =>
        {
            int areaA = a.width * a.height;
            int areaB = b.width * b.height;
            return areaB.CompareTo(areaA);
        });

        for (int i = 0; i < resolutions.Count; i++)
        {
            Resolution res = resolutions[i];

            options.Add($"{res.width} × {res.height}"); // 解像度をDropdown表示用に追加

            if (res.width == Screen.width &&
                res.height == Screen.height)
            {
                currentIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        int saved =
            PlayerPrefs.GetInt(ResolutionKey, currentIndex);

        if (saved < 0 || saved >= resolutions.Count)
            saved = currentIndex;

        resolutionDropdown.SetValueWithoutNotify(saved);

        // 画面モードを適用
        ApplyScreenMode(
            PlayerPrefs.GetInt(ScreenModeKey, 0));

        ApplyResolution(saved); // 解像度を適用

        resolutionDropdown.onValueChanged.AddListener(ApplyResolution);
    }

    // 16:9または16:10か確認
    private bool IsSupportedAspect(int width, int height)
    {
        float ratio = (float)width / height;

        return Mathf.Abs(ratio - (16f / 9f)) < 0.02f ||
               Mathf.Abs(ratio - (16f / 10f)) < 0.02f;
    }

    // 画面モード変更
    public void ApplyScreenMode(int index)
    {
        if (resolutions.Count == 0)
            return;

        FullScreenMode mode =
            index == 0
            ? FullScreenMode.FullScreenWindow
            : FullScreenMode.Windowed;

        Resolution res = resolutions[resolutionDropdown.value];

        // 画面サイズ・モードを変更
        Screen.SetResolution(
            res.width,
            res.height,
            mode,
            res.refreshRateRatio);

        // 設定を保存
        PlayerPrefs.SetInt(ScreenModeKey, index);
        PlayerPrefs.Save(); // 設定保存を確定
    }

    // 解像度変更
    public void ApplyResolution(int index)
    {
        if (index < 0 || index >= resolutions.Count)
            return;

        Resolution res = resolutions[index];

        Screen.SetResolution(
            res.width,
            res.height,
            Screen.fullScreenMode,
            res.refreshRateRatio);

        PlayerPrefs.SetInt(ResolutionKey, index);
        PlayerPrefs.Save();
    }

#endif
}
