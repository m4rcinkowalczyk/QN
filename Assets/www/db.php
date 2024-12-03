<?php
$host = 'sql.studiomi.home.pl';
$user = '00313842_quizapp';
$password = 'Nf4y7tLr_@#';
$database = '00313842_quizapp';

$conn = new mysqli($host, $user, $password, $database);

// Sprawdzamy połączenie
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}

// Ustawienie kodowania UTF-8 dla połączenia z bazą
$conn->set_charset("utf8");

// Nagłówki, aby dane wyjściowe były w UTF-8
header('Content-Type: application/json; charset=utf-8');
?>