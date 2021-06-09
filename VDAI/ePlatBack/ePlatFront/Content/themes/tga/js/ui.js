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
                    src: "/content/themes/base/images/puerto-vallarta/destination/puerto-vallarta-06.jpg",
                    animation: "random",
                    transition: "swirlRight2"
                },
                {
                    src: "/content/themes/base/images/cabo-san-lucas/destination/cabo-san-lucas-07.jpg",
                    animation: "random",
                    transition: "swirlRight2"
                },
                {
                    src: "/content/themes/base/images/cancun/destination/cancun-05.jpg",
                    animation: "random",
                    transition: "swirlRight2"
                },
                {
                    src: "/content/themes/base/images/loreto/destination/loreto-06.jpg",
                    animation: "random",
                    transition: "swirlRight2"
                },
                {
                    src: "/content/themes/base/images/orlando/destination/orlando-01.jpg",
                    animation: "random",
                    transition: "swirlRight2"
                },
                {
                    src: "/content/themes/base/images/puerto-vallarta/destination/puerto-vallarta-07.jpg",
                    animation: "random",
                    transition: "swirlRight2"
                },
                {
                    src: "/content/themes/base/images/cabo-san-lucas/destination/cabo-san-lucas-08.jpg",
                    animation: "random",
                    transition: "swirlRight2"
                },
                {
                    src: "/content/themes/base/images/cancun/destination/cancun-03.jpg",
                    animation: "random",
                    transition: "swirlRight2"
                },
                {
                    src: "/content/themes/base/images/loreto/destination/loreto-02.jpg",
                    animation: "random",
                    transition: "swirlRight2"
                },
                {
                    src: "/content/themes/base/images/orlando/destination/orlando-06.jpg",
                    animation: "random",
                    transition: "swirlRight2"
                }
            ]
        });
    }
});