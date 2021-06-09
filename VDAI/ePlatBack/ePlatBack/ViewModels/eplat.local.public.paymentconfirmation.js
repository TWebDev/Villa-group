var Confirmation = new Vue({
    el: '#app',
    data: {
        transaction: window.location.href.split('=')[1],
        transactionInfo: null,
        response: false,
        XhrRequest: null
    },
    methods: {
        getTransactionInfo: function () {
            let self = this;
            if (this.XhrRequest && this.XhrRequest.readyState != 4) {
                this.XhrRequest.abort();
            }
            this.XhrRequest = $.ajax({
                url: '/Public/GetPaymentConfirmationInfo',
                cache: false,
                data: { transaction: this.transaction.replace('#','') },
                success: function (data) {
                    self.transactionInfo = data;
                    Vue.nextTick(function () {
                        $('#signature').vgSign();
                        self.response = true;
                    })
                }
            });
        },
        xhr: function () {
            $(document).ajaxStart(function () {
                $('.progress').show();
                var count = 10;
                setInterval(function () {
                    if (parseInt($('.progress-bar').attr('style').split(':')[1].trim().substr(0, $('.progress-bar').attr('style').split(':')[1].trim().length - 1)) < 95) {
                        $('.progress-bar').css({ 'width': count + '%' });
                        count += 5;
                    }
                    else {
                        clearInterval();
                    }
                }, 1000);
            }).ajaxComplete(function () {
                $('.progress-bar').css({ 'width': '100%' });
            }).ajaxStop(function () {
                $('.progress').hide();
            });
        }
    },
    computed: {
        OptionsToJson: function () {
            if (this.transactionInfo.OptionsSold.indexOf('<') != -1) {
            //if (this.transactionInfo.OptionsSold != null && this.transactionInfo.OptionsSold.indexOf('<') != -1) {
                return null;
            }
            else {
                return $.parseJSON(this.transactionInfo.OptionsSold);
            }
        }
    },
    mounted: function () {
        let self = this;
        self.xhr();
        self.getTransactionInfo();
    }
})