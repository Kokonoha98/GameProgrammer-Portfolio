using UnityEngine;
using UnityEngine.Rendering;

public class OptionManeger : MonoBehaviour
{
    // 各オプション画面を取得
    public GameObject OptionPanel;
    public GameObject VolumeControlPanel;
    public GameObject KeyConfiguPanel;
    public GameObject ChangeResolutionPanel;
    public GameObject LanguageSettingsPanel;

    [SerializeField] private GameObject keyConfigButton; // キー設定ボタンを取得
    [SerializeField] private GameObject resolutionButton; // 解像度設定ボタンを取得

    //オプション画面を初期化
    void Start()
    {
        //各オプション画面を非表示
        OptionPanel.SetActive(false);
        VolumeControlPanel.SetActive(false);
        KeyConfiguPanel.SetActive(false);
        ChangeResolutionPanel.SetActive(false);
        LanguageSettingsPanel.SetActive(false);

        // モバイル端末か確認
        if (Application.isMobilePlatform)
        {
            keyConfigButton.SetActive(false); // モバイルではキー設定ボタンを非表示
            resolutionButton.SetActive(false); // モバイルでは解像度設定ボタンを非表示
        }
    }

    void Update()
    {

    }

    // オプション画面を表示
    public void OptionTitle()
    {
        OptionPanel.SetActive(true);
        VolumeControlPanel.SetActive(false);
        KeyConfiguPanel.SetActive(false);
        ChangeResolutionPanel.SetActive(false);
        LanguageSettingsPanel.SetActive(false);
    }

    // オプション画面を閉じる
    public void OptionClose()
    {
        OptionPanel.SetActive(false);
        VolumeControlPanel.SetActive(false);
        KeyConfiguPanel.SetActive(false);
        ChangeResolutionPanel.SetActive(false);
        LanguageSettingsPanel.SetActive(false);
    }

    // 音量設定画面を表示
    public void VolumeControlOpen()
    {
        OptionPanel.SetActive(false);
        VolumeControlPanel.SetActive(true);
        KeyConfiguPanel.SetActive(false);
        ChangeResolutionPanel.SetActive(false);
        LanguageSettingsPanel.SetActive(false);
    }

    // キー設定画面を表示
    public void KeyConfiguOpen()
    {
        if (Application.isMobilePlatform)
            return;

        OptionPanel.SetActive(false);
        VolumeControlPanel.SetActive(false);
        KeyConfiguPanel.SetActive(true);
        ChangeResolutionPanel.SetActive(false);
        LanguageSettingsPanel.SetActive(false);
    }

    // キー設定画面を閉じる
    public void KeyConfiguOpenClose()
    {
        KeyConfigManager.Instance.CancelRebind(); // キー変更処理をキャンセル

        OptionPanel.SetActive(true);
        VolumeControlPanel.SetActive(false);
        KeyConfiguPanel.SetActive(false);
        ChangeResolutionPanel.SetActive(false);
        LanguageSettingsPanel.SetActive(false);
    }

    // 解像度設定画面を表示
    public void ChangeResolutionOpen()
    {
        if (Application.isMobilePlatform)
            return;

        OptionPanel.SetActive(false);
        VolumeControlPanel.SetActive(false);
        KeyConfiguPanel.SetActive(false);
        ChangeResolutionPanel.SetActive(true);
        LanguageSettingsPanel.SetActive(false);
    }

    // 言語設定画面を表示
    public void LanguageSettingOpen()
    {
        OptionPanel.SetActive(false);
        VolumeControlPanel.SetActive(false);
        KeyConfiguPanel.SetActive(false);
        ChangeResolutionPanel.SetActive(false);
        LanguageSettingsPanel.SetActive(true);
    }

}
