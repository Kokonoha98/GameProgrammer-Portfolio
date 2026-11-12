using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioOptionController : MonoBehaviour
{
    public AudioMixer mixer; // AudioMixerを取得

    public Slider bgmSlider; // BGM音量スライダーを取得
    public Slider seSlider; // SE音量スライダーを取得

    // 音量設定を初期化
    private void Start()
    {
        // 保存された音量を読み込む
        float bgm = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float se = PlayerPrefs.GetFloat("SEVolume", 1f);

        // スライダーに音量を反映
        bgmSlider.value = bgm; // BGMスライダーに音量を反映
        seSlider.value = se; // SEスライダーに音量を反映

        // 音量を反映
        SetBGMVolume(bgm); // BGM音量を反映
        SetSEVolume(se); // SE音量を反映

        // スライダー変更時の処理を登録
        bgmSlider.onValueChanged.AddListener(SetBGMVolume); 
        seSlider.onValueChanged.AddListener(SetSEVolume);
    }

    // BGM音量を設定・保存
    public void SetBGMVolume(float value)
    {
        // AudioMixerのBGM音量を変更
        mixer.SetFloat(
            "BGMVolume",
            Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20 // 音量をデシベル値に変換
        );

        PlayerPrefs.SetFloat("BGMVolume", value); // BGM音量を保存
        PlayerPrefs.Save();
    }

    // SE音量を設定・保存
    public void SetSEVolume(float value)
    {
        // AudioMixerのSE音量を変更
        mixer.SetFloat(
            "SEVolume",
            Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20
        );

        PlayerPrefs.SetFloat("SEVolume", value); // SE音量を保存
        PlayerPrefs.Save();
    }
}
