<?php

// Iniciamos la sesión
session_start();

// Vaciamos todas las variables de sesión
$_SESSION = [];

// Destruimos la sesión
session_destroy();

// Redirigimos al login
header("Location: ../ingreso.html");
exit();

?>