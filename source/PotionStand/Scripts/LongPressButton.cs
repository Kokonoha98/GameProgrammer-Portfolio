using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LongPressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float requiredHoldTime = 1f;
    public UnityEvent onLongPress;

    [Header("円形ゲージ")]
    public Image holdGaugeImage;

    private bool isHolding = false;
    private float holdTimer = 0f;
    private bool hasTriggered = false;

    void Update()
    {
        if (!isHolding) return;

        holdTimer += Time.deltaTime;

        float progress = holdTimer / requiredHoldTime;

        if (holdGaugeImage != null)
            holdGaugeImage.fillAmount = progress;

        if (holdTimer >= requiredHoldTime && !hasTriggered)
        {
            hasTriggered = true;
            isHolding = false;

            if (holdGaugeImage != null)
                holdGaugeImage.fillAmount = 1f;

            onLongPress.Invoke();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        hasTriggered = false;
        holdTimer = 0f;

        if (holdGaugeImage != null)
            holdGaugeImage.fillAmount = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetHold();
    }

    public void ResetHold()
    {
        isHolding = false;
        holdTimer = 0f;
        hasTriggered = false;

        if (holdGaugeImage != null)
            holdGaugeImage.fillAmount = 0f;
    }
}
