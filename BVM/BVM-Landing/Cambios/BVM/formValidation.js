//add event construct for modern browsers or IE
//which fires the callback with a pre-converted target reference
function addEvent(node, type, callback) {
  if (node.addEventListener) {
    node.addEventListener(
      type,
      function(e) {
        callback(e, e.target);
      },
      false
    );
  } else if (node.attachEvent) {
    node.attachEvent("on" + type, function(e) {
      callback(e, e.srcElement);
    });
  }
}

//identify whether a field is visible
function isHidden(el) {
  var style = window.getComputedStyle(el);
  return ((style.display === 'none') || (style.visibility === 'hidden'))
}

//identify whether a field should be validated
//ie. true if the field is neither readonly nor disabled,
//and has either "pattern", "required" or "aria-invalid"
function shouldBeValidated(field) {
  return (
    !(field.getAttribute("readonly") || field.readonly) &&
    !(field.getAttribute("disabled") || field.disabled) &&
    (field.getAttribute("pattern") || field.getAttribute("required"))
  );
}
//get the timezone and add it to the form
function getTimezoneName() {
  tmSummer = new Date(Date.UTC(2005, 6, 30, 0, 0, 0, 0));
  so = -1 * tmSummer.getTimezoneOffset();
  tmWinter = new Date(Date.UTC(2005, 12, 30, 0, 0, 0, 0));
  wo = -1 * tmWinter.getTimezoneOffset();

  if (-660 == so && -660 == wo) return 'Pacific/Midway';
  if (-600 == so && -600 == wo) return 'Pacific/Tahiti';
  if (-570 == so && -570 == wo) return 'Pacific/Marquesas';
  if (-540 == so && -600 == wo) return 'America/Adak';
  if (-540 == so && -540 == wo) return 'Pacific/Gambier';
  if (-480 == so && -540 == wo) return 'US/Alaska';
  if (-480 == so && -480 == wo) return 'Pacific/Pitcairn';
  if (-420 == so && -480 == wo) return 'US/Pacific';
  if (-420 == so && -420 == wo) return 'US/Arizona';
  if (-360 == so && -420 == wo) return 'US/Mountain';
  if (-360 == so && -360 == wo) return 'America/Guatemala';
  if (-360 == so && -300 == wo) return 'Pacific/Easter';
  if (-300 == so && -360 == wo) return 'US/Central';
  if (-300 == so && -300 == wo) return 'America/Bogota';
  if (-240 == so && -300 == wo) return 'US/Eastern';
  if (-240 == so && -240 == wo) return 'America/Caracas';
  if (-240 == so && -180 == wo) return 'America/Santiago';
  if (-180 == so && -240 == wo) return 'Canada/Atlantic';
  if (-180 == so && -180 == wo) return 'America/Montevideo';
  if (-180 == so && -120 == wo) return 'America/Sao_Paulo';
  if (-150 == so && -210 == wo) return 'America/St_Johns';
  if (-120 == so && -180 == wo) return 'America/Godthab';
  if (-120 == so && -120 == wo) return 'America/Noronha';
  if (-60 == so && -60 == wo) return 'Atlantic/Cape_Verde';
  if (0 == so && -60 == wo) return 'Atlantic/Azores';
  if (0 == so && 0 == wo) return 'Africa/Casablanca';
  if (60 == so && 0 == wo) return 'Europe/London';
  if (60 == so && 60 == wo) return 'Africa/Algiers';
  if (60 == so && 120 == wo) return 'Africa/Windhoek';
  if (120 == so && 60 == wo) return 'Europe/Amsterdam';
  if (120 == so && 120 == wo) return 'Africa/Harare';
  if (180 == so && 120 == wo) return 'Europe/Athens';
  if (180 == so && 180 == wo) return 'Africa/Nairobi';
  if (240 == so && 180 == wo) return 'Europe/Moscow';
  if (240 == so && 240 == wo) return 'Asia/Dubai';
  if (270 == so && 210 == wo) return 'Asia/Tehran';
  if (270 == so && 270 == wo) return 'Asia/Kabul';
  if (300 == so && 240 == wo) return 'Asia/Baku';
  if (300 == so && 300 == wo) return 'Asia/Karachi';
  if (330 == so && 330 == wo) return 'Asia/Calcutta';
  if (345 == so && 345 == wo) return 'Asia/Katmandu';
  if (360 == so && 300 == wo) return 'Asia/Yekaterinburg';
  if (360 == so && 360 == wo) return 'Asia/Colombo';
  if (390 == so && 390 == wo) return 'Asia/Rangoon';
  if (420 == so && 360 == wo) return 'Asia/Almaty';
  if (420 == so && 420 == wo) return 'Asia/Bangkok';
  if (480 == so && 420 == wo) return 'Asia/Krasnoyarsk';
  if (480 == so && 480 == wo) return 'Australia/Perth';
  if (540 == so && 480 == wo) return 'Asia/Irkutsk';
  if (540 == so && 540 == wo) return 'Asia/Tokyo';
  if (570 == so && 570 == wo) return 'Australia/Darwin';
  if (570 == so && 630 == wo) return 'Australia/Adelaide';
  if (600 == so && 540 == wo) return 'Asia/Yakutsk';
  if (600 == so && 600 == wo) return 'Australia/Brisbane';
  if (600 == so && 660 == wo) return 'Australia/Sydney';
  if (630 == so && 660 == wo) return 'Australia/Lord_Howe';
  if (660 == so && 600 == wo) return 'Asia/Vladivostok';
  if (660 == so && 660 == wo) return 'Pacific/Guadalcanal';
  if (690 == so && 690 == wo) return 'Pacific/Norfolk';
  if (720 == so && 660 == wo) return 'Asia/Magadan';
  if (720 == so && 720 == wo) return 'Pacific/Fiji';
  if (720 == so && 780 == wo) return 'Pacific/Auckland';
  if (765 == so && 825 == wo) return 'Pacific/Chatham';
  if (780 == so && 780 == wo) return 'Pacific/Enderbury'
  if (840 == so && 840 == wo) return 'Pacific/Kiritimati';
  return 'US/Pacific';
}

//field testing and validation function
function instantValidation(field) {
  var webForm = document.getElementById("form");
  var bForm = isHidden(webForm) ? document.getElementById("mc-embedded-subscribe") : document.getElementById("mc-embedded-subscribe-mob");
  var tzField = document.getElementById("mce-TZ");
  //add tz value to the form
  if (!tzField.value)
    tzField.value = getTimezoneName();

  var valError = false;
  //if the field should be validated
  if (shouldBeValidated(field)) {
    //the field is invalid if:
    //it's required but the value is empty
    //it has a pattern but the (non-empty) value doesn't pass
    var invalid =
      (field.getAttribute("required") && !field.value) ||
      (field.getAttribute("pattern") &&
        field.value &&
        !new RegExp(field.getAttribute("pattern")).test(field.value));

    //add or remove the attribute is indicated by
    //the invalid flag and the current attribute state
    if (!invalid && field.getAttribute("aria-invalid")) {
      field.removeAttribute("aria-invalid");
    } else if (invalid && !field.getAttribute("aria-invalid")) {
      field.setAttribute("aria-invalid", "true");
      valError = true;
    }

    //enable/disable button
    var fields = isHidden(webForm) ? document.getElementsByClassName("desk") : document.getElementsByClassName("mob");
    for (var a = fields.length, i = 0; i < a; i++) {
        if((fields[i].getAttribute("aria-invalid")))
          valError = true;
    }
    if(valError) {
      bForm.setAttribute("disabled","true");
      bForm.classList.add("disabled");
    }
    else {
      bForm.removeAttribute("disabled");
      bForm.classList.remove("disabled");
    }
  }
}

function sendToEplat () {
  //var form = document.getElementsByClassName('mainform')[0];
  var email = document.getElementById('mce-EMAIL');
  var fname = document.getElementById('mce-FNAME');
  var lname = document.getElementById('mce-LNAME');
  var phone = document.getElementById('mce-PHONE');
  var timeToReach = document.getElementById('mce-TZ');
  var notes = document.getElementById('mce-PREF_DATES');
  /*var timeToReach = timeZone.value;*/
  var data = JSON.stringify({"webhookID": "886b01a7-6409-8db1-7083-6159c280728e", "email": email.value, "firstName": fname.value, "lastName": lname.value, "phone": phone.value, "notes": notes.value, "timeToReach": timeToReach.value});
  var xhr = new XMLHttpRequest();
  var url = "https://developers.eplat.com/api/webhooks/incoming/genericjson/newlead?code=80f32f6123104d09a72c000047564e51";
  xhr.open("POST", url, true);
  xhr.setRequestHeader("Content-Type", "application/json");
  // Create a state change callback 
  xhr.onreadystatechange = function () { 
      if (xhr.readyState === 4 && xhr.status === 200) { 
          // console.log(xhr.statusText);
      } 
  };
  xhr.send(data);
  //console.log(data);
}
function recaptchaCallback() {
  var rCResponse = grecaptcha.getResponse();
  if(rCResponse.length == 0) //rCaptcha validation failed
    document.getElementById('rc-inv').value = "";
  else
    document.getElementById('rc-inv').value = "nonEmpty";
  //console.log(document.getElementById('rc-inv').value);
  instantValidation(document.getElementById('rc-inv'));
}

function recaptchaExpired(){  
  document.getElementById('rc-inv').value = "";
  //console.log(document.getElementById('rc-inv').value);
  instantValidation(document.getElementById('rc-inv'));

}
//now bind a delegated change event
//== THIS FAILS IN INTERNET EXPLORER <= 8 ==//
//addEvent(document, 'change', function(e, target)
//{
//	instantValidation(target);
//});

//now bind a change event to each applicable for field
/*var webForm = document.getElementById("form");
var fields = isHidden(webForm) ? document.getElementsByClassName("desk") : document.getElementsByClassName("mob");
//document.getElementsByTagName("input");
for (var a = fields.length, i = 0; i < a; i++) {
    addEvent(fields[i], "change", function(e, target) {
      instantValidation(target);
    });
}*/
