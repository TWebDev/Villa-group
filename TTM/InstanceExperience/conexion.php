<?php

  $usuario = "Paola";
  $password = "021298PV";
  $servidor = "localhost";
  $BD = "instanceexperience";


    /***CONECCIÓN EN BASE DE DATOS*/
    $conection = mysqli_connect ($servidor, $usuario, $password, $BD);
    if (!$conection) {
     die("Connection failed: " . mysqli_connect_error());
    }
    echo "Connected successfully";

    mysqli_select_db($conection, $BD);


 ?>
