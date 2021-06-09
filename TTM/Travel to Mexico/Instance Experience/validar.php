<?php
  if(isset($_POST['register'])){
    $first_name = $_POST['fname'];
    $last_name = $_POST['lname'];
    $phone = $_POST['phone'];
    $email = $_POST['email'];
  }
  if(isset($_POST['register'])){
    if(empty($first_name)){
      echo "Please enter your first name";
    }
  }
?>