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

// スコアの高い順に10件取得
$sql = "SELECT name, time FROM ranking ORDER BY time DESC LIMIT 10";

$result = $conn->query($sql);

$ranking = [];

while ($row = $result->fetch_assoc()) {

    $ranking[] = [
        "name" => $row["name"],
        "score" => (float)$row["time"]
    ];

}

// JSONとして返す
header("Content-Type: application/json; charset=UTF-8");

echo json_encode($ranking);

$conn->close();

?>
