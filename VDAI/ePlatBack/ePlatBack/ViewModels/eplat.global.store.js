var ePlatStore = {
    debug: true,
    State: {
        message: '',
        DependentFields: {
            Fields: [],
            Url: '',
            UpdateOnTerminalChange: false
        }
    },
    Session: {
        TerminalIDs: localStorage.Eplat_SelectedTerminals,
        TerminalIDsArr: (localStorage.Eplat_SelectedTerminals != undefined ?  localStorage.Eplat_SelectedTerminals.split(',') : []),
        SelectedTerminalsNames: '',
        WorkGroupID: localStorage.Eplat_SelectedWorkGroupID,
        RoleID: localStorage.Eplat_SelectedRole,
        Username: localStorage.Eplat_LastLogin,
        FirstName: '',
        LastName: '',
        Photo: '',
        Role: '',
        WorkGroup: '',
        UserTerminals: [],
        Culture: 'en-US',
        TerminalsList: []
    }
}