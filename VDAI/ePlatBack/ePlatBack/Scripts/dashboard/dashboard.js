$(function () {
    H.init();
});

var H = function () {
    var init = function () {
        //PENDING CLOSEOUT
        $('#divPendingCloseOuts').load('/home/GetPendingCloseOuts', function () {
            $('.agent-pos').on('click', function () {
                $(this).children().not('.pos-closeout').slideToggle('fast');
            });
        });

        //DAILY VOLUME
        $('#divDailyVolume').load('/home/GetDailyVolume/d', function () {
            UI.applyFormat('currency');
            $('#btnVolumeByDay').addClass('selected');
            setVolumeEvents();
        });        

        //DAILY COMMISSIONS
        $('#divDailyCommissions').load('/home/GetDailyCommissions', function () {
            UI.applyFormat('currency');
        });

        //EXCHANGE RATES
        $('#divExchangeRates').load('/home/GetExchangeRates', function () {
            UI.applyFormat('currency');
        });

        //HOSTESS STATUS
        $('#divHostessStatus').load('/home/GetHostessStatus', function () {
            
        });

        //AVAILABILITY STATUS
        $('#divAvailabilityStatus').load('/home/GetAvailabilityAlerts', function () {
            
        });

        //PENDING CACHE
        $('#divPendingCache').load('/home/GetPendingCache', function () {
            $('#btnGetCache').off('click').on('click', function () {
                $('#divPendingCache').html('');
                $('#divPendingCache').load('/home/GetPendingCacheReport', cacheEvents);
            });
        });        

        //ONLINE GOAL
        $('#divOnlineGoals').load('/home/GetOnlineGoal', function () {
            UI.applyFormat('currency');
        });
    }

    function cacheEvents () {
        let cacheIDs = $('#hdnPSIDs').val().split(',');
        if (cacheIDs.length === 0) {
            $('#btnRunCache').hide();
        } else {
            $('#btnRunCache').off('click').on('click', runCache);
        }
    };

    function runCache() {
        let cacheIDs = $('#hdnPSIDs').val().split(',');
        $('#btnRunCache').hide();
        $('#spnCachePercentage').text('');
        let interval = 1000;
        let i = 0;
        let counter = 0;
        let callback = function (n) {
            let id = cacheIDs[n];
            $.ajax({
                url: '/crm/Reports/ProcessCouponInfo/' + id,
                dataType: 'json',
                success: function (data) {
                    /*if (data.Processed) {
                    } else {
                    }*/
                }
            }).fail(function () {

            }).always(function () {
                counter++;
                $('#spnCachePercentage').text(counter + " de " + cacheIDs.length);
                if (counter === cacheIDs.length) {
                    $('#divPendingCache').html('');
                    $('#divPendingCache').load('/home/GetPendingCacheReport', cacheEvents);
                }
            });
        }
        next();
        function next() {
            if (callback.call(cacheIDs[i], i, cacheIDs[i]) !== false) {
                if (++i < cacheIDs.length) {
                    setTimeout(next, interval);
                }
            }
        }
    }

    function setVolumeEvents() {
        $('#btnVolumeByDay').on('click', function () {
            $('#divDailyVolume').html('');
            $('#divDailyVolume').load('/home/GetDailyVolume/d', function () {
                UI.applyFormat('currency');
                $('#btnVolumeByDay').addClass('selected');
                setVolumeEvents();
            });
        });
        $('#btnVolumeByYesterday').on('click', function () {
            $('#divDailyVolume').html('');
            $('#divDailyVolume').load('/home/GetDailyVolume/y', function () {
                UI.applyFormat('currency');
                $('#btnVolumeByYesterday').addClass('selected');
                setVolumeEvents();
            });
        });
        $('#btnVolumeByWeek').on('click', function () {
            $('#divDailyVolume').html('');
            $('#divDailyVolume').load('/home/GetDailyVolume/w', function () {
                UI.applyFormat('currency');
                $('#btnVolumeByWeek').addClass('selected');
                setVolumeEvents();
            });
        });
        $('#btnVolumeByMonth').on('click', function () {
            $('#divDailyVolume').html('');
            $('#divDailyVolume').load('/home/GetDailyVolume/m', function () {
                UI.applyFormat('currency');
                $('#btnVolumeByMonth').addClass('selected');
                setVolumeEvents();
            });
        });
        $('#btnVolumeByAgent').on('click', function () {
            $('.pos-volume').hide();
            $('.agent-volume').show();
            $('.program-volume').hide();

            $('#btnVolumeByAgent').addClass('selected');
            $('#btnVolumeByPos').removeClass('selected');
            $('#btnVolumeByProgram').removeClass('selected');
        });
        $('#btnVolumeByPos').on('click', function () {
            $('.pos-volume').show();
            $('.agent-volume').hide();
            $('.program-volume').hide();

            $('#btnVolumeByAgent').removeClass('selected');
            $('#btnVolumeByPos').addClass('selected');
            $('#btnVolumeByProgram').removeClass('selected');
        });
        $('#btnVolumeByProgram').on('click', function () {
            $('.pos-volume').hide();
            $('.agent-volume').hide();
            $('.program-volume').show();

            $('#btnVolumeByAgent').removeClass('selected');
            $('#btnVolumeByPos').removeClass('selected');
            $('#btnVolumeByProgram').addClass('selected');
        });
    }

    return {
        init: init
    }
} ();
