<?php
    include("conexion.php");

    $first_name = $_POST['fname'];
    $last_name = $_POST['lname'];
    $phone = $_POST['phone'];
    $email = $_POST['email'];

    $usuario = "Paola";
    $password = "021298PV";
    $servidor = "localhost";
    $BD = "instanceexperience";


      /***COECCIÓN EN BASE DE DATOS*/
      $conection = mysqli_connect ($servidor, $usuario, $password, $BD);

  /**CHECAR CONEXIÓN**/
    if (!empty($_POST['fname']) && !empty($_POST['lname']) &&
        !empty($_POST['phone']) && !empty($_POST['email'])){

          $consulta = "INSERT INTO formulario VALUES ('$first_name', '$last_name','$phone' , '$email')";
          $resultado = mysqli_query($conection, $consulta);

        }else {
          echo "Error al Enviar los Datos";
        }

            mysqli_close($conection);


?>
