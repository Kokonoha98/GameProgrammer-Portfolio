using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance; // PlayerManagerをシングルトン化

    [Header("プレイヤープレハブ")]
    [SerializeField]
    private GameObject playerPrefab;

    private GameObject player;

    public Transform PlayerTransform => player != null ? player.transform : null; // プレイヤーのTransformを取得

    // プレイヤーの入力コンポーネントを取得
    public PlayerInput PlayerInput =>
        player != null ? player.GetComponent<PlayerInput>() : null;

    // プレイヤーの入力情報を取得
    public StarterAssetsInputs StarterInputs =>
        player != null ? player.GetComponent<StarterAssetsInputs>() : null;

    // プレイヤーのPlayerSpawnerを取得
    public PlayerSpawner PlayerSpawner =>
        player != null ? player.GetComponent<PlayerSpawner>() : null;

    private void Awake()
    {
        Instance = this;
    }

    // プレイヤーを生成
    public void Spawn(Transform point)
    {
        // 既存のプレイヤーがいるか確認
        if (player != null)
        {
            Destroy(player); // 既存のプレイヤーを削除
        }

        // プレイヤーを指定位置に生成
        player = Instantiate(
            playerPrefab,
            point.position,
            point.rotation);

        GameManager.Instance.InitializePlayer(); // GameManagerのプレイヤー情報を初期化
    }
}
