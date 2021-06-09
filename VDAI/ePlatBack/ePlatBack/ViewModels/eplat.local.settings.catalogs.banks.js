var Banks = new Vue({
    mixins: [ePlatUtils],//obtengo la herencia de los métodos compartidos
    el: '#app',//asigno la instancia al div principal
    data: {
        Shared: ePlatStore,//Obtengo la herencia de los campos compartidos en todos los módulos
        Banks: [],//lista para almacenar los bancos de la UI
        DataTable: {//configuración de la DataTable
            fields: [
                { key: 'BankID', label: 'ID' },
                { key: 'BankName', sortable: true },
                { key: 'CveSat', sortable: true },
                { key: 'FromDate', sortable: true },
                { key: 'ToDate', sortable: true },
                { key: 'Terminal', sortable: true },
                { key: 'actions', label: '' }
            ],
            sortBy: 'BankName',
            sortDesc: false,
            perPage: 10,
            currentPage: 1,
            striped: true,
            bordered: true,
            hover: true
        },
        Bank: {//objeto para guardar los datos del formulario de Bank Info
            BankID: null,
            BankName: '',
            CveSat: '',
            FromDate: '',
            ToDate: '',
            TerminalID: ''
        },
        showBankInfo: false //propiedad para mostrar u ocultar el formulario de Bank Info
        //DatePickerConfig: {

        //},
        //searchFromDate: null,
        //searchToDate: null
    },
    computed: {
        FromDateYYYYMMDD:  {
            get: function () {
                //uso esta propiedad computada para mostrar en el formulario el valor en formato yyyy-MM-dd
                return moment(this.Bank.FromDate).format("YYYY-MM-DD");
            },
            set: function (newValue) {
                this.Bank.FromDate = newValue;
            }
        },
        ToDateYYYYMMDD: {
            get: function () {
                //uso esta propiedad computada para mostrar en el formulario el valor en formato yyyy-MM-dd
                return moment(this.Bank.ToDate).format("YYYY-MM-DD");
            },
            set: function (newValue) {
                this.Bank.ToDate = newValue;
            }
        }
    },
    methods: {
        setSearchResults: function (data) {
            //asignar resultados de la búsqueda a la lista de bancos
            this.Banks = data;
        },
        openBank: function (row, i) {
            //asignar objeto fila al objeto banco
            this.Bank = row;
            //mostrar el formulario
            this.showBankInfo = true;
            //seleccionar fila
            $('#tblBanks tr').removeClass('selected');
            $('#tblBanks tr[aria-rowindex="' + (i + 1) + '"]').addClass('selected');
            //mover el scroll al formulario
            this.UI().scrollTo('divBankInfo');
        },
        newBank: function () {
            //limpiar el objeto banco
            this.Bank = {
                BankID: null,
                BankName: '',
                CveSat: '',
                FromDate: '',
                ToDate: '',
                TerminalID: ''
            };
            //mostrar el formulario
            this.showBankInfo = true;
            //mover el scroll
            this.UI().scrollTo('divBankInfo');
        },
        deleteBank: function (bankObj) {
            //agregar confirm
            $.confirm({
                title: 'Delete Bank',
                content: 'Are you sure that you want to delete ' + bankObj.BankName + '?',
                animation: 'zoom',
                closeAnimation: 'scale',
                type: 'orange',
                typeAnimated: true,
                buttons: {
                    confirm: function () {
                        //en caso de confirmar, enviar al servidor
                        $.ajax({
                            url: '/Catalogs/DeleteBank/' + bankObj.BankID,
                            cache: false,
                            type: 'DELETE',
                            success: function (data) {
                                //respuesta
                                if(data.ResponseType == 1) {
                                    //si la respuesta es ok, eliminar de la colección
                                    let index = -1;
                                    $.each(self.Banks, function (b, bank) {
                                        if (bank.BankID == bankObj.BankID) {
                                            index = b;
                                        }
                                    });
                                    self.Banks.splice(index, 1);

                                    //mensaje de confirmación
                                    $.alert({
                                        title: 'Bank Succesfully Deleted!',
                                        content: bankObj.BankName + ' was deleted from your Banks Catalog.',
                                        animation: 'zoom',
                                        closeAnimation: 'scale',
                                        autoClose: 'ok|5000',
                                        type: 'green'
                                    });
                                } else {
                                    //mensaje de error al eliminar
                                    $.alert({
                                        title: 'Error',
                                        content: data.ResponseMessage,
                                        animation: 'zoom',
                                        closeAnimation: 'scale',
                                        autoClose: 'ok|5000',
                                        type: 'green'
                                    });
                                }                                
                            }
                        });
                    },
                    cancel: function () {
                     
                    }
                }
            });
        },
        bankSaved: function (data) {
            if (data.ResponseType == 1) {
                //si la respuesta es OK
                //agregar elemento a lista
                this.Bank.BankID = data.ItemID;
                this.Bank.Terminal = $('#TerminalID option:selected').text();
                this.Banks.push(this.Bank);
                //ocultar formulario
                this.showBankInfo = false;
                //notificación
                $.alert({
                    title: 'Bank Succesfully Saved',
                    content: this.Bank.BankName + ' was succesfully saved.',
                    animation: 'zoom',
                    closeAnimation: 'scale',
                    autoClose: 'ok|3000',
                    type: 'green'
                });
            } else {
                //notificar el error al guardar
                $.alert({
                    title: 'Error Saving',
                    content: 'There was an error trying to save ' + this.Bank.BankName + '. Try again.',
                    animation: 'zoom',
                    closeAnimation: 'scale',
                    autoClose: 'ok|3000',
                    type: 'red'
                });
            }            
        },
        hideBankInfo: function () {
            //ocultar el formulario
            this.showBankInfo = false;
        }
    },
    mounted: function () {
        //iniciar la sesión
        this.Session().getSessionDetails();

        //iniciar datepickers
        $('.datetimepicker-input').datetimepicker({
            format: 'YYYY-MM-DD'    
        }).on('datetimepicker.hide', function () {
            this.dispatchEvent(new Event('input'));
        });

        //iniciar selectores múltiples
        $('[multiple="multiple"]').multiselect({
            buttonWidth: '100%',
            includeSelectAllOption: true,
        });

        //si hay más de una terminal seleccionada, mostrar la búsqueda por default
        if (this.Shared.Session.TerminalIDs.split(',').length != 1) {
            this.UI().showSearchCard();
        } else {
            //de lo contrario, correr la búsqueda para la terminal seleccionada
            $('#btnSearchBanks').trigger('click');
        }
    }
})
