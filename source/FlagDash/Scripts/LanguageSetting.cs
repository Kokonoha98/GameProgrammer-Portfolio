using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageSetting : MonoBehaviour
{
    // 言語を日本語に変更
    public void Japanese()
    {
        // 使用する言語を設定
        LocalizationSettings.SelectedLocale =
            LocalizationSettings.AvailableLocales.GetLocale("ja"); // 日本語のロケールを取得
    }

    // 言語を英語に変更
    public void English()
    {
        // 使用する言語を設定
        LocalizationSettings.SelectedLocale =
            LocalizationSettings.AvailableLocales.GetLocale("en"); // 英語のロケールを取得
    }
}
