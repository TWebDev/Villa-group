var Tickets = function () {
    var List;
    var init = function () {
        $('#btnSupportTickets').on('click', function () {
            $.fancybox.open([
                {
                    type: 'ajax',
                    href: '/Home/Tickets'
                }
            ], {
                'width': 640,
                'height': 600,
                'autoSize': false,
                'openEffect': 'fadeIn',
                'closeEffect': 'fadeOut',
                'openEasing': 'easeOutBack',
                'closeEasing': 'easeInBack',
                'title': null,
                'closeBtn': true,
                'afterShow': function () {
                    //seleccionar búsqueda default
                    $('#Search_SupportTicketStatusID option').each(function () {
                        if ($(this).val() < 6) {
                            $(this).prop('selected', true);
                        }                        
                    });
                    $('#Search_SupportTicketStatusID').multiselect({
                        noneSelectedText: "All",
                        minWidth: "auto", selectedList: 1
                    }).multiselectfilter();
                    //iniciar búsqueda
                    $('#frmSearchTickets').submit();
                    $('#fdsTicketInfo legend').on('click', function () {
                        $('#divTicketsResults').slideDown('fast');
                        $('#fdsTicketInfo').find('div').slideUp('fast');
                    });
                    $('#btnNewTicketInfo').on('click', function () {
                        $('#supportTicketID').val('00000000-0000-0000-0000-000000000000');
                        $('#subject').val('');
                        $('#body').val('');
                        $('#terminalID').val('');
                        $('#reference').val('');
                        $('#assignedToUserID').val('');
                        $('#changeset').val('');
                        $('#SupportTicketStatusID').val('1');
                        $('#StatusComment').val('');
                        $('#divStatusLog').html('');

                        $('#subject').prop('disabled', false);
                        $('#body').prop('disabled', false);
                        $('#terminalID').prop('disabled', false);
                        $('#reference').prop('disabled', false);

                        $('#SupportTicketStatusID option').show();
                        
                        $('#fdsTicketInfo').find('div').slideDown('fast');
                        $('#divTicketsResults').slideUp('fast');
                    });
                    $('#assignedToUserID').on('change', function () {
                        $('#SupportTicketStatusID').val('2');
                    });
                    UI.adjustLegends();
                },
                'beforeShow': function () {
                    this.wrap.draggable();
                    $('.fancybox-overlay').css({
                        'overflow': 'hidden',
                        'overflow-y': 'hidden',
                    });
                }
            });
        });

        $("#ulUnassignedTickets, #ulTickets").sortable({
            update: function (event, ui) {
                updateMyTickets(1);
            },
            connectWith: ".connectedSortable"
        }).disableSelection();

        $('#btnQuickTicket').on('click', function () {
            if ($('#txtQuickSubject').val() != "" || $('#txtQuickBody').val() != "") {
                if (ePlatHubConnected) {
                    $('#imgAsignedTickets').show();
                    ePlatHub.server.quickSaveTicket($('#txtQuickSubject').val(), $('#txtQuickBody').val(), $('#ulTickets li').length);
                    $('#txtQuickSubject').val('');
                    $('#txtQuickBody').val('');
                }
            }            
        });
    }

    var loaded = function (data) {
        Tickets.List = data;
        var table = '<table id="tblTickets" style="width:100%; display:none; font-size: .9em;" class="table">'
            + '<thead>'
            + '<tr>'
            + '<th>ID</th>'
            + '<th>Ticket</th>'
            + '<th>Requested by</th>'
            + '<th>Requested on</th>'
            + '<th>Assigned to</th>'
            + '<th>Status</th>'
            + '</tr>'
            + '</thead><tbody>';
        $.each(Tickets.List, function (t, ticket) {
            table += '<tr id="' + ticket.supportTicketID + '">'
            + '<td>' + ticket.supportTicketID.substr(30) + '</td>'
            + '<td>' + ticket.subject + '</td>'
            + '<td>' + ticket.RequestedByUser + '</td>'
            + '<td style="width: 90px;">' + ticket.StatusHistory[0].Date + '</td>'
            + '<td>' + ticket.AssignedToUser + '</td>'
            + '<td ' + (ticket.SupportTicketStatus == 'In Process' ? 'class="mb-warning"' : '') + (ticket.SupportTicketStatus == 'Ready to Publish' ? 'class="mb-confirmation"' : '') + '>' + ticket.SupportTicketStatus + '</td>'
            + '</tr>';
        });
        table += '</tbody></table>';
        $('.ticket-searching').hide();
        $('#divTicketsResults').html(table);
        $('#tblTickets').show();
        UI.adjustLegends();
        UI.tablesHoverEffect();
        $('#tblTickets').find('tbody tr').unbind('click').bind('click', function (e) {
                $(this).parent().find('.selected-row').removeClass('selected-row primary');
                $(this).addClass('selected-row primary');
                var tid = $(this).attr('id');
            //cargar info en formulario
                $.each(Tickets.List, function (t, ticket) {
                    if (ticket.supportTicketID == tid) {
                        $('#supportTicketID').val(ticket.supportTicketID);
                        $('#subject').val(ticket.subject);
                        $('#body').val(ticket.body);
                        $('#terminalID').val(ticket.terminalID);
                        $('#reference').val(ticket.reference);
                        $('#assignedToUserID').val(ticket.assignedToUserID);
                        $('#changeset').val(ticket.changeset);
                        $('#SupportTicketStatusID').val(ticket.SupportTicketStatusID);
                        $('#StatusComment').val(ticket.StatusComment);
                        var log = '';
                        $.each(ticket.StatusHistory, function (s, status) {
                            log += '<div class="ticket-status">'
                                + '<div class="ticket-status-state">' + status.SupportTicketStatus + (status.SupportTicketStatus == "Assigned" ? " to " + ticket.AssignedToUser : "") + '</div>'
                                + '<div class="ticket-status-date">' + status.Date + '</div>'
                                + '<div class="ticket-status-user">' + status.SavedByUser + '</div>'
                                + '<div class="ticket-status-comments">' + (status.Comments != null ? status.Comments : '') + '</div>'
                                + '</div>';
                        });
                        $('#divStatusLog').html(log);

                        for (var i = 1; i < ticket.SupportTicketStatusID; i++) {
                            $('#SupportTicketStatusID option[value="' + i + '"]').hide();
                        }

                        if (ticket.SupportTicketStatusID == 6) {
                            $('#subject').prop('disabled', true);
                            $('#body').prop('disabled', true);
                            $('#terminalID').prop('disabled', true);
                            $('#reference').prop('disabled', true);
                        }
                    }
                });
                $('#subject').val()
                $('#divTicketsResults').slideUp('fast');
                $('#fdsTicketInfo').find('div').slideDown('fast');
        });
    }

    var saveTicketSuccess = function (data) {
        //notificar nuevo ticket
        $('#frmSearchTickets').submit();
        $('#divTicketsResults').slideDown('fast');
        $('#fdsTicketInfo').find('div').slideUp('fast');
    }

    var seachBegin = function () {
        $('#tblTickets').hide();
        $('#divTicketsResults').html('<span class="ticket-searching" style="display:none;"><img src="/images/loading.gif" style="width:15px;"/> Searching for Tickets...</span>');
        $('.ticket-searching').show();
    }

    var renderTickets = function (tickets) {
        $('#ulTickets').html('');
        $('#ulUnassignedTickets').html('');
        $.each(tickets, function (t, ticket) {
            var ticketStr = '<li class="li-ticket" data-ticketid="' + ticket.supportTicketID + '">'
                + '<span class="li-ticket-subject">' + ticket.subject + '</span>'
                + '<span class="li-ticket-body" style="display: none;">' + ticket.body + '</span>'
                + '<span class="li-ticket-info" style="display: none;">' + ticket.supportTicketID.substr(0, 6) + ' : Status ' + ticket.SupportTicketStatus + ' : ' + ticket.RequestedByUser + ' <br> ' + ticket.StatusHistory[0].Date + '</span>'
                + '<span class="li-ticket-status"><input type="checkbox" class="assigned chk-ticket-status" title="Assigned" /><input type="checkbox" class="in-process chk-ticket-status" title="In Process" /><input type="checkbox" class="stand-by chk-ticket-status" title="Stand By" /><input type="checkbox" class="ready-to-publish chk-ticket-status" title="Ready to Publish" /><input type="checkbox" class="completed chk-ticket-status" title="Completed" /></span>'
                + '</li>';
            if (ticket.assignedToUserID != null) {
                $('#ulTickets').append(ticketStr);
            } else {
                $('#ulUnassignedTickets').append(ticketStr);
            }
            //llenado de chk
            $.each(ticket.StatusHistory, function (s, status) {
                if (status.SupportTicketStatus != 'Stand By' || (status.SupportTicketStatus == 'Stand By' || s == ticket.StatusHistory.length - 1)) {
                    $('li[data-ticketid="' + ticket.supportTicketID + '"] .' + status.SupportTicketStatus.toLowerCase().replace(' ', '-')).attr('checked', 'checked');
                }                
            })
        });
        $('.li-ticket-subject').off('click').on('click', function () {
            $(this).parent().find('.li-ticket-body').slideToggle('fast');
            $(this).parent().find('.li-ticket-info').slideToggle('fast');
        })
        $('.chk-ticket-status').off('click').on('click', function () {
            var index = $("#ulTickets li").index($(this).closest('li'));;
            if ($(this).closest('#ulUnassignedTickets').length) {
                //asignar
                $(this).closest('li').insertAfter('#ulTickets li:last');
                index = $('#ulTickets li').length - 1;
            }
            updateMyTickets(2, index);
        });
        $('#imgUnasignedTickets, #imgAsignedTickets').hide();
    }

    function updateMyTickets(mode, index) {
        $('#imgAsignedTickets').show();
        var Tickets = [];
        if (mode == 1) { //order
            $('#ulTickets .li-ticket').each(function (t) {
                var id = $(this).attr('data-ticketid');
                var cTicket = {
                    SupportTicketID: id,
                    Order: t,
                    Status: []
                };
                cTicket.Status.push(1);
                $(this).find('.chk-ticket-status:checked').each(function (s) {
                    if ($(this).hasClass('assigned')) {
                        cTicket.Status.push(2);
                    } else if ($(this).hasClass('in-process')) {
                        cTicket.Status.push(3);
                    } else if ($(this).hasClass('stand-by')) {
                        cTicket.Status.push(4);
                    } else if ($(this).hasClass('ready-to-publish')) {
                        cTicket.Status.push(5);
                    } else if ($(this).hasClass('completed')) {
                        cTicket.Status.push(6);
                    }
                });
                Tickets.push(cTicket);
            });
        } else if (mode == 2) {
            var cTicket = {
                SupportTicketID: $("#ulTickets li").eq(index).attr('data-ticketid'),
                Order: index,
                Status: []
            };
            cTicket.Status.push(1);
            $("#ulTickets li").eq(index).find('.chk-ticket-status:checked').each(function (s) {
                if ($(this).hasClass('assigned')) {
                    cTicket.Status.push(2);
                } else if ($(this).hasClass('in-process')) {
                    cTicket.Status.push(3);
                } else if ($(this).hasClass('stand-by')) {
                    cTicket.Status.push(4);
                } else if ($(this).hasClass('ready-to-publish')) {
                    cTicket.Status.push(5);
                } else if ($(this).hasClass('completed')) {
                    cTicket.Status.push(6);
                }
            });
            Tickets.push(cTicket);
        }
        if (ePlatHubConnected) {
            ePlatHub.server.updateTickets($.toJSON(Tickets), mode);
        }
    }

    var ticketUpdated = function (updated) {
        if (updated) {
            $('#imgUnasignedTickets, #imgAsignedTickets').hide();        
        }
    }

    return {
        init: init,
        loaded: loaded,
        saveTicketSuccess: saveTicketSuccess,
        List: List,
        seachBegin: seachBegin,
        renderTickets: renderTickets,
        ticketUpdated: ticketUpdated
    }
}();

$(function () {
    Tickets.init();
});