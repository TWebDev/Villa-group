if (UTILS.isSetURLParameter('referredByID')) {
    $(window).unload(function () {
        if (window.opener != null) {
            window.close();
        }
    });
}
