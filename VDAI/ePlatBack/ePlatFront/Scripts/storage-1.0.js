const Storage = {    
        supports_html5_storage: function supports_html5_storage() {
            try {
                return 'localStorage' in window && window['localStorage'] !== null;
            } catch (e) {
                return false;
            }
        },

        save: function save(name, value, days) {
            value = $.toJSON(value);
            if (Storage.supports_html5_storage) {
                localStorage["Eplat." + location.hostname + "." + name] = value;
            } else {
                Storage.Cookies.set("Eplat." + location.hostname + "." + name, value, days);
            }
        },

        get: function get(name) {
            var value = '';
            if (Storage.supports_html5_storage) {
                value = localStorage["Eplat." + location.hostname + "." + name];
            } else {
                value = Storage.Cookies.get("Eplat." + location.hostname + "." + name);
            }
            return eval("(" + value + ")");
        },

        Cookies: {
            set: function set(name, value, days) {
                if (days) {
                    var date = new Date();
                    date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                    var expires = "; expires=" + date.toGMTString();
                }
                else var expires = "";
                document.cookie = name + "=" + value + expires + "; path=/";
            },

            get: function get(name) {
                var nameEQ = name + "=";
                var ca = document.cookie.split(';');
                for (var i = 0; i < ca.length; i++) {
                    var c = ca[i];
                    while (c.charAt(0) == ' ') c = c.substring(1, c.length);
                    if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
                }
                return null;
            },

            clear: function clear(name) {
                Cookies.set(name, "", -1);
            }
        }
    }