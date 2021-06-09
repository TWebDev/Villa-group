<?php
 //Llamada a campos
 $fname = $_POST['fname'];
 $lname = $_POST['lname'];
 $phone = $_POST['phone'];
 $email = $_POST['email'];

 //Datos hacia el correo
 $destinatario = "paola.vazra98@gmail.com";
 $asunto = "Lead Instance Experience";

 $carta = "De: $fname \n "
 $carta .= "Email: $email \n"
 $carta .= "Phone: $phone ";

//Enviando mensaje
mail($destinatario, $asunto, $carta);





 ?>
