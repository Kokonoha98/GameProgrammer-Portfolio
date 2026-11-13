using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AnswerEffectPlayer : MonoBehaviour
{
    [Header("演出用Canvas")]
    public Canvas effectCanvas;

    [Header("エフェクト")]
    public GameObject correctEffectPrefab;
    public GameObject wrongEffectPrefab;

    [Header("演出時間")]
    public float flyTime = 0.8f;

    [Header("放物線の高さ倍率")]
    [Range(0.01f, 1.0f)]
    public float arcHeightRate = 0.12f;

    [Header("回転速度")]
    public float rotateSpeed = 180f;

    [Header("吸い込み")]
    public float absorbTime = 0.15f;

    [Header("判定待機")]
    public float resultWaitTime = 0.5f;

    public IEnumerator PlayEffect(
    Image sourceIcon,
    Button targetButton,
    bool isCorrect)
    {
        // Canvas準備
        Canvas canvas = effectCanvas;
        Camera uiCamera = canvas.worldCamera;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // 元UI取得
        RectTransform sourceRect = sourceIcon.rectTransform;

        RectTransform targetRect = targetButton.GetComponent<RectTransform>();

        // Screen座標へ変換
        Vector2 startScreenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, sourceRect.position);

        Vector2 targetScreenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, targetRect.position);

        // Canvasローカルへ変換
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, startScreenPos, uiCamera, out Vector2 startPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, targetScreenPos, uiCamera, out Vector2 targetPos);
        
        // 飛行用コピー生成
        Image flyingIcon = Instantiate(sourceIcon, canvas.transform);

        flyingIcon.gameObject.SetActive(true);

        // 最前面へ
        flyingIcon.transform.SetAsLastSibling();

        // 本体を隠す
        sourceIcon.gameObject.SetActive(false);

        // RectTransform取得
        RectTransform flyingRect = flyingIcon.rectTransform;

        flyingRect.anchoredPosition = startPos;

        // 距離に応じて高さ調整
        float distance = Vector2.Distance(startPos, targetPos);

        float arcHeight = distance * arcHeightRate;

        // 飛行演出
        float t = 0;

        while (t < flyTime)
        {
            t += Time.deltaTime;

            float p = t / flyTime;

            // なめらか補間
            p = Mathf.SmoothStep(0, 1, p);

            Vector2 pos = Vector2.Lerp(startPos, targetPos, p);

            // 放物線
            pos.y += Mathf.Sin(p * Mathf.PI) * arcHeight;

            flyingRect.anchoredPosition = pos;

            // 回転
            flyingRect.Rotate(
                0,
                0,
                rotateSpeed * Time.deltaTime
            );

            yield return null;
        }

        flyingRect.anchoredPosition = targetPos;

        // 吸い込み演出
        float absorbT = 0;

        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.zero;

        while (absorbT < absorbTime)
        {
            absorbT += Time.deltaTime;

            float p = absorbT / absorbTime;

            flyingRect.localScale = Vector3.Lerp(startScale, endScale, p);

            yield return null;
        }

        // 飛行アイコン削除
        Destroy(flyingIcon.gameObject);

        // ボタン押し込み演出
        yield return StartCoroutine(PunchButton(targetButton));

        yield return new WaitForSeconds(1.0f);

        // エフェクト生成
        GameObject prefab = isCorrect ? correctEffectPrefab : wrongEffectPrefab;

        GameObject effect = Instantiate(prefab, effectCanvas.transform);

        RectTransform effectRect = effect.GetComponent<RectTransform>();

        if (effectRect != null)
        {
            effectRect.anchoredPosition = targetPos;
        }

        Destroy(effect, 1.5f);

        // 待機
        yield return new WaitForSeconds(resultWaitTime);
    }

    IEnumerator PunchButton(Button button)
    {
        Vector3 normal = Vector3.one;
        Vector3 pressed = Vector3.one * 1.2f;

        float duration = 0.08f;

        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;

            button.transform.localScale = Vector3.Lerp(normal, pressed, t / duration);

            yield return null;
        }

        t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;

            button.transform.localScale = Vector3.Lerp(pressed, normal, t / duration);

            yield return null;
        }

        button.transform.localScale = Vector3.one;
    }
}
