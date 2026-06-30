<?php

//Invocamos la conexion a la base de datos mediante el archivo conexion.php
require_once 'conexion.php';

/*Permite solamente que el proceso se ejecute si recibio el formulario
bajo el metodo POST*/
if ($_SERVER["REQUEST_METHOD"] != "POST") {
    die("Acceso no permitido");
}


// Datos proporcionados por registro.html y asignados cada uno a una variable

$tipo_doc = $_POST['tipo_doc'];
$documento = $_POST['documento'];
$nombre = $_POST['nombre'];
$apellido = $_POST['apellido'];
$fecha_nacimiento = $_POST['fecha_nacimiento'];
$email = $_POST['email'];
$usuario = $_POST['usuario'];
$contraseña = $_POST['passwordA'];
$repetir_contraseña = $_POST['passwordB'];

// Validamos que las contraseñas recibidas de registro.html sean iguales

if ($contraseña == $repetir_contraseña) {

    // Creacion de datos necesarios no proporcionados

    $bancos = array( // Vector con Bancos que operativos
        "Banco Nacion",
        "Banco Provincia",
        "Banco Galicia",
        "Banco Santander",
        "Banco BBVA",
        "Banco Macro",
        "Banco Credicoop",
    );

    $banco = $bancos[array_rand($bancos)]; // Seleccionamos de forma aleatoria un Banco

    $estado = "Activa"; // Estado de la tarjeta

    //IMPORTANTE: Al numero de tarjeta lo creamos mas adelante en el codigo

    ?>

<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8">  
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Altas</title>
</head>
<body>

    <?php

    // Corroboramos que los datos Documento y Email no esten registrados en las tablas

    $sql = "SELECT documento FROM usuarios u WHERE u.documento = '$documento'";
    $resultado_documento = $conn->query($sql);

    $sql = "SELECT email FROM usuarios WHERE email = '$email'";
    $resultado_email = $conn->query($sql);

    if (!filter_var($email, FILTER_VALIDATE_EMAIL)){ //Se valida que la estructura sea de un email valido

        die("<p>El formato del correo electronico es incorrecto. Retornar a la página de <a href='../registro.html'>registro</a>.</p>");

    } 
    
    if ($resultado_email->num_rows > 0) {

        die("<p>El email ya está registrado. Retornar a la página de <a href='../registro.html'>registro</a>.</p>");

    } else if ($resultado_documento->num_rows > 0) {

        die("<p>Usuario ya está registrado. Retornar a la página de <a href='../registro.html'>registro</a>.</p>");

    }
    

    // Creamos tarjetas, hasta conseguir una que no este registrada
    do {

        $numero_tarjeta = random_int(100000000000000, 999999999999999);

        $sql = "SELECT numero_tarjeta FROM tarjetas WHERE numero_tarjeta = '$numero_tarjeta'";
        $resultado_tarjeta = $conn->query($sql);

    } while ($resultado_tarjeta->num_rows > 0);

    // Muestreo de los datos recibidos de registro.html y los creados por algoritmos antes de guardar en base de datos
    echo "<h2>Datos recibidos muestreo:</h2>";

    echo "<p>Tipo de documento: ".$tipo_doc."</p>";
    echo "<p>Documento: ".$documento."</p>";
    echo "<p>Nombre: ".$nombre."</p>";
    echo "<p>Apellido: ".$apellido."</p>";
    echo "<p>Fecha nacimiento: ".$fecha_nacimiento."</p>";
    echo "<p>Correo electronico: ".$email."</p>";
    echo "<p>Usuario: ".$usuario."</p>";
    echo "<p>Contraseña: ".$contraseña."</p>";
    echo "<p>Repetir contraseña: ".$repetir_contraseña."</p>";


    echo "<h2>Datos necesarios no proporcionados muestreo:</h2>";
    echo "<p>Numero de tarjeta: " .$numero_tarjeta."</p>";
    echo "<p>Banco: " .$banco."</p>";


    // Almacenamiento de consulta en variable $sql con los valores proporcionados por registro.html

    $sql = "INSERT INTO usuarios (documento, tipo_doc, nombre, apellido, fecha_nacimiento, email, usuario, password)
    VALUES ('$documento','$tipo_doc', '$nombre', '$apellido', '$fecha_nacimiento', '$email', '$usuario', '$contraseña')";

    // Realizamos la insercion de la consulta almacenada en $sql en la tabla "usuarios"

    if ($conn->query($sql) === TRUE){

        echo "<p>Insertado con exito en la tabla usuarios!</p>";

    } else {

        echo "Error insertando en la tabla usuarios: " . $conn->error;

    }

    // Almacenamiento de consulta en variable $sql con los valores no proporcionados pero necesarios

    $sql = "INSERT INTO tarjetas (numero_tarjeta, banco_emisor, estado, saldo, dni_titular)
    VALUES ('$numero_tarjeta', '$banco', '$estado', 0, '$documento')";

    // Realizamos la insercion de la consulta almacenada en $sql en la tabla "tarjetas"

    if ($conn->query($sql) === TRUE){

        echo "<p>Insertado con exito en la tabla tarjetas!</p>";

    } else {

        echo "Error insertando en la tabla tarjetas: " . $conn->error;

    }

    // Cerramos la conexion

    $conn->close();

    echo "<a href='../ingreso.html'>Ir a Login </a>";

} else {
    die("<p>Contraseñas no coinciden. Retornar a la página de <a href='../registro.html'>registro</a>.</p>");
}


?>

</body>
</html>