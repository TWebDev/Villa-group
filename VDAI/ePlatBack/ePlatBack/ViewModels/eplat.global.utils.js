var ePlatUtils = {
    data: function () {
        return {
            UIData: {
                showSearchCard: false
            }
        }
    },
    methods: {
        UI: function () {
            self = this;
            return {
                showSearchCard: function () {
                    if (self.UIData.showSearchCard) {
                        self.UIData.showSearchCard = false
                    } else {
                        self.UIData.showSearchCard = true
                    }
                },
                scrollTo: function (elementID, scrollingDuration) {
                    if (scrollingDuration == null) { scrollingDuration = 500 }
                    //let headerHeight = parseInt($("header").height());
                    let navHeight = parseInt($("nav").height());
                    let currentOffSet = $("#" + elementID).offset().top;
                    let newScrollTop = currentOffSet - navHeight;
                    if ($('.fixed-panel').length > 0 && newScrollTop > 1120) {
                        newScrollTop -= 120;
                    }
                    $('html, body').animate({ scrollTop: newScrollTop }, scrollingDuration);
                },
                loadMenu: function () {
                    var url = $(location).attr('protocol') + '//' + $(location).attr('host');
                    $.ajax({
                        url: '/Account/GetMenuComponents',
                        type: 'GET',
                        data: {
                            selectedWorkGroupID: self.Shared.Session.WorkGroupID,
                            selectedRoleID: self.Shared.Session.RoleID
                        },
                        success: function (data) {
                            let builder = '';
                            let flag;
                            $.each(data, function (index, item) {
                                if (data[index].SysComponentTypeID == 1) {
                                    flag = false;
                                    if ((item.SysComponentUrl != undefined && item.SysComponentUrl != null) || !hasChildren(item.SysComponentID)) {
                                        builder += '<li class="nav-item">';
                                        builder += '<a class="nav-link" href="' + item.SysComponentUrl + '">' + item.SysComponentName + '</a>';
                                    }
                                    else {
                                        builder += '<li class="nav-item dropdown">';
                                        builder += '<a class="nav-link dropdown-toggle" href="#" id="m_' + item.SysComponentID + '" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">' + item.SysComponentName + '</a>';
                                    }
                                    builder += '<ul class="dropdown-menu" aria-labelledby="m_' + item.SysComponentID + '">';
                                    recursive(item.SysComponentID);
                                }
                            });
                            function hasChildren(componentID) {
                                let has = false;
                                $.each(data, function (index2, item2) {
                                    if (item2.SysParentComponentID == componentID) {
                                        has = true;
                                        return false;
                                    }
                                });
                                return has;
                            }
                            function recursive(componentID) {
                                $.each(data, function (index2, item2) {
                                    if (item2.SysParentComponentID == componentID) {
                                        flag = true;
                                    }
                                });
                                if (flag == false) {//has children
                                    builder += '</ul></li>';
                                }
                                else {
                                    $.each(data, function (index2, item2) {
                                        if (item2.SysParentComponentID == componentID) {
                                            if ((item2.SysComponentUrl != undefined && item2.SysComponentUrl != null) || !hasChildren(item2.SysComponentID)) {
                                                builder += '<li>';
                                                builder += '<a class="dropdown-item" href="' + url + item2.SysComponentUrl + '">' + item2.SysComponentName.toString().trim() + '</a>';
                                            }
                                            else {
                                                builder += '<li class="dropdown-submenu">';
                                                builder += '<a class="dropdown-item dropdown-toggle" href="#">' + item2.SysComponentName.toString().trim() + '</a>';
                                            }
                                            builder += '<ul class="dropdown-menu submenu">';
                                            recursive(item2.SysComponentID);
                                        }
                                    });
                                    builder += '</ul></li>';
                                }
                                return builder;
                            }
                            $('#menu').empty();
                            while (builder.indexOf('<ul class="dropdown-menu submenu"></ul>') > 0) {
                                builder = builder.substr(0, builder.indexOf('<ul class="dropdown-menu submenu"></ul>')) + builder.substr(builder.indexOf('<ul class="dropdown-menu submenu"></ul>') + 39, builder.length);
                            }
                            $('#menu').append(builder);
                            $('.dropdown-menu a.dropdown-toggle').off('click').on('click', function (e) {
                                if (!$(this).next().hasClass('show')) {
                                    $(this).parents('.dropdown-menu').first().find('.show').removeClass("show");
                                }
                                var $subMenu = $(this).next(".dropdown-menu");
                                $subMenu.toggleClass('show');


                                $(this).parents('li.nav-item.dropdown.show').on('hidden.bs.dropdown', function (e) {
                                    $('.dropdown-submenu .show').removeClass("show");
                                });

                                return false;
                            });
                        }
                    });
                },
                setEvents: function () {
                    $('.eplat-tool').off('click').on('click', function () {
                        $('.sidebar').addClass('d-none');
                        $('#' + $(this).attr('title')).removeClass('d-none');
                        $('#main').addClass('col-md-8 col-lg-9');
                    });
                    $('.sidebar .close').off('click').on('click', function () {
                        $('.sidebar').addClass('d-none');
                        $('#main').removeClass('col-md-8 col-lg-9');
                    });
                },
                updateSelectedTerminals: function () {
                    if (self.Shared.Session.TerminalIDsArr.length > 0) {
                        self.Shared.Session.TerminalIDs = self.Shared.Session.TerminalIDsArr.join(',');
                        //obtener WorkGroup y RoleID
                        let tid = self.Shared.Session.TerminalIDsArr[0];
                        let objT = self.Shared.Session.UserTerminals.filter(function (terminal) {
                            return terminal.TerminalID == tid;
                        });
                        self.Shared.Session.WorkGroupID = objT[0].WorkGroupID;
                        self.Shared.Session.WorkGroup = objT[0].WorkGroup;
                        self.Shared.Session.RoleID = objT[0].RoleID;
                        self.Shared.Session.Role = objT[0].Role;
                        //actualizar menu
                        if (self.Shared.Session.WorkGroupID != localStorage.Eplat_SelectedWorkGroupID) {
                            self.UI().loadMenu();
                        }
                        //guardar ticket
                        self.Session().saveTicket(function () {
                            localStorage.Eplat_SelectedTerminals = self.Shared.Session.TerminalIDs;
                            localStorage.Eplat_SelectedWorkGroupID = self.Shared.Session.WorkGroupID;
                            localStorage.Eplat_SelectedRole = self.Shared.Session.RoleID;
                        });

                        self.UI().getSelectedTerminalsNames();
                        self.UI().loadTerminalDependentLists();
                        $('#btnDropTerminals').trigger('click');

                    } else {
                        alert('You need to select 1 terminal at least');
                    }
                },
                selectedTerminal: function (terminalID, roleID, workgroupID) {
                    if (self.Shared.Session.TerminalIDsArr.length == 0) {
                        $.each(self.Shared.Session.UserTerminals, function (i, terminal) {
                            terminal.Visible = true;
                        });
                    } else if (self.Shared.Session.TerminalIDsArr.length >= 1) {
                        $.each(self.Shared.Session.UserTerminals, function (i, terminal) {
                            terminal.Visible = (terminal.WorkGroupID == workgroupID ? true : false);
                        });
                    }
                },
                loadTerminalDependentLists: function () {
                    let terminalsList = [];
                    if (self.Shared.Session.UserTerminals.length > 0) {
                        let Terminals = self.Shared.Session.UserTerminals.filter(function (item) {
                            return self.Shared.Session.TerminalIDs.split(',').indexOf(item.TerminalID.toString()) >= 0;
                        })

                        $.each(Terminals, function (t, terminal) {
                            terminalsList.push({
                                label: terminal.Terminal,
                                title: terminal.Terminal,
                                value: terminal.TerminalID
                            });
                        });
                    }
                    self.Shared.Session.TerminalsList = terminalsList;
                    $('.terminal-dependent-list[multiple="multiple"]').multiselect('dataprovider', self.Shared.Session.TerminalsList);
                    $('body').trigger('selectedTerminalChanged');
                },
                getSelectedTerminalsNames: function () {
                    if (self.Shared.Session.TerminalIDsArr.length == 1) {
                        let tid = parseInt(self.Shared.Session.TerminalIDsArr[0]);
                        let currentTerminal = self.Shared.Session.UserTerminals.filter(function (terminal) {
                            return terminal.TerminalID == tid;
                        });
                        self.Shared.Session.SelectedTerminalsNames = currentTerminal[0].Terminal;
                    } else {
                        self.Shared.Session.SelectedTerminalsNames = self.Shared.Session.TerminalIDsArr.length + ' Selected Terminal' + (self.Shared.Session.TerminalIDsArr.length > 1 ? 's' : '');
                    }
                },
                stripTags: function (str) {
                    return str.replace(/<\/?[^>]+>/gi, '');
                },
                loadDependentFields: function (url, updateOnTerminalChange, callback) {
                    self.Shared.State.DependentFields.Url = url;
                    self.Shared.State.DependentFields.UpdateOnTerminalChange = updateOnTerminalChange;
                    $.ajax({
                        url: url,
                        cache: false,
                        type: 'GET',
                        success: function (data) {
                            self.Shared.State.DependentFields.Fields = data.Fields;
                            $.each(self.Shared.State.DependentFields.Fields, function (f, field) {
                                $('#' + field.ParentField).on('change', function () {
                                    var options = '';
                                    var parentValue = $(this).val();
                                    var validFields = 0;
                                    $('#' + field.Field).html('');
                                    $.each(field.Values, function (v, value) {
                                        var grandParentValue = $('#' + field.GrandParentField).val();
                                        if (value.GrandParentValue == grandParentValue || value.GrandParentValue == null) {
                                            if (value.ParentValue == parentValue || value.ParentValue == null) {
                                                if (value.Value != '') {
                                                    validFields += 1;
                                                }
                                                options += '<option value="' + value.Value + '">' + value.Text + '</option>';
                                            }
                                        }
                                    });
                                    $('#' + field.Field).html(options);
                                    if ($('#' + field.Field).prop('multiple')) {
                                        $('[multiple="multiple"]').multiselect('rebuild');
                                    }
                                    if (validFields == 1) {
                                        $('#' + field.Field + ' option').eq(1).prop('selected', true).trigger('change');
                                    }
                                });
                            });
                            if (typeof callback == 'function') {
                                callback();
                            }
                        }
                    });
                },
                filterTarget: function (url) {
                    //eliminar dominio
                    if (url.indexOf('//') >= 0) {
                        url = url.replace('//', '');
                        url = url.substr(url.indexOf('/'));
                    }
                    //reemplazar null, números y arreglos de los parámetros
                    var params = url.split('&');
                    var newUrl = '';
                    $.each(params, function (s, string) {
                        var value = string.substr(string.indexOf('=') + 1);
                        if (value == '') {
                            newUrl += (newUrl != "" ? "&" : "") + string + 'x';
                        }
                        else if (!isNaN(value) || value.indexOf(',') >= 0 || value.indexOf('-') >= 0 || value.indexOf('%7C') >= 0 || value == "null" || value == "true" || value == "false") {
                            newUrl += (newUrl != "" ? "&" : "") + string.replace(value, 'x');
                        } else {
                            newUrl += (newUrl != "" ? "&" : "") + string;
                        }                        
                    });
                    //reemplazar /x/x/id
                    var folders = newUrl.split('/');
                    var newUrl2 = '';
                    $.each(folders, function (s, string) {
                        if (string != '') {
                            var pathParams = string.split('?');
                            if (!isNaN(pathParams[0])) {
                                newUrl2 += '/x';
                            } else {
                                newUrl2 += '/' + pathParams[0];
                            }
                            if (pathParams.length > 1) {
                                newUrl2 += '?' + pathParams[1];
                            }
                        }
                    });

                    url = newUrl2;
                    return url;
                },
                xhrTargets: function () {
                    $(document).ajaxSend(
                        function (evt, request, settings) {
                            //$.blockUI();
                            var registered = false;

                            $.each(RequestTargets, function (r, request) {
                                //console.log(self.UI().filterTarget(settings.url));
                                if (request.url == self.UI().filterTarget(settings.url)) {
                                    //console.log(request.url);
                                    registered = true;
                                    if (request.target == '') {
                                        //no mostrar status
                                    }
                                    else if (request.target == 'StatusBar') {
                                        if ($('#statusBar .message span[data-xhr="' + request.url + '"]').length == 0) {
                                            $('#statusBar .message').append('<span data-xhr="' + request.url + '">' + request.message + '...</span>');
                                            $('#statusBar .message').addClass('loading');
                                            $('#statusBar').fadeIn('fast');
                                        }
                                    } else {
                                        $('#' + request.target).before('<div class="loading ldg ldg-' + request.target + '"></div>');
                                        $('.ldg-' + request.target).width($('#' + request.target).width());
                                        $('.ldg-' + request.target).height($('#' + request.target).height());
                                    }
                                    if (request.block != '') {
                                        $.each(request.block.split(','), function (index, item) {
                                            if (item.indexOf('.') > -1) {
                                                $(item).attr('disabled', 'disabled');
                                            }
                                            else {
                                                $('#' + item).attr('disabled', 'disabled');
                                            }
                                        });
                                    }
                                }
                            });
                            if (!registered) {
                                //no registrado
                                //console.log('xhr: ' + self.UI().filterTarget(settings.url));
                            }
                        }).ajaxComplete(
                        //$.unblockUI
                        function (evt, request, settings) {
                            $.each(RequestTargets, function (r, request) {
                                if (request.url == self.UI().filterTarget(settings.url)) {
                                    if (request.target == 'StatusBar') {
                                        $('#statusBar .message span[data-xhr="' + request.url + '"]').fadeOut('fast', function () {
                                            $(this).remove();
                                            if ($('#statusBar .message span').length == 0) {
                                                $('#statusBar').removeClass('loading').fadeOut('fast');
                                            }
                                        });
                                    } else {
                                        $('.ldg-' + request.target).fadeOut('fast', function () { $(this).remove(); });
                                    }
                                    if (request.block != '') {
                                        $.each(request.block.split(','), function (index, item) {
                                            if (item.indexOf('.') > -1) {
                                                $(item).removeAttr('disabled');
                                            }
                                            else {
                                                $('#' + item).removeAttr('disabled');
                                            }
                                        });
                                    }
                                }
                            });
                            //UI.exportToExcel();
                        }
                    ).ajaxStop(function () {
                        $('#statusBar .message').html('');
                        $('#statusBar').removeClass('loading').fadeOut('fast');
                    });
                },
                exportToExcel: function (buttonID, tableID) {
                    //renderizar imagen dentro del botton
                    var tableHTML;
                    var img;
                    if (buttonID != undefined || buttonID != null) {
                        img = '<img src="/content/images/xls.png" class="export-excel non-printable" style="width:20px;" id="btn"' + '" alt="Export to Excel" title="Export to Excel" />';
                        var newButton = '<button id="btnGetExcelReport"' + 'class="btn btn-sm" ' + 'v-if="showExportable"' + '>' + img + '</button>';
                        $('#' + buttonID).replaceWith(newButton);
                        $('#btnGetExcelReport').on('click', function () {
                            var fileName = $('h1').eq(0).html();
                            var clone = $('#' + tableID).clone();
                            tableHTML = clone.html();
                            tableHTML = tableHTML.replace(/<span .*?class="(.*?rule.*?)">(.*?)<\/span>/gi, "");
                            tableHTML = tableHTML.replace(/'/g, "");
                            while (tableHTML.indexOf('style="display:none;"', "") != -1)
                            {
                                tableHTML = tableHTML.replace('style="display:none;"', '');
                            }
                            var iframe = $("<iframe id='frmExcelReport' style='display: none' src='about:blank'></iframe>").appendTo("body");
                            var formDoc = iframe[0].contentWindow || iframe[0].contentDocument;
                            if (formDoc.document) {
                                formDoc = formDoc.document;
                            }
                            formDoc.write("<html><head><title>" + fileName + '.xls' +
                                "</title></head><body><form method='post' action='/crm/Reports/GetExcelFile?Length=3'><input type='hidden' id='content' name='content' value='" +
                                '<table border="1">' + tableHTML + '</table>' + "' /><input type='hidden' id='filename' name='filename' value='" + fileName +
                                "' /><input type='hidden' id='time' name='time' value='" + new Date().getTime() + "' /></form></body></html>");
                            var form = $(formDoc).find('form');
                            form.submit();
                        });
                    }
                }
            }
        },
        Session: function () {
            self = this;
            return {
                getSessionDetails: function () {
                    self.UI().xhrTargets();
                    self.UI().setEvents();
                    self.UI().loadMenu();
                    $.ajax({
                        url: '/Account/SessionDetails',
                        type: 'GET',
                        cache: false,
                        success: function (data) {
                            //Guardar los datos del usuario en Shared
                            self.Shared.Session.TerminalIDs = data.TerminalIDs;
                            self.Shared.Session.TerminalIDsArr = data.TerminalIDs.split(',');
                            self.Shared.Session.WorkGroupID = data.WorkGroupID;
                            self.Shared.Session.WorkGroup = data.WorkGroup;
                            self.Shared.Session.RoleID = data.RoleID;
                            self.Shared.Session.Role = data.Role;
                            self.Shared.Session.Username = data.Username;
                            self.Shared.Session.FirstName = data.FirstName;
                            self.Shared.Session.LastName = data.LastName;
                            self.Shared.Session.Photo = data.Photo;
                            self.Shared.Session.UserTerminals = data.UserTerminals;

                            self.UI().getSelectedTerminalsNames();
                            self.UI().loadTerminalDependentLists();
                            self.UI().selectedTerminal(self.Shared.Session.TerminalIDsArr[0], self.Shared.Session.RoleID, self.Shared.Session.WorkGroupID);
                        }
                    });
                },
                saveTicket: function (callback) {
                    $.ajax({
                        url: '/Account/SaveTicket',
                        type: 'POST',
                        cache: false,
                        data: {
                            workGroupID: self.Shared.Session.WorkGroupID,
                            roleID: self.Shared.Session.RoleID,
                            terminals: self.Shared.Session.TerminalIDs
                        },
                        success: function (data) {
                            if (data.ok != undefined) {
                                if (typeof callback == 'function') {
                                    callback();
                                }
                            }
                            else {
                                $('#aLogOff').click();
                            }
                        }
                    });
                }
            }
        },
        DateFormat: function () {
            return {
                toDateYYYYMMDD: function (value) {
                    //if (!value || value.indexOf("Date") == -1) return value;
                    //let re = /-?\d+/;
                    //let m = re.exec(value);
                    //let d = new Date(parseInt(m[0]));
                    return moment(value).format('YYYY-MM-DD');
                }
            }
        },
        Filters: function () {
            return {
                currency: function (value, moneyChar) {
                    if (value) {
                        let amount = value.toFixed(2);
                        var parts = amount.toString().split(".");
                        parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                        return moneyChar + parts.join(".");
                    }
                    return value;
                },
                currencyToDecimal: function (value) {
                    if (value) {
                        return value.replace('$', '').replace(/,/g, '');
                    }
                    return value;
                },
                maskedPhone: function (value) {
                    if (value) {
                        let regex = /(\d+)/g;
                        let phoneNumbers = value.match(regex).join();
                        if (phoneNumbers != null && phoneNumbers.length >= 10) {
                            return phoneNumbers.substr(0, 3) + " ••• ••" + phoneNumbers.slice(-2);
                        }
                    }
                    return value;
                },
                maskedEmail: function (value) {
                    if (value) {
                        if (value.indexOf("@") >= 0) {
                            return value.substr(0, 2) + '•' + value.substr(value.indexOf("@"));
                        }
                    }
                    return value;
                },
                breakLines: function (value) {
                    if (value) {
                        value = value.replace(/\n/g, '<br />');
                    }
                    return value;
                },
                filterHtmlMail: function (value) {
                    if (value) {
                        if (value.indexOf('<body') >= 0) {
                            var pattern = /<body[^>]*>((.|[\n\r])*)<\/body>/im;
                            var array_matches = pattern.exec(value);
                            value = array_matches[1];
                            value = value.replace(/\n/g, ' ');
                        }
                    }
                    return value;
                },
                highlight: function (value, text) {
                    if (value && text) {
                        return value
                            .replace(text, '<mark>' + text + '</mark>')
                            .replace(text.charAt(0).toUpperCase() + text.slice(1), '<mark>' + text.charAt(0).toUpperCase() + text.slice(1) + '</mark>');
                    }
                    return value;
                }
            }
        },
        Forms: function () {
            self = this;
            return {
                showValidationSummary: function (frmID) {
                    var target = $("#" + frmID);
                    if (!$("#" + frmID).valid()) {
                        var errorsHolder = "#";
                        if (!$(target).is("form")) {
                            errorsHolder += $("#" + frmID).parents("form").attr("id");
                        } else {
                            errorsHolder += frmID;
                        }

                        errorsHolder += "_ErrorMessages";
                        var validationSummary = "There were some validation errors:";

                        //get validationMessages when @Html.ValidationSummary(false)
                        var selector = errorsHolder + " div.validation-summary-errors";
                        if ($(selector).length > 0) {
                            validationSummary += $(selector).html();
                        }
                        else { //get validationMessages when @Html.ValidationSummary(true)    
                            validationSummary += "<ul>";
                            $(errorsHolder + " > span").each(
                                function (index) {
                                    validationSummary += "<li>";
                                    validationSummary += self.UI().stripTags($(this).html());
                                    validationSummary += "</li>";
                                });
                            validationSummary += "</ul>"
                        }
                        validationSummary = validationSummary.replace(/<li><\/li>/g, '');
                        console.log(validationSummary);
                        $.alert({
                            title: 'Error Saving',
                            content: validationSummary,
                            animation: 'zoom',
                            closeAnimation: 'scale',
                            type: 'red'
                        });
                    }
                }
            }
        },
        Guid: function () {
            let self = this;
            return {
                G4: function () {
                    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substr(1)
                },
                newGuid: function () {
                    return (self.Guid().G4() + self.Guid().G4() + "-" + self.Guid().G4() + "-" + self.Guid().G4() + "-" + self.Guid().G4() + "-" + self.Guid().G4() + self.Guid().G4() + self.Guid().G4());
                }
            }
        }
    }
}
