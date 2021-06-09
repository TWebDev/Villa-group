var Landing = function() {
    let date = moment().add(45, 'minutes');
    let timer;
    let contador = iniciarContador();
    let contadorIteraciones = 0;
    let contadorIntervalo;
    let contadorArr = [2, 1, 4];
    let contadorIndex = 0;

    let init = function() {
        if (localStorage.getItem("limiteOferta") != undefined) {
            date = moment(localStorage.getItem("limiteOferta"), 'YYYY-MM-DD HH:mm:ss');
        } else {
            localStorage.setItem("limiteOferta", date.format('YYYY-MM-DD HH:mm:ss'));
        };

        timer = setInterval(Landing.actualizaTiempo, 1000);
        contadorIntervalo = setInterval(Landing.actualizaContador, 4000);
        Landing.actualizaContador();

        $('.owl-carousel').owlCarousel({
            loop: true,
            margin: 10,
            nav: false,
            responsive: {
                0: {
                    items: 2
                },
                600: {
                    items: 3
                },
                1000: {
                    items: 7
                }
            }
        });

        //validacion
        'use strict';
        window.addEventListener('load', function() {
            // Fetch all the forms we want to apply custom Bootstrap validation styles to
            var forms = document.getElementsByClassName('needs-validation');
            // Loop over them and prevent submission
            var validation = Array.prototype.filter.call(forms, function(form) {
                form.addEventListener('submit', function(event) {
                    event.preventDefault();
                    event.stopPropagation();
                    if (form.checkValidity()) {
                        Landing.enviarFormulario();
                    }
                    form.classList.add('was-validated');
                }, false);
            });
        }, false);
    }

    function iniciarContador() {
        return Math.floor(Math.random() * (145 - 130) + 130);
    }

    function pad(n, width, z) {
        z = z || '0';
        n = n + '';
        return n.length >= width ? n : new Array(width - n.length + 1).join(z) + n;
    }

    let actualizaContador = function() {
        if (contadorIteraciones < 900) {
            contador += contadorArr[contadorIndex];
            contadorIteraciones++;
            if (contadorIndex < 2) {
                contadorIndex++;
            } else {
                contadorIndex = 0;
            }
            document.getElementById('contador').innerHTML = contador;
        } else {
            clearInterval(contadorIntervalo);
        }
    }

    let actualizaTiempo = function() {
        let horas = date.diff(moment(), 'hours');
        let minutos = date.diff(moment(), 'minutes');
        let segundos = date.diff(moment(), 'seconds');
        document.getElementById('horas').innerHTML = pad(horas, 2);
        document.getElementById('minutos').innerHTML = pad(minutos, 2);

        document.getElementById('segundos').innerHTML = pad(segundos - minutos * 60, 2);

        if (segundos - minutos * 60 < 0) {
            clearInterval(timer);
            document.getElementById('timer').remove();
        }
    }

    let mostrarFormulario = function() {
        var targetOffset = $("#cardForm").offset().top;
        $('html,body').animate({ scrollTop: targetOffset }, 500);
    }

    let enviarFormulario = function() {
        if (Landing.verificarValidacion()) {
            $('#lblWait').removeClass('d-none');
            $('button[type=submit]').hide();
            var data = {
                "webhookID": $('#hdnFormID').val(),
                "firstName": document.getElementById("firstName").value,
                "lastName": document.getElementById("lastName").value,
                "phone": document.getElementById("phone").value,
                "email": document.getElementById("email").value,
                "destinationID": 3
            };
            axios.post('https://developers.eplat.com/api/webhooks/incoming/genericjson/newlead?code=80f32f6123104d09a72c000047564e51', data)
                .then(function(response) {
                    $('#lblWait').addClass('d-none');
                    $('button[type=submit]').show();
                    window.location.replace($('#hdnConfirmation').val());
                })
                .catch(function(error) {
                    console.log(error);
                });

        }
    }


    let verificarValidacion = function() {
        let valid = true;
        if ($("#chkAgree").is(":checked")) {
            document.getElementById('lblMustAgree').classList.add('d-none');
        } else {
            valid = false;
            document.getElementById('lblMustAgree').classList.remove('d-none');
        }

        return valid;
    }

    return {
        init: init,
        actualizaTiempo: actualizaTiempo,
        actualizaContador: actualizaContador,
        mostrarFormulario: mostrarFormulario,
        enviarFormulario: enviarFormulario,
        verificarValidacion: verificarValidacion
    }
}();

Landing.init();