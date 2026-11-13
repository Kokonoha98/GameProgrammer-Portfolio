using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [Header("Settings")]
    public float cameraDistance = 3f;
    public float cameraRadius = 0.2f;
    public float smoothSpeed = 10f;

    public float playerBuffer = 0.1f; // Playeyとカメラの最初距離

    public LayerMask obstacleLayers;

    private Vector3 defaultLocalPosition;

    private CharacterController playerController;

    void Start()
    {
        // カメラ位置を保存。障害物との衝突判定後も、この位置を基準にカメラの向き・距離を計算
        defaultLocalPosition = transform.localPosition;

        // 親をたどってPlayerのCharacter Controllerを取得
        playerController = GetComponentInParent<CharacterController>();
    }

    void LateUpdate()
    {
        // 親オブジェクトを取得
        Transform pivot = transform.parent;

        if (pivot == null)
        {
            return;
        }

        Vector3 origin = pivot.position; // 判定開始位置を設定
        Vector3 direction = defaultLocalPosition.normalized; // カメラの方向を取得

        float targetDistance = cameraDistance; // カメラの目標距離を設定

        // 現在のカメラからプレイヤーへの方向
        Vector3 currentCameraDirection = transform.position - origin;

        // カメラの壁・障害物との衝突判定
        RaycastHit hit;

        // 障害物までの距離からカメラ位置を計算
        if (Physics.SphereCast(origin,
                               cameraRadius,
                               currentCameraDirection,
                               out hit,
                               cameraDistance,
                               obstacleLayers,
                               QueryTriggerInteraction.Ignore))
        {
            // 壁の手前までカメラを移動
            targetDistance = hit.distance - cameraRadius;

            // カメラが近づきすぎないようにする
            targetDistance = Mathf.Max(targetDistance, 0.3f);

            /* カメラの最小距離を判定
            if (targetDistance < 0.3f)
                targetDistance = 0.3f; // カメラの最小距離を設定*/
        }

        // プレイヤー自身との衝突判定
        if (playerController != null)
        {
            Vector3 playerCenter = playerController.bounds.center;

            float playerRadius = playerController.radius;

            float playerHeight = playerController.height;

            // Character Controllerの上下方向
            Vector3 playerUp = playerController.transform.up;

            // カプセルの半分の高さ
            float halfHeight = Mathf.Max(playerHeight * 0.5f, playerRadius);

            // カプセルの上下の中心点
            Vector3 capsuleBottom = playerCenter + playerUp * (-halfHeight + playerRadius);

            Vector3 capsuleTop = playerCenter + playerUp * (halfHeight - playerRadius);

            // プレイヤーのカプセルとカメラまでの距離を確認
            Vector3 closestPoint =
                ClosestPointOnLineSegment(
                    capsuleBottom,
                    capsuleTop,
                    transform.position
                );

            float distanceToPlayer =
                Vector3.Distance(
                    transform.position,
                    closestPoint
                );

            float minimumPlayerDistance = playerRadius + cameraRadius + playerBuffer;

            // カメラがプレイヤーのカプセルに入り込んでいる場合
            if (distanceToPlayer < minimumPlayerDistance)
            {
                Vector3 pushDirection = transform.position - closestPoint;

                // 方向が取得できない場合の対策
                if (pushDirection.sqrMagnitude < 0.0001f)
                {
                    pushDirection = -direction;
                }

                pushDirection.Normalize();

                // プレイヤーから安全な位置まで押し出す
                Vector3 safePosition = closestPoint + pushDirection * minimumPlayerDistance;

                // プレイヤーから安全位置までの距離
                float safeDistance =
                    Vector3.Distance(
                        origin,
                        safePosition
                    );

                // 壁との衝突判定結果よりも
                // プレイヤーとの衝突を優先
                targetDistance =
                    Mathf.Max(
                        targetDistance,
                        safeDistance
                    );
            }
        }

        // カメラの目標位置を計算
        Vector3 targetLocalPos = direction * targetDistance;

        // カメラを滑らかに移動
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetLocalPos,
            Time.deltaTime * smoothSpeed);
    }

    // 線分上から一番近い点を取得する
    private Vector3 ClosestPointOnLineSegment(
        Vector3 start,
        Vector3 end,
        Vector3 point)
    {
        Vector3 line = end - start;

        float lengthSquared =
            line.sqrMagnitude;

        if (lengthSquared < 0.0001f)
        {
            return start;
        }

        float t =
            Vector3.Dot(point - start, line) / lengthSquared;

        t = Mathf.Clamp01(t);

        return start + line * t;
    }
}
