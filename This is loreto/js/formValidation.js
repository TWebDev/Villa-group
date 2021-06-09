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

//field testing and validation function
function instantValidation(field) {
  var webForm = document.getElementById("form");
  var bForm = isHidden(webForm) ? document.getElementById("mc-embedded-subscribe") : document.getElementById("mc-embedded-subscribe-mob");
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

//now bind a delegated change event
//== THIS FAILS IN INTERNET EXPLORER <= 8 ==//
//addEvent(document, 'change', function(e, target)
//{
//	instantValidation(target);
//});

//now bind a change event to each applicable for field
var webForm = document.getElementById("form");
var fields = isHidden(webForm) ? document.getElementsByClassName("desk") : document.getElementsByClassName("mob");
//document.getElementsByTagName("input");
for (var a = fields.length, i = 0; i < a; i++) {
    addEvent(fields[i], "change", function(e, target) {
      instantValidation(target);
    });
}
