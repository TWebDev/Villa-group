Cookies.set("WebPage", "true", {expires: 2});

var myCookie = Cookies.get("WepPage");
var body = document.querySelector('body');

if (myCookie){
   body.classList.add("cookie"); 
}

if(myCookie = 'false'){
    body.classList.remove("cookie")
}