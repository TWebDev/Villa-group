wow = new WOW(
    {
        boxClass: 'wow',      // default
        animateClass: 'animated', // default
        offset: 0,          // default
        mobile: true,       // default
        live: true        // default
    }
);
wow.init();
$(function () {
    if (window.location.pathname == "/") {
        $("body").vegas({
            overlay: !0,
            slides: [
                {
                    src: "https://www.mynextvacations.com/content/themes/base/images/puerto-vallarta/destination/hd/puerto-vallarta-01.jpg",
                    animation: "kenburns",
                    transition: "fade"
                },
                {
                    src: "https://www.mynextvacations.com/content/themes/base/images/cabo-san-lucas/destination/hd/cabo-san-lucas-01.jpg",
                    animation: "kenburns",
                    transition: "fade"
                },
                {
                    src: "https://www.mynextvacations.com/content/themes/base/images/cancun/destination/hd/cancun-01.jpg",
                    animation: "kenburns",
                    transition: "fade"
                },
                {
                    src: "https://www.mynextvacations.com/content/themes/base/images/loreto/destination/hd/loreto-02.jpg",
                    animation: "kenburns",
                    transition: "fade"
                },
                {
                    src: "https://www.mynextvacations.com/content/themes/base/images/orlando/destination/orlando-01.jpg",
                    animation: "kenburns",
                    transition: "fade"
                },
                {
                    src: "https://www.mynextvacations.com/content/themes/base/images/puerto-vallarta/destination/hd/puerto-vallarta-02.jpg",
                    animation: "kenburns",
                    transition: "fade"
                },
                {
                    src: "https://www.mynextvacations.com/content/themes/base/images/cabo-san-lucas/destination/hd/cabo-san-lucas-02.jpg",
                    animation: "kenburns",
                    transition: "fade"
                },
                {
                    src: "https://www.mynextvacations.com/content/themes/base/images/cancun/destination/hd/cancun-03.jpg",
                    animation: "kenburns",
                    transition: "fade"
                },
                {
                    src: "https://www.mynextvacations.com/content/themes/base/images/loreto/destination/hd/loreto-03.jpg",
                    animation: "kenburns",
                    transition: "fade"
                },
                {
                    src: "https://www.mynextvacations.com/content/themes/base/images/orlando/destination/orlando-02.jpg",
                    animation: "kenburns",
                    transition: "fade"
                }
            ]
        });
    }
});