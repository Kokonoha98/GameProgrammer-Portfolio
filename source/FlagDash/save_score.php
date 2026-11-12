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

// Unityから名前とスコアを受け取る
$name = $_POST["name"];
$score = $_POST["score"];

// 名前を大文字に変換
$name = strtoupper($name);

// 英数字3文字か確認
if (!preg_match('/^[A-Z0-9]{3}$/', $name)) {
    die("名前は英数字3文字で入力してください");
}

// 現在のランキング件数を確認
$sql = "SELECT COUNT(*) AS count FROM ranking";
$result = $conn->query($sql);
$row = $result->fetch_assoc();

$count = $row["count"];

// 10件未満なら保存
if ($count < 10) {

    $sql = "INSERT INTO ranking (name, time) VALUES ('$name', $score)";

    if ($conn->query($sql) === TRUE) {
        echo "保存成功";
    } else {
        echo "保存失敗";
    }

} else {

    // 現在の10位のスコアを取得
    // スコアが高い順なので、一番小さい値が10位
    $sql = "SELECT time FROM ranking ORDER BY time ASC LIMIT 1";
    $result = $conn->query($sql);
    $row = $result->fetch_assoc();

    $lastScore = $row["time"];

    // 10位より高いスコアなら保存
    if ($score > $lastScore) {

        // 新しいスコアを保存
        $sql = "INSERT INTO ranking (name, time) VALUES ('$name', $score)";
        $conn->query($sql);

        // 11件になったので一番低いスコアを削除
        $sql = "DELETE FROM ranking ORDER BY time ASC LIMIT 1";
        $conn->query($sql);

        echo "保存成功";

    } else {

        echo "ランキング外";

    }
}

$conn->close();

?>
