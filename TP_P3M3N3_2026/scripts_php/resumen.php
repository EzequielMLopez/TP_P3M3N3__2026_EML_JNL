<?php

//Invocamos la conexion a la base de datos mediante el archivo conexion.php
require_once 'conexion.php';

// Iniciamos una sesion
session_start();

// Si no hay sesion iniciada, redirigimos a ingreso.html
if (!isset($_SESSION['tipo_doc'], $_SESSION['documento'], $_SESSION['usuario'])) {
    header ("Location: ../ingreso.html");
    exit();
}

?>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Resumen</title>
</head>
<body>

    <!-- Damos la bienvenida al usuario y mostramos los datos -->
    <h1>Bienvenido, <?php echo $_SESSION['usuario']; ?></h1>

    <p>Usuario: <?php echo $_SESSION['usuario']; ?></p>
    <p>Documento: <?php echo $_SESSION['documento']; ?></p>
    <p>Tipo de documento: <?php echo $_SESSION['tipo_doc']; ?></p>

    <!-- Le ofrecemos la posibilidad de realizar logout -->
    <p>
        A continuacion se le mostrara su resumen de tarjeta. 
        En caso de desear finalizar la sesion, haga click en 
        <a href="logout.php">cerrar sesion</a>.
    </p>

    <!-- mostramos la ultima liquidacion -->
    <p>-------------------------------</p>
    <h2>ULTIMA LIQUIDACION</h2>
    <p>-------------------------------</p>


    <?php
    
    /*Extraemos el campo documento de los datos de la sesion para
    almacenarlo en una variable $documento*/
    $documento = $_SESSION['documento'];

    /*Escribimos la consulta para traer la ultima liquidacion,
    por eso ordenamos por l.periodo en forma descendente y limitamos
    la respuesta a 1 (LIMI 1)*/ 
    $sql = 
    "SELECT l.periodo, l.fecha_vencimiento, l.total_a_pagar, l.pago_minimo
    FROM liquidaciones l
    JOIN tarjetas t
    ON l.num_cuenta = t.num_cuenta
    WHERE t.dni_titular = '$documento'
    ORDER BY l.periodo DESC
    LIMIT 1";
    
    /* Ejecutamos la consulta en la conexion $conn y guardamos lo devuelto
    en la varibale $resultado*/
    $resultado = $conn->query($sql);

    // Validamos que nos haya devuelto una fila
    if ($resultado->num_rows > 0) {

        //Reciclamos la variable $resultado convirtiendola en un array asociativo
        $resultado = $resultado->fetch_assoc();
        //Mostramos los datos en pantalla con estilo de parrafo de html
        echo "<p>Periodo: ".$resultado['periodo']."</p>";
        echo "<p>Vencimiento: ".$resultado['fecha_vencimiento']."</p>";
        echo "<p>Total a pagar: ".$resultado['total_a_pagar']."</p>";
        echo "<p>Pago minimo: ".$resultado['pago_minimo']."</p>";

    } else {
        //Si no se encontro liquidaciones se muestra un mensaje
        die("Liquidacion no encontrada.");
    }

    ?>

    <p>-------------------------------</p>
    <h2>HISTORIAL</h2>
    <p>-------------------------------</p>

    <?php

    $sql = 
    "SELECT l.periodo, l.fecha_vencimiento, l.total_a_pagar, l.pago_minimo
    FROM liquidaciones l
    JOIN tarjetas t
    ON l.num_cuenta = t.num_cuenta
    WHERE t.dni_titular = '$documento'
    ORDER BY l.periodo DESC
    LIMIT 1, 6";
    
    $resultado = $conn->query($sql);

    if ($resultado->num_rows > 0) {
        
        while ($fila = $resultado->fetch_assoc()) {

            echo "<p>";
            echo "<a href='detalle_liquidacion.php?periodo=".$fila['periodo']."'>
                    Liquidacion ".$fila['periodo']."
                    </a>";
            echo "</p>";
        
        
        }      


    } else {
        die("Liquidaciones mas antiguas, no encontradas.");
    }

    ?>

</body>
</html>

