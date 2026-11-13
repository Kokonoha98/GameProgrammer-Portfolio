using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EncyclopediaManager : MonoBehaviour
{
    public static EncyclopediaManager Instance;

    public List<CreatureData> allCreatures;

    private HashSet<string> discovered = new HashSet<string>();

    private HashSet<string> unlocked = new HashSet<string>();

    private const string SAVE_KEY_UNLOCK = "ENCY_UNLOCK";
    private const string SAVE_KEY_DISCOVER = "ENCY_DISCOVER";

    // =========================
    // 自動生成
    // =========================
    

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //LoadAllCreatures();

        LoadData(); // ★起動時ロード
    }

    // =========================
    // Creature自動読込
    // =========================
    
    // =========================
    //  発見時
    // =========================
    public void Discover(CreatureData data)
    {
        if (data == null) return;

        if (discovered.Contains(data.id)) return;

        discovered.Add(data.id);
        SaveData();
    }

    // =========================
    //  解放
    // =========================
    public void Unlock(CreatureData data)
    {
        if (data == null) return;

        if (unlocked.Contains(data.id)) return;

        unlocked.Add(data.id);

        SaveData(); // 解放時に保存
    }

    // =========================
    //  解放チェック
    // =========================
    public bool IsDiscovered(CreatureData data)
    {
        if (data == null) return false;

        return discovered.Contains(data.id);
    }

    public bool IsUnlocked(CreatureData data)
    {
        if (data == null) return false;

        return unlocked.Contains(data.id);
    }

    // 解放率計算
    public float GetEncyclopediaRate()
    {
        int total = allCreatures.Count;
        if (total == 0) return 0;

        float score = 0f;

        foreach (var c in allCreatures)
        {
            if (unlocked.Contains(c.id))
            {
                score += 1f;
            }
            else if (discovered.Contains(c.id))
            {
                score += 0.5f;
            }
        }

        return (score / total) * 100f;
    }

    // =========================
    //  セーブ
    // =========================
    void SaveData()
    {
        // 解放
        PlayerPrefs.SetString(SAVE_KEY_UNLOCK,string.Join(",", unlocked));

        // 発見
        PlayerPrefs.SetString(SAVE_KEY_DISCOVER,string.Join(",", discovered));

        PlayerPrefs.Save();
    }

    // =========================
    //  ロード
    // =========================
    void LoadData()
    {
        unlocked.Clear();
        discovered.Clear();

        // 解放
        string unlockSave = PlayerPrefs.GetString(SAVE_KEY_UNLOCK, "");
        foreach (var id in unlockSave.Split(','))
        {
            if (!string.IsNullOrEmpty(id))
            {
                unlocked.Add(id);
            }
        }

        // 発見
        string discoverSave = PlayerPrefs.GetString(SAVE_KEY_DISCOVER, "");
        foreach (var id in discoverSave.Split(','))
        {
            if (!string.IsNullOrEmpty(id))
            {
                discovered.Add(id);
            }
        }
    }

    // =========================
    //  図鑑用
    // =========================
    public List<CreatureData> GetAllCreatures()
    {
        return allCreatures.OrderBy(c => c.difficulty).ToList();
    }
}
