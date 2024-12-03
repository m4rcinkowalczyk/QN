<?php

require_once("db.php");

$sql = "SELECT * FROM pytania";
$result = $conn->query($sql);

$data = array();
if ($result->num_rows > 0) {
    while ($row = $result->fetch_assoc()) {
        $data[] = $row;
    }
}

// Opakowanie tablicy w obiekt
echo json_encode(['questions' => $data]);
$conn->close();

?>