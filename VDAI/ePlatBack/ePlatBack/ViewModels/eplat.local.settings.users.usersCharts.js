var usersOrgCharts = new Vue({
    mixins: [ePlatUtils],
    el: '#app',
    data: {
        Shared: ePlatStore,
        OrgCharts: [],
        users: [],
        userList:[],
        user:
			{
			    userID : null,
			    userName : '',
			    firstName: '',
			    lastName: '',
			    locked: false,
			    email: '',
			    system: '',
			    supervisorID: null,
                supervisorName: null,
			    supervisorAgentID:null,
			    supervisors: [],
			},
        orgChart: true,
        orgCharInfo: false
    },
    computed:{},
    methods: {
    	setUsers:function(data)
    	{
    		let self = this;
    		self.OrgCharts = data.OrgCharts;
    		self.users = data.userList;
    		
    		google.charts.load('current', { packages: ["orgchart"] });
    		google.charts.setOnLoadCallback(drawChart);
    		function drawChart()
    		{
    			var org = new google.visualization.DataTable();
    			org.addColumn('string', 'Name');
    			org.addColumn('string', 'Manager');
    			org.addColumn('string', 'ToolTip');
    			// For each orgchart box, provide the name, manager, and tooltip to show.
    			//  [{v:clave,f:valor},'nodoPadre','Comentario'], //Objeto 1 
    			$.each(self.OrgCharts, function (index, value) {
    				org.addRows([[{ v: value.v, f: value.f }, value.nodoPadre, value.comentario]]);
    			});
    			// Create the chart.
    			var chart = new google.visualization.OrgChart(document.getElementById('chart_div'));
    			// Draw the chart, setting the allowHtml option to true for the tooltips.
    			chart.draw(org, { allowHtml: true });

    			// Every time the table fires the "select" event, it should call your
    			// selectHandler() function.
    			google.visualization.events.addListener(chart, 'select', function () {
    				var selection = chart.getSelection();
    				if(selection.length>0)
    				{
    					self.getUser(org.getValue(selection[0].row,0));
    				}
    			});
    		}
    	},
    	getUser:function(userID)
    	{
    	    let self = this;       
    	    for (x = 0; x < usersOrgCharts.users.length; x++)
    		{
    			if(self.users[x].userID == userID)
    			{
    			    self.user = self.users[x];
    			    $.each(self.OrgCharts, function (index, value)
    			    {
    			        $('#supervisorID option[value=' + value.v + ']').show();
                        if(value.nodoPadre == self.user.userID)
                            $('#supervisorID option[value=' + value.v + "]").hide();
    			    });
    			    if ($('#supervisorID option[value="' + self.user.supervisorID + '"]').length == 0)
    			        $('#supervisorID').append('<option value="' + self.user.supervisorID + '">' + self.user.supervisorName + '</option>');
    			    self.showModal();
    			}
    		}
    	},
    	saveUserInfo:function(data)
    	{
    	    if (data.ResponseType == 1) {
    	        //mensaje de confirmación
    	        $.alert({
    	            title: 'Save User Info',
    	            content: data.ResponseMessage,
    	            animation: 'zoom',
    	            closeAnimation: 'scale',
    	            autoClose: 'ok|5000',
    	            type: 'green'
    	        });
    	    } else {
    	        $.alert({
    	            title: 'Error trying to save information',
    	            content: data.ResponseMessage,
    	            animation: 'zoom',
    	            closeAnimation: 'scale',
    	            autoClose: 'ok|5000',
    	            type: 'red'
    	        });
    	    }
            this.hideModal();
    	},
    	updateSupervisor: function ()
    	{
    	    var user = usersOrgCharts.user;
    	    var userList = usersOrgCharts.users;
    	    var supervisorID = this.user.supervisorID;
    	    var agentUserID = this.user.userID;

    	    $.each(usersOrgCharts.OrgCharts, function (index, value) {
    	        if (value.v == user.userID) {
    	            value.nodoPadre = value.nodoPadre == null ? '' : supervisorID;
    	            value.f = (user.firstName == null ? '' : user.firstName) + " " + (user.lastName == null ? '' : user.lastName) + "<div class=" + '"' + "row" + '"' + ">" +
                                           "<div class=" + '"' + "col-md-12 col-lg-12 text-center" + '"' + '>'
                                           + "<h6>" + user.jobPosition + "</h6>" + "Last Activity Date" + "<div>" + moment(user.lastActivityDate).format("YYYY-MM-DD") + "</div>" + "</div>" + "</div>";
    	        }
    	    });
    	    usersOrgCharts.userList = userList;
    	    usersOrgCharts.setUsers(usersOrgCharts);
    	},
        showModal: function () {
            this.$refs.ModalRef.show(); 
        },
        hideModal: function () {
            this.$refs.ModalRef.hide();
        }
    },
    mounted: function () {
        //iniciar la sesión
        this.Session().getSessionDetails();
        //DependentFields
        this.UI().loadDependentFields("/settings/Users/GetDependantFieldsFromSystemUsers", true);     
        this.UI().xhrTargets();
        //show search card
        this.UI().showSearchCard();
        $('#saveInfo').on('click', this.updateSupervisor);
    }
});