var invitation = new Vue({
    mixins: [ePlatUtils],
    el: "#app",
    data:
    {
        Shared: ePlatStore,
        invitations: [],//
        DataTable: {
            fields: [
                { key: 'presentationDateTimeFormat', label: 'Presentation Date', sortable: true },
                { key: 'pickUpTimeFormat', label: 'Pick Up Time' },
                { key: 'presentationPlace', label: 'Presentation Place' },
                { key: 'guest', label: 'Guest', sorteable: true, sortable: true },
                { key: 'spouse', label: 'Spouse' },
                // { key: 'depositCurrencyCode', label:'Currency' },
                // { key: 'depositAmount', label:'Deposit Amount', sortable:'true' },                   
                { key: 'invitationNumber', label: 'Invitation Number', sortable: true },
                { key: 'confirmed', label: 'Confirmed' },
                { key: 'saved', label: 'Saved' },
                { key: 'modified', label: 'Modified' },
                { key: 'premanifest', label: '' }
            ],
            sortBy: 'saved',
            sortDesc: true,
            perPage: 10,
            currentPage: 1,
            striped: true,
            bordered: true,
            hover: false
        },
        invitationModel: {
            invitationID: null,
            presentationDateTime: '',
            presentationDateTimeFormat: '',
            pickUpTime: '',
            pickUpTimeFormat: '',
            firstName: '',
            secondName: '',
            lastName: '',
            spouseFirstName: '',
            spouseSecondName: '',
            spouseLastName: '',
            guestEmail: '',
            guestPhone: '',
            invitationNumber: '',
            pickUpNotes: '',
            gift: '',
            confirmed: false,
            depositAmount: null,
            depositCurrencyCode: '',
            presentationPlace: '',
            presentationPlaceID: null,
            //spi
            spihotelPickUPID: null,
            spihotelPickUp: '',
            spiCountryID: null,
            spiCountry: '',
            spiLanguageID: null,
            spiLanguage: '',
            opcID: null,
            opc: '',
            spiGroupID: null,
            spiGroup: '',
            spiTeamID: null,
            spiTeam: '',
            spiHotelID: null,
            spiHotel: '',
            spiLocationID: null,
            spiLocation: '',
            spiSalesRoomID: null,
            guest: '',
            spouse: '',
            state: '',
            jaladorOpcID: null,
            jaladorOpc: '',
            programID: null,
            program: '',
            premanifest: false,
            sendEmail: false,
            pickUpTypeID: null,
            pickUpName: '',
            pickUpTypeID: null,
            pickUpType: '',
            depositPickUpNotesID: null,
            depositPickUpNotes: '',
            shiftID: null,
            shift: '',
            manifestFolio: null,
            invitationDeposits: [],
            adults: 1,
            childs: 0,
            spiCategoryID:null,
            category:''
        },//
        showInvitationInfo: false,
        showInvitation: true,
        guestsToMatch: [],
        matchTable:
        {
            fields: [
                { key: 'customerName1', label: 'Guest', sortable: true },
                { key: 'customerName2', label: 'Spouse', sortable: true },
                { key: 'st', label: 'State' },
                { key: 'country', label: 'Country' },
                { key: 'tourDateFormat', label: ' Tour Date', sortable: true },
                { key: 'volumen', label: 'Volumen' },
                { key: 'match', label: 'Match' },
                { key: 'matchUser', label: ' Match Guest' }
            ],
            sortBy: 'presentationDateTime',
            sortDesc: true,
            perPage: 10,
            currentPage: 1,
            striped: true,
            bordered: true,
            hover: false
        },
        modalInfo: {
            invitationIDModal: null,
            custumerIDModal: null,
            firstNameModal: '',
            lastNameModal: '',
            spouseFirstNameModal: '',
            spouseLastNameModal: '',
            guestEmailModal: '',
            guestPhoneModal: '',
            guestModal: '',
            spouseGuestModal: '',
            guestStateModal:'',
            model: ''
        },
        emailObject: {
            presentationPlaceID: null,
            presentationDateTimeFormat: '',
            guest: '',
            spiHotelID: null,
            spiLanguageID: '',
            pickUpNotes: '',
            pickUpTime: '',
            depositAmount: '',
            gift: '',
            guestEmail: '',
            programID: null,
            confirmed: false,
            pickUpTypeID: null,
            invitationNumber: null,
            invitationID: '',
            invitationDeposits: []
        },
        invitationDeposits: {
            invitationDepositID: null,
            invitationID: null,
            amount: null,
            currencyID: null,
            currency: '',
            paymentTypeID: null,
            paymentType: '',
            ccReferenceNumber: '',
            received: false,
            dateSaved: '',
            savedByUserID: null,
            saveUser: '',
            dateLastModification: '',
            modifiedByUserID: null,
            modifiedByUser: '',
            deleted: false,
            deletedByUserID: null,
            deletedDateTime: '',
            editDeposit: false
        },
        update: false
    },
    computed:
    {
        FromDateYYYYMMDD: {
            get: function () {
                //uso esta propiedad computada para mostrar en el formulario el valor en formato yyyy-MM-dd
                return moment(this.invitationModel.presentationDateTimeFormat).format("YYYY-MM-DD");
            },
            set: function (newValue) {
                this.invitationModel.presentationDateTimeFormat = newValue;
            }
        },
    },
    methods:
    {
        //modal
        info: function (row, index, button) {
            //disparar datos de busqueda para guest 
            // asignar 
            this.modalInfo.invitationID = row.invitationID;
            this.modalInfo.firstNameModal = row.firstName;
            this.modalInfo.lastNameModal = row.lastName;
            this.modalInfo.spouseFirstNameModal = row.spouseFirstName;
            this.modalInfo.spouseLastNameModal = row.spouseLastName;
            this.modalInfo.guestEmailModal = row.guestEmail;
            this.modalInfo.guestPhone = row.guestPhone;
            this.modalInfo.guestStateModal = row.state;
            this.modalInfo.model = row;
            
           /* $('#invitationIDModal').val(row.invitationID);
            $('#firstNameModal').val(row.firstName);
            $('#lastNameModal').val(row.lastName);
            $('#spouseNameModal').val(row.spouseFirstName);
            $('#spouseLastNameModal').val(row.spouseLastName);
            $('#guestEmailModal').val(row.guestEmail);
            $('#guestPhoneModal').val(row.guestPhone);
            $('#stateModal').val(row.state);*/
            $('#btnSearchGuest').click();
            row.spiCountry = $('#spiCountryID option[value="' + row.spiCountryID + '"]').text();
            row.spiLanguage = $('#spiLanguageID option[value="' + row.spiLanguageID + '"]').text();
            this.invitationModel = row;
        },
        resetModal: function () {
            this.modalInfo.invitationIDModal = null;
            this.modalInfo.custumerIDModal = null;
            this.modalInfo.firstNameModal = '';
            this.modalInfo.lastNameModal = '';
            this.modalInfo.spouseFirstNameModal = '';
            this.modalInfo.spouseLastNameModal = '';
            this.modalInfo.guest
            this.modalInfo.model = '';
            this.guestsToMatch = [];
        },
        setSearchGuest: function (data) {
            //mostrar datos de busqueda
            this.guestsToMatch = data;
        },
        /*getRowValues:function(row, index, button)
        {
            var list = this.guestsToMatch; 
            $('#custumerIDModal').val(null);
            $('#custumerIDModal').val(row.guestToMatchID);
            for (value = 0; value < list.length; value++) {
                var item = list[value];
             //   if (item.guestToMatchID != row.guestToMatchID)
                //if (value != index)
                    $('#tblMatchTable tr[aria-rowindex="' + (value + 1) + '"]').find(':checkbox').prop('checked', false);
            }
            if ($('#checkMatch:checked').length == 0)
                $('#btnPreManifest').attr('disabled', true);
            else
                $('#btnPreManifest').attr('disabled', false);                    
        },*/
        getRowValues: function (row, index, button) {
            var list = this.guestsToMatch;
            $('#custumerIDModal').val(null);
            $('#custumerIDModal').val(row.guestToMatchID);
            for (value = 0; value < list.length; value++) {
                if (value != index)
                    $('#tblMatchTable tr[aria-rowindex="' + (value + 1) + '"]').find(':checkbox').prop('checked', false);
            }
            if ($('#checkMatch:checked').length == 0)
                $('#btnPreManifest').attr('disabled', true);
            else
                $('#btnPreManifest').attr('disabled', false);
        },
        validate: function (row, index) {
            $('#tblMatchTable').find(':checkbox').prop('checked', false);
            $('#btnPreManifest').attr('disabled', true);
            $('.close').click();
            $('#previewInformation').show();
        },
        closePreview: function () {
            $('#custumerIDModal').val(null);
            $('#previewInformation').hide();
        },
        newGuest: function () {
            let self = this;
            model = self.invitationModel;
            self.loadDependentLists();
            self.getDepositTotalAmount(model.invitationID);
            $('.oneValue').each(function (e) {
                listID = $(this).attr('id');
                val = listID.slice(0, -2);
                value = model[listID];
                model[val] = model[listID] == null ? "" : $('#' + listID + ' option[value ="' + value + '"]').text();
            });
            $('.close').click();
            $('#custumerIDModal').val(null);
            $('#previewInformation').show();
            Vue.nextTick(function () {
                if ($('#verProgram').val() == 'Airport')
                    $('.jaladorSH').show();
                else
                    $('.jaladorSH').hide();
            });
        },
        savePreManifest: function () {
            var self = this;
            var customerID = $('#custumerIDModal').val();
            var invitationID = $('#invitationID').val();

            //var invitationID = $('#invitationIDModal').val();
            var salesRoomID = self.invitationModel.spiSalesRoomID;
            $.confirm({
                title: 'Send Premanifest to SPI',
                content: 'The Information is Correct?',
                animation: 'zoom',
                closeAnimation: 'scale',
                type: 'orange',
                typeAnimated: true,
                buttons: {
                    cancel: function () {
                    },
                    confirm: function () {
                        //en caso de confirmar, enviar al servidor
                        $.ajax({
                            url: 'Invitations2/manifestInvitation/',
                            cache: false,
                            type: 'Save',
                            data: { spiInvitationID: invitationID, salesRoomID: salesRoomID, customerID: customerID },
                            success: function (data) {
                                //respuesta
                                if (data.ResponseType == 1) {
                                    //mensaje de confirmación
                                    $.alert({
                                        title: 'Invitation',
                                        content: 'Premanifested Success !',
                                        animation: 'zoom',
                                        closeAnimation: 'scale',
                                        autoClose: 'ok|5000',
                                        type: 'green'
                                    });

                                    self.closePreview();
                                    self.guestsToMatch = [];
                                    self.resetModal();
                                    self.newInvitation();
                                } else {
                                    $.alert({
                                        title: 'Error',
                                        content: data.ResponseMessage,
                                        animation: 'zoom',
                                        closeAnimation: 'scale',
                                        autoClose: 'ok|5000',
                                        type: 'red'
                                    });
                                }
                            }
                        });
                    }

                }
            });
        },
        setSearchInvitations: function (data) {
            this.invitations = data;
        },
        openInvitation: function (row, i) {
            let self = this;
            self.newInvitation();
            self.invitationModel = row;
            self.invitationModel.pickUpTime = (row.pickUpTime.replace('a. m.', 'AM').replace('p. m.', 'PM'));
            self.invitationModel.pickUpTimeFormat = (row.pickUpTimeFormat.replace('a. m.', 'AM').replace('p. m.', 'PM'));
            self.loadDependentLists();
            self.showInvitationInfo = true;
            self.sendEmail = true;
            self.invitationModel.invitationDeposits = row.invitationDeposits;

            //self.invitationModel.deposits.dateSaved = moment(row.invitationDeposits.dateSaved.slice(1, -1)).format('YYYY-MM-DD hh:mm A');
            //self.invitationModel.deposits.dateLastModification = moment(self.invitationModel.deposits.dateLastModification.slice(1, -1)).format('YYYY-MM-DD hh:mm A');
            //----------------------------
            if ($('#Edit').val() == 'False') {
                $('.validate').hide();
            }
            self.UI().scrollTo('divInvitationInfo');
            $('#tblInvitationsInfo tr').removeClass('selected');
            $('#tblInvitationsInfo tr[aria-rowindex="' + (i + 1) + '"]').addClass('selected');
            //-----            
            this.UI().scrollTo('divInvitationInfo');
            //ADD BUTTON UPDATE
            $('#invitationUpdate').show();
           // $('#btnSaveInvitation').hide();
            $('#sendEmailButton').show();
            $('#openEmailPreview').text("Update and Send");
            //-----
            self.update = true;
            //-----
            Vue.nextTick(function () {

                if ($('#programID').val() == 16)
                    $('.jaladorSH').show();
                else
                    $('.jaladorSH').hide();

                self.invitationModel.spiTeamID = row.spiTeamID;
                self.invitationModel.opcID = row.opcID;
                self.invitationModel.jaladorOpcID = row.jaladorOpcID;
               // self.invitationModel.spiCategoryID = row.spiCategoryID;
            });
        },
        saveInvitation: function (data) {
            let self = this;
            if (data.ResponseType == 1) {
                if (data.ResponseMessage == "Invitation Updated Success") {
                    if (self.invitationModel.invitationDeposits.length != 0) {
                        self.saveDeposits(data.ObjectID,false);
                    }
                }
                else {
                    if (self.invitationModel.invitationDeposits.length == 0) {
                        self.sendEmailInformation(data.ObjectID);
                    } else {
                        self.saveDeposits(data.ObjectID,true);
                    }
                }
                $.alert({
                    title: "Invitation Saved Success",
                    content: '',
                    animation: 'zoom',
                    closeAnimation: 'scale',
                    autoClose: 'ok|5000',
                    type: 'green'
                });
              //  self.invitations.push(self.invitationModel);
              //  $('btnSearchInvitations').click();
                $('#modalInfo').modal('hide');
                this.resetModal();
                this.hideModal();
                this.showInvitationInfo = false;
            }
            else {
                $.alert({
                    title: 'Error!!',
                    content: 'There was an error traying to save invitation',
                    animation: 'zoom',
                    closeAnimation: 'scale',
                    autoClose: 'ok|5000',
                    type: 'red'
                });
            }
        },
        newInvitation: function () {
            this.invitationModel = {
                invitationID: null,
                presentationDateTimeFormat: moment(moment(), 'YYYY-MM-DD').add(1, 'days').format('YYYY-MM-DD'),
                pickUpTime: '',
                firstName: '',
                secondName: '',
                lastName: '',
                spouseFirstName: '',
                spouseSecondName: '',
                spouseLastName: '',
                guestEmail: '',
                invitationNumber: '',
                pickUpNotes: '',
                gift: '',
                confirmed: false,
                depostiAmount: null,
                depositCurrencyCode: null,
                //spi
                spiCountryID: null,
                spiCountry: '',
                spiLanguage: null,
                opcID: null,
                opc: '',
                spiGroupID: null,
                spiGroup: '',
                spiTeamID: null,
                spiTeam: '',
                spiHotelID: null,
                spiHotel: '',
                spiLocationID: null,
                spiLocation: '',
                //spiSalesRoomID: null,
                jaladorOpcID: null,
                jaladorOPc: '',
                programID: null,
                program: '',
                presentationPlaceID: null,
                presentationPlace: '',
                sendEmail: false,
                pickUpTypeID: null,
                pickUpType: '',
                depositPickUpID: null,
                depositPickUp: '',
                shiftID: null,
                shift: '',
                manifestFolio: null,
                invitationDeposits: [],

            };
            $('#depositCurrencyCode option[value="' + 0 + '"]').attr('selected', true);
            $('#spiLanguageID option[value="' + 0 + '"]').attr('selected', true);
            $('#spiCountryID option[value="' + 0 + '"]').attr('selected', true);
            $('#opcID option[value="' + 0 + '"]').attr('selected', true);
            $('#spiGroupID option[value="' + 0 + '"]').attr('selected', true);
            $('#spiTeamID option[value="' + 0 + '"]').attr('selected', true);
            $('#spiLocationID option[value="' + 0 + '"]').attr('selected', true);
            $('#spiHotelID option[value="' + 0 + '"]').attr('selected', true);
            $('#spiSalesRoomID option[value="' + 0 + '"]').attr('selected', true);
            $('#presentationPlaceID option[value="' + 0 + '"]').attr('selected', true);
            $('.validate').show();
            this.showInvitationInfo = true;
            this.UI().scrollTo('divInvitationInfo');
            //one value in a list                
            $('.oneValue').each(function (e) {
                list = $(this);
                if (list.children('option').length == 2)
                    self.invitationModel[list.attr('id')] = $(":last-child", list).val();
                else
                    self.invitationModel[list.attr('id')] = $(":first-child", list).val();
            });
            $('#invitationUpdate').hide();
            $('#sendEmailButton').hide();
           // $('#btnSaveInvitation').show();
            $('#openEmailPreview').text("Preview");
            //--
            self.update = false;
            //--
        },
        loadDependentLists: function () {
            $.each(self.Shared.State.DependentFields.Fields, function (i, v) {
                if (v.ParentField.indexOf('search') == -1) {
                    list = '';//opciones de la lista
                    for (x = 1; x < v.Values.length; x++) {
                        if (self.invitationModel[v.ParentField] == v.Values[x].ParentValue)
                            list += '<option value="' + v.Values[x].Value + '">' + v.Values[x].Text + '</option>\n';
                    }
                    $('#' + v.Field).html(list);
                }
            });
        },
        showEmail: function (sendEmail) {
            let self = this;
            var emailModel = self.emailObject;
            $.each(self.invitationModel.invitationDeposits, function (i, v) {
                let object = {
                    amount: v.amount,
                    currency: v.currency,
                    paymentType: v.paymentType,
                    ccReferenceNumber: v.ccReferenceNumber,
                    received: v.received,
                    deleted: v.deleted
                };
                emailModel.invitationDeposits.push(object);
            });
            var url = window.location.origin.indexOf("http://localhost:45000") > -1 ? "http://localhost:45000/crm/invitations2/RenderEmailPreview" : "https://eplat.villagroup.com/crm/invitations2/RenderEmailPreview";            //var url = "https://eplat.villagroup.com/crm/invitations2/RenderEmailPreview";
            //var url = "http://localhost:45000/crm/invitations2/RenderEmailPreview";
            emailModel.presentationPlaceID = self.invitationModel.presentationPlaceID;
            emailModel.presentationDateTimeFormat = self.invitationModel.presentationDateTimeFormat;
            emailModel.guest = self.invitationModel.firstName + " " + self.invitationModel.lastName;
            emailModel.spiHotelID = self.invitationModel.spiHotelID;
            emailModel.pickUpNotes = self.invitationModel.pickUpNotes;
            //  emailModel.pickUpTime = self.invitationModel.pickUpTime == "" ? self.invitationModel.pickUpTimeFormat : self.invitationModel.pickUpTime;
            emailModel.pickUpTime = self.invitationModel.pickUpTimeFormat == "" ? moment(self.invitationModel.pickUpTime).format('HH:mm') : moment(emailModel.presentationDateTimeFormat + " " + self.invitationModel.pickUpTimeFormat).format('HH:mm');
            emailModel.depositAmount = 0;
            emailModel.gift = self.invitationModel.gift;
            emailModel.spiLanguageID = self.invitationModel.spiLanguageID;
            emailModel.guestEmail = self.invitationModel.guestEmail;
            emailModel.confirmed = self.invitationModel.depositPickUpID == 3 ? true : false;
            emailModel.programID = self.invitationModel.programID;
            emailModel.invitationID = self.invitationModel.invitationID != null ? self.invitationModel.invitationID : "00000000-0000-0000-0000-000000000000";
            emailModel.pickUpTypeID = self.invitationModel.pickUpTypeID;
            emailModel.invitationNumber = self.invitationModel.invitationNumber;
            var model = (JSON.stringify(emailModel)).replace("@\'", '');
            this.getEmailPreview(sendEmail, model);
           // if (self.invitationModel.programID != null && self.invitationModel.presentationPlaceID) {
              //  $('#emailPreviewFrame').attr('src', url + '/?sendEmail=' + sendEmail + '&' + 'model=' + model);
                // $('#emailPreviewFrame').attr('src', url + '/?sendEmail=' + sendEmail + '&' + 'model=' + (JSON.stringify(emailModel)).replace("@\'", ''));
            //}
            //this.clearEmailObject();
        },
        hideModal: function () {
            this.$refs.previewEmail.hide();
            var frame = document.getElementById('emailPreviewFrame');
            var doc = frame.contentDocument || frame.contentWindow.document;
            doc.documentElement.innerHTML = "";
            var email = this.emailObject;
            email.presentationPlaceID = null;
            email.presentationDateTimeFormat = "";
            email.guest = "";
            email.spiHotelID = null;
            email.spiLanguageID = '';
            email.pickUpNotes = '';
            email.pickUpTime = '';
            email.depositAmount = '';
            email.gift = '';
            email.guestEmail = '';
            email.programID = null,
            email.confirmed = false,
            email.invitationID = '';
        },
        sendEmail: function () {
            showEmail(true)
        },
        showSaveButton() {

        },
        addPayment: function () {
            let self = this;
            var deposit = this.invitationDeposits;
            deposit.dateSaved = moment().format('YYYY-MM-DD hh:mm A');
            deposit.invitationID = $('#invitationID').val() == null ? "00000000-0000-0000-0000-000000000000" : $('#invitationID').val();
            $('#depositPaymentType option').each(function (i, o) {
                if (deposit.currencyID == 1) {
                    $('#depositccReference').attr("disabled", true);
                } else {
                    $('#depositccReference').attr("disabled", false);
                }
            });
            deposit.currency = $("#depositCurrency option:selected").text();
            deposit.paymentType = $('#depositPaymentType option:selected').text();

            if ((deposit.amount == null) || (deposit.paymentTypeID == null) || deposit.currencyID == null) {
                this.alertMessage('Please add a valid deposit information');
            }
            else {
                if (deposit.paymentTypeID == 2 && deposit.ccReferenceNumber == '') {
                    this.alertMessage('Please add ccReference');
                }
                else {
                    self.invitationModel.invitationDeposits.push(deposit);
                    self.clearInvitationDeposits();
                }
            }
        },
        clearInvitationDeposits: function () {
            this.invitationDeposits = {
                invitationDepositID: null,
                amount: null,
                currencyID: null,
                currency: '',
                paymentTypeID: null,
                paymentType: '',
                ccReferenceNumber: '',
                received: false,
                dateSaved: '',
                savedByUserID: null,
                saveUser: '',
                dateLastModification: '',
                modifiedByUserID: '',
                modifiedByUser: '',
                deleted: false,
                deletedByUserID: '',
                deleteDateTime: ''
            }
        },
        deleteDeposit: function (i) {
            this.invitationModel.invitationDeposits[i].deleted = true;
            this.invitationModel.invitationDeposits[i].deletedDateTime = moment().format('YYYY-MM-DD hh:mm A');
        },
        saveDeposits: function (invitationID,sendemail) {
            if (invitationID != 0) {
                let self = this;
                var deposits = this.invitationModel.invitationDeposits;
                if (deposits.length > 0) {
                    $.each(deposits, function (i, deposit) {
                        deposit.invitationID = invitationID;
                        deposit.dateSaved = deposit.dateSaved.indexOf('Date') > -1 ? moment(deposit.dateSaved.slice(1, -1)).format('YYYY-MM-DD hh:mm A') : deposit.dateSaved;
                        deposit.dateLastModification = deposit.dateLastModification == null ? null : deposit.dateLastModification.indexOf('Date') > -1 ? moment(deposit.dateLastModification.slice(1, -1)).format('YYYY-MM-DD hh:mm A') : deposit.dateLastModification;
                        deposit.deletedDateTime = deposit.deletedDateTime == null ? null : deposit.deletedDateTime.indexOf('Date') > -1 ? moment(deposit.deletedDateTime.slice(1, -1)).format('YYYY-MM-DD hh:mm A') : deposit.deletedDateTime;
                        deposit.deleted = deposit.deleted == null ? false : deposit.deleted;
                    });
                    $.ajax({
                        url: '/crm/Invitations2/SaveInvitationDeposits',
                        cache: false,
                        type: 'POST',
                        data: { model: JSON.stringify(deposits) },
                        success: function (data) {
                            if (data.ResponseType == 1) {
                                if (sendemail)
                                    self.sendEmailInformation(invitationID);
                                self.deposit = [];
                                self.clearInvitationDeposits();
                            } else {
                                //notificar el error al guardar
                                $.alert({
                                    title: 'Error Saving',
                                    content: 'There was an error trying to save the deposits',
                                    animation: 'zoom',
                                    closeAnimation: 'scale',
                                    autoClose: 'ok|3000',
                                    type: 'red'
                                });
                            }
                        }
                    });
                }
            }
        },
        editPayment: function (n) {
            let self = this;
            this.invitationDeposits = self.invitationModel.invitationDeposits[n];
            this.invitationModel.invitationDeposits[n].editDeposit = true;
            if (self.invitationModel.invitationDeposits[n].paymentTypeID == 2)
                $("depositccReference").attr("disabled", true);
        },
        saveEditPayment: function (n) {
            let self = this;
            var deposit = self.invitationModel.invitationDeposits[n];
            if ((deposit.amount == null) || (deposit.paymentTypeID == null) || deposit.currencyID == null) {
                this.alertMessage('Please add a valid deposit information');
            }
            else {
                if (deposit.paymentTypeID == 2 && deposit.ccReferenceNumber == '') {
                    this.alertMessage('Please add ccReference');
                }
                else {
                    deposit = this.invitationDeposits;
                    deposit.dateLastModification = moment().format('YYYY-MM-DD hh:mm A');
                    deposit.paymentType = $("#depositPaymentType option[value='" + deposit.paymentTypeID + "']").text();
                    deposit.currency = $("#depositCurrency option[value='" + deposit.currencyID + "']").text();
                    this.clearInvitationDeposits();
                    this.invitationModel.invitationDeposits[n].editDeposit = false;
                }
            }
        },
        alertMessage: function (text) {
            $.alert({
                title: 'Required Fields',
                content: text,
                animation: 'zoom',
                closeAnimation: 'scale',
                autoClose: 'ok|3000',
                type: 'red'
            });
        },
        renderEmailinformation: function (sendEmail) {
            let self = this;
            var emailModel = self.emailObject;
            var invitationModel = self.invitationModel;
            emailModel.presentationPlaceID = self.invitationModel.presentationPlaceID;
            emailModel.presentationDateTimeFormat = self.invitationModel.presentationDateTimeFormat;
            emailModel.guest = self.invitationModel.firstName + " " + self.invitationModel.lastName;
            emailModel.spiHotelID = self.invitationModel.spiHotelID;
            emailModel.pickUpNotes = self.invitationModel.pickUpNotes;
            emailModel.pickUpTime = self.invitationModel.pickUpTimeFormat == "" ? moment(self.invitationModel.pickUpTime).format('HH:mm') : moment(emailModel.presentationDateTimeFormat + " " + self.invitationModel.pickUpTimeFormat).format('HH:mm');
            //emailModel.depositAmount = self.invitationModel.depositAmount;
            emailModel.gift = self.invitationModel.gift;
            emailModel.spiLanguageID = self.invitationModel.spiLanguageID;
            emailModel.guestEmail = self.invitationModel.guestEmail;
            emailModel.confirmed = self.invitationModel.depositPickUpID == 3 ? true : false;
            emailModel.programID = self.invitationModel.programID;
            emailModel.invitationID = self.invitationModel.invitationID != null ? self.invitationModel.invitationID : "00000000-0000-0000-0000-000000000000";
            emailModel.pickUpTypeID = self.invitationModel.pickUpTypeID;
            emailModel.invitationNumber = self.invitationModel.invitationNumber;
            emailModel.invitationDeposits = [];

            $.each(self.invitationModel.invitationDeposits, function (i, v) {
                let object = {
                    amount: v.amount,
                    currency: v.currency,
                    paymentType: v.paymentType,
                    ccReferenceNumber: v.ccReferenceNumber,
                    received: v.received
                };
                emailModel.invitationDeposits.push(object);
            });
            console.log(emailModel);
            console.log(this.invitationModel);
            $.ajax({
                url: 'Invitations2/RenderEmailPreviewText/',
                cache: false,
                type: 'Save',
                data: {
                    sendEmail: sendEmail,
                    model: invitationModel
                },
                success: function (data) {
                    console.log(data);
                    $('#emailPreviewFrame').html(data);
                }
            });
        },
        clearEmailObject: function () {
            this.emailObject = {
                presentationPlaceID: null,
                presentationDateTimeFormat: '',
                guest: '',
                spiHotelID: null,
                spiLanguageID: '',
                pickUpNotes: '',
                pickUpTime: '',
                depositAmount: '',
                gift: '',
                guestEmail: '',
                programID: null,
                confirmed: false,
                pickUpTypeID: null,
                invitationNumber: null,
                invitationID: '',
                invitationDeposits: []
            }
        },
        sendEmailInformation: function(invitationID) {
            $.ajax({
                url: '/crm/Invitations2/SendEmail',
                cache: false,
                type: 'POST',
                data: { invitationID: invitationID },
                success: function (data) {
                    if (data.ResponseType == 1) {
                        $.alert({
                            title: 'Email',
                            content: 'Email was send success',
                            animation: 'zoom',
                            closeAnimation: 'scale',
                            autoClose: 'ok|3000',
                            type: 'green'
                        });
                    } else {
                        //notificar el error al guardar
                        $.alert({
                            title: 'Error',
                            content: 'There was an error trying to send email',
                            animation: 'zoom',
                            closeAnimation: 'scale',
                            autoClose: 'ok|3000',
                            type: 'red'
                        });
                    }
                }
            });
        },
        getDepositTotalAmount(invitationID) {
            let self = this
            $.ajax({
                url: '/crm/Invitations2/GetDepositsTotalAmount',
                cache: false,
                type: 'POST',
                data: { invitationID: invitationID },
                success: function (data) {
                    self.invitationModel.depositAmount = data;
                }
            });
        },
        getEmailPreview(sendEmail, model) {
            let self = this;
            $.ajax({
                url: "/Invitations2/RenderEmailPreview",
                chae:false,
                type: "POST",
                //dataType: "json",
                data: { sendEmail: sendEmail, model: model },
                //contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    self.insertEmail(data);
                    self.emailObject.invitationDeposits = [];
                }
            });
        },
        insertEmail(data) {
            var iframe = document.getElementById('emailPreviewFrame');
            iframe = iframe.contentWindow || (iframe.contentDocument.document || iframe.contentDocument);

            iframe.document.open();
            iframe.document.write(data);
            iframe.document.close();
        },
        submitInfo(sendEmail) {
            let self = this;
            if (sendEmail)//save and send
            {
                $('#frmInvitationInfo').submit();
                self.showEmail(sendEmail);
                self.hideModal();
            } else { //only save
                $('#frmInvitationInfo').submit();
            }
        }

        
    },
    mounted: function ()
    {
        let self = this;
        this.Session().getSessionDetails();
        this.UI().loadDependentFields('/crm/Invitations2/GetDependentFieldsFromInvitations', true);
        this.UI().showSearchCard();
        
        //iniciar datepickers
        $('.datetimepicker-input').datetimepicker({
            format: 'YYYY-MM-DD'
        }).on('blur', function () {
            self.invitationModel.presentationDateTime = $(this).val();
            self.invitationModel.presentationDateTimeFormat = $(this).val();
        });
        $('#pickUpTime').datetimepicker({
            format: 'LT',
            autoclose: true
        });
        $('#pickUpTime').on('blur', function (e) {
            self.invitationModel.pickUpTimeFormat = this.value;

           var visible = $('.bootstrap-datetimepicker-widget').is(':visible');
           if (visible)
              $('.bootstrap-datetimepicker-widget').remove();
        });
        $('[multiple="multiple"]').multiselect({
            buttonWidth: '100%',
            includeSelectAllOption: true,
            enableFiltering: true,
            maxHeight:400
        });
        //phoneValidation
        $('#guestPhone').on('blur', function ()
        {
            if (this.value.length < 10 && this.value.length > 0)
            {
                $.alert({
                    title: 'Error!!',
                    content: 'Please enter 10 digits',
                    animation: 'zoom',
                    closeAnimation: 'scale',
                    autoClose: 'ok|5000',
                    type: 'red'
                });
            }
        });
        //onlyWriteNumbers in a texbox
        $('.onlyNumbers').on('keydown', function (e)
        {
            if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
                // Allow: Ctrl+A, Command+A
               (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
                // Allow: home, end, left, right, down, up
               (e.keyCode >= 35 && e.keyCode <= 40)) {
                // let it happen, don't do anything
                return;
            }
            // Ensure that it is a number and stop the keypress
            if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                e.preventDefault();
            }
        });
        $('.onlyLetters').on('keydown', function (e) {
            var keyCode = (e.keyCode ? e.keyCode : e.which);
            if (keyCode > 47 && keyCode < 58 || keyCode > 95 && keyCode < 107 )
                e.preventDefault();
        });
        $('#programID').on('change', function () {
            val = $(this).val();
            if (val == 16)
                $('.jaladorSH').show();
            else
                $('.jaladorSH').hide();
        });
        $('#sendEmailButton').on('click', function () {
            $('#sendEmail').val("true");
        });
        $('#sendEmailButton').on('click', function () {
            $('#sendEmail').val("false");
        });
        $('#invitationUpdate').on('click', function () {
            $('#sendEmail').val("false");
        });
        $('#depositPaymentType').on('change', function () {
            $('#depositPaymentType option').each(function (i, o) {
                if (self.invitationDeposits.paymentTypeID == 1) {
                    $('#depositccReference').attr("disabled", true);
                } else {
                    $('#depositccReference').attr("disabled", false);
                }
            });
        });
        $('#btnSaveInvitation').on('click', function () {
            $('#invitationUpdate').click();
        });
    }
    /* $.ajax({
            url: "/Home/jQueryAddComment",
            type: "POST",
            dataType: "json",
            data: json,
            contentType: 'application/json; charset=utf-8',
            success: function(data){
                //var message = data.Message;
                alert(data);
                $('.CommentSection').html(data);
            }*/
});
