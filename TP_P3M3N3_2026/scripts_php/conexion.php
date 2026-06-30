<?php

// Datos necesarios para realizar la conexion con la DB
$servername = "localhost";
$username = "root";
$password = "";
$dbname = "mi_banco_db";

// Instanciamos un objeto $conn perteneciente a la clase msqli propia de php.

$conn = new mysqli($servername, $username, $password, $dbname);

// Checkeo de conexion

if ($conn -> connect_error){
    die("Error en la conexion: " . $conn->connect_error);
}