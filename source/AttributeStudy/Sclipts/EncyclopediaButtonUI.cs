using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaButtonUI : MonoBehaviour
{
    public Text nameText;

    private CreatureData data;

    public void SetupEmpty()
    {
        data = null;
        nameText.text = "？？？";
    }
    public void Setup(CreatureData d)
    {
        data = d;

        if (data == null)
        {
            nameText.text = "？？？";
            return;
        }

        if (EncyclopediaManager.Instance.IsUnlocked(data))
        {
            nameText.text = data.creatureName;
        }
        else if (EncyclopediaManager.Instance.IsDiscovered(data))
        {
            nameText.text = data.creatureName; // 名前だけ見える
        }
        else
        {
            nameText.text = "？？？";
        }
    }

    public void OnClick()
    {
        if (data == null) return;

        if (!EncyclopediaManager.Instance.IsDiscovered(data))
        {
            return;
        }

        EncyclopediaDetailUI.Instance.Show(data);
    }
}
