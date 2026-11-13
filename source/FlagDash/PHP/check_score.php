<?php

$host = "localhost";
$user = "root";
$password = "";
$dbname = "score_date";

// MySQLに接続
$conn = new mysqli($host, $user, $password, $dbname);

if ($conn->connect_error) {
    die("MySQL接続失敗");
}

// Unityから送られてきたスコア
$score = isset($_POST["score"])
    ? (float)$_POST["score"]
    : 0;

// 現在のランキング件数
$countResult = $conn->query(
    "SELECT COUNT(*) AS count FROM ranking"
);

$countRow = $countResult->fetch_assoc();
$count = (int)$countRow["count"];

// 10件未満ならランキング入り
if ($count < 10) {

    echo "true";

} else {

    // 10位のスコアを取得
    $result = $conn->query(
        "SELECT time
         FROM ranking
         ORDER BY time DESC
         LIMIT 1 OFFSET 9"
    );

    $row = $result->fetch_assoc();

    $lowestScore = (float)$row["time"];

    // 10位より高ければランキング入り
    if ($score > $lowestScore) {
        echo "true";
    } else {
        echo "false";
    }
}

$conn->close();

?>
