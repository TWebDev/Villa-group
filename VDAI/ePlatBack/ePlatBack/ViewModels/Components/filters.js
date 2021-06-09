import Vue from 'vue';

Vue.filter('capitalize', function (value) {
    if (!value) {
        return '';
    } else {
        value = value.toString();
        value = value.toLowerCase();
        value = value.charAt(0).toUpperCase() + value.slice(1);
        return value;
    }
});

Vue.filter('dateYYYYMMDD', function (value) {
    if (!value) return '';
    let re = /-?\d+/;
    let m = re.exec(value);
    let d = new Date(parseInt(m[0]));
    return moment(d).format('YYYY-MM-DD');
});

Vue.filter('dateYYYYMMDDhhmmss', function (value) {
    if (!value) return '';
    let re = /-?\d+/;
    let m = re.exec(value);
    let d = new Date(parseInt(m[0]));
    return moment(d).format('YYYY-MM-DD hh:mm:ss A');
});

Vue.filter('bool-to-string', function (value) {
    if (!value) return 'No';
    else return 'Yes';
});

Vue.filter('currency', function (value) {
    //return '$' + value.toFixed(2)
    let amount = parseFloat(value).toFixed(2);
    var parts = amount.toString().split(".");
    parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return '$' + parts.join(".");
});

Vue.filter('breakLines', function (value) {
    return value.replace('\n', '<br />');
});

Vue.filter('maskedPhone', function (value) {
    if (value) {
        let regex = /(\d+)/g;
        let phoneNumbers = value.match(regex).join();
        if (phoneNumbers !== null && phoneNumbers.length >= 10) {
            return phoneNumbers.substr(0, 3) + " ••• ••" + phoneNumbers.slice(-2);
        }
    }
    return value;
});

Vue.filter('maskedEmail', function (value) {
    if (value) {
        if (value.indexOf("@") >= 0) {
            return value.substr(0, 2) + '•' + value.substr(value.indexOf("@"));
        }
    }
    return value;
});

Vue.filter('removeHtml', function (value) {
    if (value) {
        return value.replace(/<[^>]*>/g, '').replace(/\n/g, '');
    }
    return value;
});

Vue.filter('highlight', function (value, text) {
    if (value && text) {
        return value
            .replace(text, '<mark>' + text + '</mark>')
            .replace(text.charAt(0).toUpperCase() + text.slice(1), '<mark>' + text.charAt(0).toUpperCase() + text.slice(1) + '</mark>');
    }
    return value;
});


Vue.filter('formatMailDate', function (value) {
    if (!value) return '';
    let re = /-?\d+/;
    let m = re.exec(value);
    let d = new Date(parseInt(m[0]));
    if (moment(d).format('YYYY-MM-DD') == moment().format('YYYY-MM-DD')) {
        return moment(value).format('hh:mm A');
    } else {
        return moment(d).format('dddd MM-DD');
    }
});

