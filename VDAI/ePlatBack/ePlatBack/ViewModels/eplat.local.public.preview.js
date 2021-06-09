
var Email = new Vue({
    el: '#app',
    data: {
        preview: [],
        content: {}
        //preview: window.location.href.split('=')[1]
    },
    methods: {
        loadPreview: function () {
            this.preview = $.parseJSON(localStorage.Eplat_EmailPreview);
            this.send($.parseJSON(localStorage.Eplat_EmailPreviewParams));
        },
        send: function (params) {
            let self = this;
            
            $('.send').unbind('click').on('click', function () {
                $.ajax({
                    url: '/PreArrival/SendEmail',
                    type: 'POST',
                    cache: false,
                    data:{model: JSON.stringify(self.preview) },
                    success: function (data) {
                        alert(data.ResponseMessage);
                        window.parent.postMessage(data.ResponseMessage);
                        $('.fancybox-close').click();

                    }
                });
            });
        }
    },
    computed: {
        OptionsToJson: function () {
            if (this.preview.length > 0) {
                if (this.preview.OptionsSold.indexOf('<') != -1) {
                    return null;
                }
                else {
                    return $.parseJSON(this.preview.OptionsSold);
                }
            }
            return null;
        }
    },
    mounted: function () {
        let self = this;
        self.loadPreview();

    }
})