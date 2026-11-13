using UnityEngine;
using UnityEngine.EventSystems;

public class PotButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    float pressTime;
    bool isPressing;

    public float requiredTime = 1f;

    void Update()
    {
        if (isPressing)
        {
            pressTime += Time.deltaTime;
            if (pressTime >= requiredTime)
            {
                isPressing = false;
                GameManager.Instance.CheckResult();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressing = true;
        pressTime = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressing = false;
    }
}

