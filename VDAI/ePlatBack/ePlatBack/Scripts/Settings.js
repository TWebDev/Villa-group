//Contains the parameter definitions for functions declared in UTILS.
var OPTIONS = {
    messageBox: {
        boxTypes: {
            messageBox: "messageBox",
            confirmBox: "confirmBox",
            twoActionBox: "twoActionBox"
        },
        messageTypes: {
            confirmation: 1,
            warning: 0,
            error: -1
        }
    },
    dataTable: {
        tagStyle: {
            table: {
                table: {
                    tag: "table",
                    attributes: "",
                    cssClass:""
                },
                thead: {
                    tag: "thead",
                    attributes: "",
                    cssClass: ""
                },
                tbody: {
                    tag: "tbody",
                    attributes: "",
                    cssClass: ""
                },
                tfoot: {
                    tag: "tfoot",
                    attributes: "",
                    cssClass: ""
                },
                tr: {
                    tag: "tr",
                    attributes: "",
                    cssClass: ""
                },
                th: {
                    tag: "th",
                    attributes: "",
                    cssClass: ""
                },
                td: {
                    tag: "td",
                    attributes: "",
                    cssClass: ""
                },
                caption: {
                    tag: "caption",
                    attributes: "",
                    cssClass: ""
                },
                col: {
                    tag: "col",
                    attributes: "",
                    cssClass: ""
                },
                colgroup: {
                    tag: "colgroup",
                    attributes: "",
                    cssClass: ""
                },

            },
            div: {
                table: {
                    tag: "div",
                    attributes: " class='div_TABLE' ",
                    cssClass: "div_TABLE"
                },
                thead: {
                    tag: "div",
                    attributes: " class='div_THEAD' ",
                    cssClass: "div_THEAD"
                },
                tbody: {
                    tag: "div",
                    attributes: " class='div_TBODY' ",
                    cssClass: "div_TBODY"
                },
                tfoot: {
                    tag: "div",
                    attributes: " class='div_TFOOT' ",
                    cssClass: "div_TFOOT"
                },
                tr: {
                    tag: "div",
                    attributes: " class='div_TR' ",
                    cssClass: "div_TR"
                },
                th: {
                    tag: "div",
                    attributes: " class='div_TH' ",
                    cssClass: "div_TH"
                },
                td: {
                    tag: "div",
                    attributes: " class='div_TD' ",
                    cssClass: "div_TD"
                },
                caption: {
                    tag: "div",
                    attributes: " class='div_CAPTION' ",
                    cssClass: "div_CAPTION"
                },
                col: {
                    tag: "div",
                    attributes: " class='div_COL' ",
                    cssClass: "div_COL"
                },
                colgroup: {
                    tag: "div",
                    attributes: " class='div_COLGROUP' ",
                    cssClass: "div_COLGROUP"
                },

            }

        },
        cellTypes: {
            action: "action",
            data: "data",
            hidden: "hidden"
        },
        actionTypes: {
            remove: "remove",
            edit: "edit"           
        },
        actionIcons: {
            remove: "/Content/themes/base/images/trash.png",
            edit: "",
        },
        dataAttributes: {
            cellType: "cell-type",
            actionType: "action-type",
            value: "value",
        },
        actionButtons: {
            add: "add",
            update: "update",
            cancel:"cancel"
        }
        
    }
}

var DEFAULTS = {
    messageBox:{
        duration:5,
        boxType: OPTIONS.messageBox.boxTypes.messageBox,
        type: OPTIONS.messageBox.messageTypes.confirmation,
    }
}
var SETTINGS = function () {
    //Defines the parameters for UTILS.searchTextInColumns.
    function confirmBox() {
        var cfb = new messageBox();     

        cfb.type = OPTIONS.messageBox.messageTypes.warning;
        cfb.boxType = OPTIONS.messageBox.boxTypes.confirmBox;        
        cfb.duration = -1;
        //cfb.message = confirmationText;
        //cfb.onAcceptCallBack = null;
        //cfb.onAcceptParams = onAcceptParams;
        return cfb;
    }
    function messageBox() {
        this.type = DEFAULTS.messageBox.type;
        this.message = "";
        this.duration = DEFAULTS.messageBox.duration;
        this.innerException = "";
        this.boxType = DEFAULTS.messageBox.boxType;
        this.onAcceptCallBack = null;
        this.onAcceptParams = null;
        //mike
        this.onAcceptButtonValue = null;
        this.onCancelCallBack = null;
        this.onCancelParams = null;
        this.onCancelButtonValue = null;
    }

    function twoActionBox() {
        var tab = new messageBox();
        tab.type = OPTIONS.messageBox.messageTypes.confirmation;
        //tab.type = OPTIONS.messageBox.messageTypes.warning;
        tab.boxType = OPTIONS.messageBox.boxTypes.twoActionBox;
        tab.duration = -1;
        return tab;
    }

    function highlightElements() {
        this.highlightTime = 5000; //miliseconds
        this.cssClass = "input-validation-error";
        this.elements = [];
    }

    function getDataFromColumn() {
        this.tableID = "";
        this.columnIndex = 1;
        this.separator = ";";
    }

    function tableDataToJason() {
        this.tableID = "";
        this.columnIndexes = undefined;
        this.subject = "items";
    }
    function tableDataValuesToJason() {
        this.tableID = "";
        this.columnIndexes = undefined;
        this.subject = "items";
    }
    function searchTextInColumns() {
        ///<summary>Defines the search parameters</summary>
        this.text = "";
        this.searchType = "containsExact";
        //contains,containsExact,containsExactCase,containsRegex
        this.tableID = "";
        this.specifiedColumns = [];
        this.matchIdentifier = "_searchTextInColumnsMatch";
    }

    //Defines the parameters for UTILS.newTableRows.
    function newTableRows() {
        this.dataRows = [];//The data which the rows are going to be created from.
        this.options = new SETTINGS.newTableRowsOptions();
    }
    //Defines the parameters for UTILS.newTableRow.settings
    function newTableRowsOptions() {
        this.dataColumns = [];
        this.addDeleteIcon = true;
        this.deleteIconCallBack = function () { };
    }
    //Defines the parameters for UTILS.getInfoAsync.
    function getInfoAsync() {
        this.URL = "";
        this.method = "POST";
        this.data = {};
        this.onCompleteCallback = function () { };
        return this;
    }
    function addToListTable()
    {
        this.fieldID = "";
        this.tableID = "";
        this.newValue = "";
        this.allowDuplicateValues = false;
        this.allowEmptyValues = false;
        this.validateWithDataAnnotation = true;
        this.newItemOptions = new SETTINGS.newTableRowsOptions();
    }

    return {
        searchTextInColumns: searchTextInColumns,
        newTableRows: newTableRows,
        newTableRowsOptions: newTableRowsOptions,
        getInfoAsync: getInfoAsync,
        highlightElements: highlightElements,
        getDataFromColumn: getDataFromColumn,
        addToListTable: addToListTable,
        tableDataToJason: tableDataToJason,
        tableDataValuesToJason: tableDataValuesToJason,
        messageBox: messageBox,
        confirmBox: confirmBox,
        twoActionBox: twoActionBox
    }
}();
