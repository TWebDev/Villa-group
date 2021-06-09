function validar(){
  var fname;
  var lname;
  var email;
  var phone;
  var expretion;


  fname = document.getElementById("fname").value;
  lname = document.getElementById("lname").value;
  email = document.getElementById("email").value;
  phone = document.getElementById("phone").value;


  expretion = /\w+@\w+\.+[a-z]/;
  //Validación de FORMULARIO
  if(fname == "" || lname == "" || email == "" || phone == ""){
    alert ("Please complete the assets");
    return false;
  }else if (fname.length>30) {
    alert ("The first name is to long");
    return false;
  }
  else if (lname.length>30) {
    alert ("The first name is to long");
    return false;
  }
  else if (isNaN(phone)) {
    alert("The phone is invalid");
    return false;
  }
  else if (phone.length>10) {
    alert ("The phone is incorrect");
    return false;
  }
  else if (phone.length<10) {
    alert("The phone is incorrect");
    return false;
  }
  else if (!expretion.test(email)){
    alert("The email is incorrect");
    return false;
  } {

  }

}



const email = document.getElementById("email");

email.addEventListener("input", function (event) {
  if (email.validity.typeMismatch) {
    email.setCustomValidity("Please enter a email");
  } else {
    email.setCustomValidity("");
  }
});
function ValidarEmail("email"){
  email = document.getElementById("email").value;
  if( !(/\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)/.test(email)) ) {
    return false;
  }
  else {
    alert("The email is incorrect");
  }
}
//Telefono
phone = document.getElementById("phone").value;
if( valor == null || phone.length == 0 || /^\s+$/.test(phone) ) {
  return false;
}
phone = document.getElementById("phone").value;
if( !(/^\d{10}$/.test(phone)) ) {
  return false;
}
