using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

public class KeyConfigManager : MonoBehaviour
{
    public static KeyConfigManager Instance; // KeyConfigManagerをシングルトン化

    // InputActionAssetを取得
    [Header("Input Actions")]
    [SerializeField] private InputActionAsset inputActions;

    // 各キー設定の表示テキストを取得
    [Header("Texts")]
    [SerializeField] private TMP_Text upText;
    [SerializeField] private TMP_Text downText;
    [SerializeField] private TMP_Text leftText;
    [SerializeField] private TMP_Text rightText;
    [SerializeField] private TMP_Text jumpText;
    [SerializeField] private TMP_Text sprintText;

    private const string SaveKey = "KeyBindings"; // キー設定の保存キーを設定

    // 移動キーのバインド番号を設定
    private const int UpIndex = 1;
    private const int DownIndex = 2;
    private const int LeftIndex = 3;
    private const int RightIndex = 4;

    private bool isRebinding; // リバインド中かを管理

    private RebindingOperation currentOperation; // 現在のリバインド処理を保存

    private string oldOverride; // 変更前のキー設定を保存
    private bool hadOverride; // 変更前に上書き設定があったか保存

    private InputAction currentAction; // 現在リバインド中のActionを保存
    private int currentBindingIndex; // 現在リバインド中のバインド番号を保存

    private void Awake()
    {
        Instance = this;
    }

    // キー設定を読み込み表示を更新
    private void Start()
    {
        LoadBindings();
        UpdateTexts();
    }

    // 移動キーの変更を開始
    public void RebindUp()
    {
        if(isRebinding)
        CancelRebind();

        EventSystem.current.SetSelectedGameObject(null);
        StartRebind("Move", UpIndex, upText);
    }

    public void RebindDown()
    {
        if (isRebinding)
            CancelRebind();

        EventSystem.current.SetSelectedGameObject(null);
        StartRebind("Move", DownIndex, downText);
    }

    public void RebindLeft()
    {
        if (isRebinding)
            CancelRebind();

        EventSystem.current.SetSelectedGameObject(null);
        StartRebind("Move", LeftIndex, leftText);
    }

    public void RebindRight()
    {
        if (isRebinding)
            CancelRebind();

        EventSystem.current.SetSelectedGameObject(null);
        StartRebind("Move", RightIndex, rightText);
    }

    // ジャンプキーの変更を開始
    public void RebindJump()
    {
        if (isRebinding)
            CancelRebind();

        EventSystem.current.SetSelectedGameObject(null);
        StartRebind("Jump", 0, jumpText);
    }

    // ダッシュキーの変更を開始
    public void RebindSprint()
    {
        if (isRebinding)
            CancelRebind();

        EventSystem.current.SetSelectedGameObject(null);
        StartRebind("Sprint", 0, sprintText);
    }

    // キー変更処理を開始
    private void StartRebind(string actionName, int bindingIndex, TMP_Text targetText)
    {

        // ボタンの選択状態を解除
        EventSystem.current.SetSelectedGameObject(null);

        if (isRebinding)
            return;

        isRebinding = true;

        currentAction = inputActions.FindAction(actionName); // 変更対象のActionを取得

        currentBindingIndex = bindingIndex; // 変更対象のバインド番号を保存

        // 現在のキーを保存（重複時に戻すため）
        var binding = currentAction.bindings[bindingIndex];
        oldOverride = binding.overridePath; // 変更前のキー設定を保存
        hadOverride = binding.hasOverrides; // 変更前の上書き設定を確認

        targetText.text = "Press Any Key..."; // キー入力待ちの表示

        currentOperation = currentAction
            .PerformInteractiveRebinding(bindingIndex) // キー入力によるリバインドを開始
            .WithControlsHavingToMatchPath("<Keyboard>") // キーボード入力に限定
            .WithControlsExcluding("<Mouse>") // マウス入力を除外
            .WithControlsExcluding("<Pointer>") // ポインター入力を除外
            .WithCancelingThrough("<Keyboard>/escape") // Escapeキーでキャンセル
            .OnComplete(operation => // リバインド完了時の処理
            {
                operation.Dispose();

                //キーの重複を確認
                if (HasDuplicateBinding(currentAction, bindingIndex))
                {
                    Debug.Log("キーが重複しています。");
                    if (hadOverride)
                    {
                        currentAction.ApplyBindingOverride(bindingIndex, oldOverride); // 変更前のキー設定に戻す
                    }
                    else
                    {
                        currentAction.RemoveBindingOverride(bindingIndex); // キーの上書き設定を解除
                    }
                }
                else
                {
                    // キー設定を保存
                    SaveBindings();
                }
                // リバインドを終了
                EndRebind();
            })

            // リバインドキャンセル時の処理
            .OnCancel(operation =>
            {
                operation.Dispose();

                RestoreOldBinding(); // 変更前のキー設定に戻す

                EndRebind();
            });

        currentOperation.Start(); // リバインド処理を開始
    }

    // リバインド終了処理
    private void EndRebind()
    {
        // リバインド状態を解除
        isRebinding = false;

        // リバインド処理を解除
        currentOperation = null;

        EventSystem.current.SetSelectedGameObject(null);

        // キー表示を更新
        UpdateTexts();
    }

    // リバインドをキャンセル
    public void CancelRebind()
    {
        if (!isRebinding)
            return;

        currentOperation.Cancel();
    }

    // 他のActionとキーが重複しているか確認
    private bool HasDuplicateBinding(InputAction currentAction, int bindingIndex)
    {
        string newPath = currentAction.bindings[bindingIndex].effectivePath; // 現在のキー設定を取得

        // 全Actionを確認
        foreach (InputAction action in inputActions)
        {
            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (action == currentAction && i == bindingIndex)
                    continue;

                // Compositeバインドを除外
                if (action.bindings[i].isComposite)
                    continue;

                if (action.bindings[i].effectivePath == newPath)
                    return true;
            }
        }

        return false;
    }

    private void SaveBindings()
    {
        string json = inputActions.SaveBindingOverridesAsJson(); // キー設定を保存

        PlayerPrefs.SetString(SaveKey, json); // キー設定をJSON化
        PlayerPrefs.Save();
    }

    //保存したキー設定を読み込み
    private void LoadBindings()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
            return;

        // JSONからキー設定を復元
        inputActions.LoadBindingOverridesFromJson(
            PlayerPrefs.GetString(SaveKey));
    }

    //キー設定を初期状態に戻す
    public void ResetBindings()
    {
        inputActions.RemoveAllBindingOverrides(); // 全キー設定の変更を解除

        PlayerPrefs.DeleteKey(SaveKey); // 保存したキー設定を削除
        PlayerPrefs.Save();

        UpdateTexts();
    }

    // 変更前のキー設定を復元
    private void RestoreOldBinding()
    {
        if (hadOverride)
        {
            currentAction.ApplyBindingOverride(
                currentBindingIndex,
                oldOverride);
        }
        else
        {
            currentAction.RemoveBindingOverride(
                currentBindingIndex);
        }
    }

    //現在のキー設定をUIに表示
    private void UpdateTexts()
    {
        InputAction move = inputActions.FindAction("Move");

        // キー名を表示用文字列に変換
        upText.text = move.bindings[UpIndex].ToDisplayString();
        downText.text = move.bindings[DownIndex].ToDisplayString();
        leftText.text = move.bindings[LeftIndex].ToDisplayString();
        rightText.text = move.bindings[RightIndex].ToDisplayString();

        jumpText.text =
            inputActions.FindAction("Jump").GetBindingDisplayString(); //キー設定を表示用文字列に変換

        sprintText.text =
            inputActions.FindAction("Sprint").GetBindingDisplayString(); // キー設定を表示用文字列に変換
    }
}
