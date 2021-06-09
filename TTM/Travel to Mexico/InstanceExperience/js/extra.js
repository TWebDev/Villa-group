{
    // Set the date we're counting down to
    //var countDown = new Date("Jan 5, 2021 15:37:25").getTime();
    
    var countDown = new Date().getTime();
    countDown += (1000*60*52);
    // Update the count down every 1 second
    var x = setInterval(function() {
    
      // Get today's date and time
      var now = new Date().getTime();
        
      // Find the distance between now and the count down date
      var distance = countDown - now;
        
      // Time calculations for days, hours, minutes and seconds
      var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
      var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
      var seconds = Math.floor((distance % (1000 * 60)) / 1000);
      var hs, ms, ss;
      if (hours<10)
        hs = "0" + hours;
      else
        hs = hours;
      if (minutes<10)
        ms = "0" + minutes;
      else
        ms = minutes;
      if (seconds<10)
        ss = "0" + seconds;
      else
        ss = seconds;
        
      // Output the result in an element with id="demo"
      document.getElementById("offer-countdown").innerHTML = hs + " : "
      + ms + " : " + ss ;
        
      // If the count down is over, write some text 
      if (distance < 0) {
        clearInterval(x);
        document.getElementById("offer-countdown").innerHTML = "00 00 00";
      }
    }, 1000);
    };