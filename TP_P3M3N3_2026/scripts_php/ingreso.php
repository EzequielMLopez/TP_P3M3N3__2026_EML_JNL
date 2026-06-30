<?php

//Invocamos la conexion a la base de datos mediante el archivo conexion.php
require_once 'conexion.php';

//Iniciamos una sesion
session_start();

/*Permite solamente que el proceso se ejecute si recibio el formulario
bajo el metodo POST*/

if ($_SERVER["REQUEST_METHOD"] != "POST") {
    die("Acceso no permitido");
}

// Datos proporcionados por ingreso.html y asignados cada uno a una variable

$tipo_doc = $_POST['tipo_doc'];
$documento = $_POST['documento'];
$usuario_post = $_POST['usuario'];
$contraseña = $_POST['password'];

// Buscamos los datos en la tabla "usuarios" para validar los datos ingresados

$sql = "SELECT * FROM usuarios u WHERE u.documento = '$documento' AND u.tipo_doc = '$tipo_doc' AND u.usuario = '$usuario_post' AND u.password = '$contraseña'";
 
// Ejecutamos la consulta y almacenamos lo que trajo

$resultado_sesion = $conn->query($sql);

// Comprobamos si nos devolvio una fila
if ($resultado_sesion->num_rows > 0) {

    // Generamos un array asociativo y lo almacenamos en la variable "$usuario"
    $usuario = $resultado_sesion->fetch_assoc();

    // de los 4 datos utilizamos 3 para iniciar la sesion, ya que usar la contraseña es peligroso, aunque podriamos usar tipo_doc y usuario solamente.
    $_SESSION['tipo_doc'] = $usuario['tipo_doc'];
    $_SESSION['documento'] = $usuario['documento'];
    $_SESSION['usuario'] = $usuario['usuario'];

    header("Location: resumen.php"); // Redirigimos a resumen.php
    exit();
} else{
    die("<p>Alguno de los datos ingresados es incorrecto. Retornar a la página de <a href='../ingreso.html'>login</a>.</p>");
}

?>

