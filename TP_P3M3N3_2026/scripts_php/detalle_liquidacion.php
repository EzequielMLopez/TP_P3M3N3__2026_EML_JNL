<?php

//Invocamos la conexion a la base de datos mediante el archivo conexion.php
require_once 'conexion.php';

// Iniciamos una sesion
session_start();

// Si no hay sesion iniciada, redirigimos a ingreso.html
if (!isset($_SESSION['tipo_doc'], $_SESSION['documento'], $_SESSION['usuario'])) {
    header("Location: ../ingreso.html");
    exit();
}


// Obtenemos el periodo desde la URL
$periodo = $_GET['periodo'];

/// Obtenemos el documento mediante los datos de la sesion
$documento = $_SESSION['documento'];

/* Realizamos la consulta para traernos los campos necesarios para armar
el detalle de la liquidacion, segun el periodo */

$sql =
"SELECT l.periodo,
        l.fecha_vencimiento,
        l.total_a_pagar,
        l.pago_minimo
FROM liquidaciones l
JOIN tarjetas t
ON l.num_cuenta = t.num_cuenta
WHERE t.dni_titular = '$documento'
AND l.periodo = '$periodo'";

/* Ejecutamos la consulta mediante la conexion  
a la base de datos de la variable $conn */
$resultado = $conn->query($sql);

// Comprobamos que nos haya traido una fila
if ($resultado->num_rows > 0) {

    // Al resultado lo volvemos un array asociativo que almacenamos en $liquidacion
    $liquidacion = $resultado->fetch_assoc();

    // Mostramos el detalle de la liquidacion
    echo "<h1>Detalle de Liquidacion:</h1>";

    echo "<p>Periodo: ".$liquidacion['periodo']."</p>";
    echo "<p>Vencimiento: ".$liquidacion['fecha_vencimiento']."</p>"; 
    echo "<p>Total a pagar: ".$liquidacion['total_a_pagar']."</p>"; 
    echo "<p>Pago minimo: ".$liquidacion['pago_minimo']."</p>";  
    
    echo "<a href='resumen.php'>Volver</a>";

} else {

    // En caso de no encontrar liquidacion detenemos la ejecucion y mostramos un mensaje.
    die("<p>No se encontro la liquidacion. Retornar a la página de <a href='resumen.php'>resumen</a>.</p>");

}

?>

