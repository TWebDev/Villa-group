var PageContainer = new Vue({
    mixins: [ePlatUtils],
    el: '#app',
    data: {
        Shared: ePlatStore,
    },
    methods: {
        setFrameSize: function () {
            $('#pageContainer').width(window.innerWidth - 64);
            $('#pageContainer').height(780 * (window.innerWidth - 64) / 1294);
        }
    },
    mounted: function () {
        //iniciar la sesión
        let self = this;
        this.Session().getSessionDetails();

        $(window).resize(function () {
            self.setFrameSize();
        });
        this.setFrameSize();
    }
});