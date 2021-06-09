RequestTargets = [
	{
	    url: '/Account/GetWorkGroupsByUser',
	    target: 'StatusBar',
	    message: 'Getting Work Groups',
	    page: 'all',
        block: ''
	},
    {
        url: '/Account/GetWorkGroupsByUser?userName=x',
        target: 'StatusBar',
        message: 'Getting Work Groups',
        page: 'all',
        block: ''
    },
    {
        url: '/Account/GetTerminalsByUser',
        target: 'StatusBar',
        message: 'Getting List of Terminals',
        page: 'all',
        block: ''
    },
    {
        url: '/Home/WikiFieldsWithContent/',
        target: '',
        message: 'Background Operation',
        page: 'all',
        block: ''
    },
    {
        url: '/Account/GetRolesByUser',
        target: '',
        message: 'Background Operation',
        page: 'all',
        block: ''
    },
    {
        url: '/Account/GetMenuComponents',
        target: 'menu',
        message: 'Loading Menu',
        page: 'all',
        block: ''
    },
    {
        url: '/Account/SaveTicket',
        target: '',
        message: 'Background Operation',
        page: 'all',
        block: ''
    },
    {
        url: '/crm/masterchart/Search?Length=x&selectedTerminals=x',
        target: 'StatusBar',
        message: 'Getting Leads',
        page: 'crm > master chart',
        block: ''
    },
    {
        url: '/crm/timeshare/SearchEgresses?Length=x',
        target: 'StatusBar',
        message: 'Getting Charges',
        page: 'crm > timeshare',
        block: ''
    },
    {
        url: '/TimeShare/GetEgressInfo',
        target: 'StatusBar',
        message: 'Getting Charge Info',
        page: 'crm > timeshare',
        block: ''
    },
    {
        url: '/crm/timeshare/SaveEgress?Length=x',
        target: 'StatusBar',
        message: 'Saving Gifting Info',
        page: 'crm > reports',
        block: 'btnSaveEgress'
    },
    {
        url: '/crm/activities/SearchActivities?Length=x',
        target: 'StatusBar',
        message: 'Getting Activities List',
        page: 'cms > activities',
        block: ''
    },
    {
        url: '/Activities/GetActivityDescription',
        target: 'StatusBar',
        message: 'Getting Activity Description',
        page: 'cms > activities',
        block: ''
    },
    {
        url: '/crm/Activities/SavePriceTypeRule?Length=x',
        target: 'StatusBar',
        message: 'Saving Rule Info',
        page: 'cms > activities',
        block: 'btnSavePriceTypeRule'
    },
    {
        url: '/CRM/masterchart/FindLead',
        target: 'StatusBar',
        message: 'Finding Lead Information',
        page: 'crm > masterchart',
        block: ''
    },
    {
        url: '/CRM/Masterchart/FindReservation',
        target: 'StatusBar',
        message: 'Finding Reservation Information',
        page: 'crm > masterchart',
        block: ''
    },
    {
        url: '/MasterChart/GetPurchase',
        target: 'StatusBar',
        message: 'Getting Purchase Info',
        page: 'crm > masterchart',
        block: ''
    },
    {
        url: '/MasterChart/GetPurchaseService',
        target: 'StatusBar',
        message: 'Getting Coupon Info',
        page: 'crm > masterchart',
        block: ''
    },
    {
        url: '/MasterChart/GetPurchasePayment',
        target: 'StatusBar',
        message: 'Getting Payment Info',
        page: 'crm > masterchart',
        block: ''
    },
    /*{
        url: '/Activities/GetActivityInfo',
        target: 'StatusBar',
        message: 'Getting Activity Info',
        page: 'cms > activities',
        block: ''
    },*/
    {
        url: '/crm/places/SearchPlaces?Length=x',
        target: 'StatusBar',
        message: 'Getting Places List',
        page: 'cms > places',
        block: ''
    },
    {
        url: '/Places/GetPlace',
        target: 'StatusBar',
        message: 'Getting Place Info',
        page: 'cms > places',
        block: ''
    },
    {
        url: '/Admin/GetComponentsTree',
        target: 'StatusBar',
        message: 'Getting System Components',
        page: 'settings > admin',
        block: ''
    },
    {
        url: '/Admin/GetPrivileges',
        target: 'StatusBar',
        message: 'Getting Privileges',
        page: 'settings > admin',
        block: ''
    },
    {
        url:'/Admin/SaveSysProfile',
        target: 'StatusBar',
        message: 'Saving Profile Info',
        page: 'settings > admin',
        block: 'btnSaveProfile'
    },
    {
        url: '/crm/terminals/SearchTerminals?Length=x',
        target: 'StatusBar',
        message: 'Getting Terminals List',
        page: 'settings > terminals',
        block: ''
    },
    {
        url: '/Terminals/GetTerminal',
        target: 'StatusBar',
        message: 'Getting Terminal Info',
        page: 'settings > terminals',
        block: ''
    },
    {
        url: '/crm/users/Search?Length=x',
        target: 'StatusBar',
        message: 'Searching Users',
        page: 'settings > users',
        block: ''
    },
    {
        url: 'Users/GetUserInfo',
        target: 'StatusBar',
        message: 'Getting User Info',
        page: 'settings > users',
        block: ''
    },
    {
        url: '/crm/users/SaveUser?Length=x',
        target: 'StatusBar',
        message: 'Saving User Info',
        page: 'settings > users',
        block: 'btnSaveUser'
    },
    {
        url: '/crm/Catalogs/SearchOPCS?Length=x',
        target: 'StatusBar',
        message: 'Searching OPCs',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/Catalogs/GetOPC',
        target: 'StatusBar',
        message: 'Getting Info',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/crm/Catalogs/SearchCommissions?Length=x',
        target: 'StatusBar',
        message: 'Getting Commissions List',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/Catalogs/GetCommission',
        target: 'StatusBar',
        message: 'Getting Commission Info',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/crm/Catalogs/SearchExchangeRates?Length=x',
        target: 'StatusBar',
        message: 'Searching Exchange Rates',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/Catalogs/GetExchangeRate',
        target: 'StatusBar',
        message: 'Getting Exchange Rate Info',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/crm/Reports/SearchCashStatement?Length=x',
        target: 'StatusBar',
        message: 'Getting Statement',
        page: 'crm > reports > petty cash statement',
        block: ''
    },
    {
        url: '/crm/Reports/SearchPricesReport?Length=x',
        target: 'StatusBar',
        message: 'Getting Prices',
        page: 'crm > reports > prices',
        block: ''
    },
    {
        url: '/crm/Reports/SearchCloseOutsHistory?Length=x',
        target: 'StatusBar',
        message: 'Getting Close Outs History',
        page: 'crm > reports > close out history',
        block: ''
    },
    {
        url: '/crm/Reports/SearchCloseOut?Length=x',
        target: 'StatusBar',
        message: 'Getting Close Out',
        page: 'crm > reports > close out',
        block: ''
    },
    {
        url: '/crm/Reports/SaveCloseOut',
        target: 'StatusBar',
        message: 'Saving Close Out',
        page: 'crm > reports > close out',
        block: 'btnPrintCloseOut'
    },
    {
        url: '/crm/Reports/SearchPolicy?Length=x',
        target: 'StatusBar',
        message: 'Getting Policy',
        page: 'crm > reports > income policy',
        block: ''
    },
    {
        url: '/crm/Reports/SearchCommissions?Length=x',
        target: 'StatusBar',
        message: 'Calculating Demonstrations',
        page: 'crm > reports > demonstrations',
        block: ''
    },
    {
        url: '/crm/Reports/SearchCoupon?Length=x',
        target: 'StatusBar',
        message: 'Getting Coupon Info',
        page: 'crm > reports > audit coupon',
        block: 'Search_Folio'
    },
    {
        url: '/crm/Reports/SearchInvoice?Length=x',
        target: 'StatusBar',
        message: 'Generating Invoice',
        page: 'crm > reports > invoice',
        block: ''
    },
    {
        url: '/crm/Reports/SearchChargeBacks?Length=x',
        target: 'StatusBar',
        message: 'Getting Report',
        page: 'crm > reports > timeshare operation',
        block: ''
    },
    {
        url: '/crm/Reports/SetCustomLayout?Length=x',
        target: 'StatusBar',
        message: 'Saving Layout',
        page: 'crm > reports > prices custom report',
        block: ''
    },
    {
        url: '/Reports/GetReportLayout/x',
        target: 'StatusBar',
        message: 'Getting Layout Info',
        page: 'crm > reports > prices custom report',
        block: ''
    },
    {
        url: '/Reports/DeleteReportLayout/x',
        target: 'StatusBar',
        message: 'Deleting Layout',
        page: 'crm > reports > prices custom report',
        block: ''
    },
    {
        url: '/crm/Reports/SearchPricesCustomReport?Length=x',
        target: 'StatusBar',
        message: 'Getting Prices Report',
        page: 'crm > reports > prices custom report',
        block: ''
    },
    {
        url: '/crm/Reports/SearchProduction?Length=x',
        target: 'StatusBar',
        message: 'Getting Report',
        page: 'crm > reports > production per activity',
        block: ''
    },
    {
        url: '/crm/Reports/SearchCategoriesProduction?Length=x',
        target: 'StatusBar',
        message: 'Getting Report',
        page: 'crm > reports > production per category',
        block: ''
    },
    {
        url: '/crm/Reports/SearchCouponSales?Length=x',
        target: 'StatusBar',
        message: 'Getting Coupons Report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/masterchart/SavePurchase?Length=x',
        target: 'StatusBar',
        message: 'Saving Purchase Info',
        page: 'crm > masterchart',
        block: 'btnSavePurchase'
    },
    {
        url: '/MasterChart/SavePurchaseService',
        target: 'StatusBar',
        message: 'Saving Coupon Info',
        page: 'crm > masterchart',
        block: 'btnSavePurchaseService'
    },
    {
        url: '/crm/masterchart/SavePayment?Length=x',
        target: 'StatusBar',
        message: 'Saving Payment Info',
        page: 'crm > masterchart',
        block: 'btnSavePurchasePayment,messageBoxBtnConfirm,messageBoxBtnCancel'
    },
    {
        url: '/crm/Reports/SearchCouponsHistory?Length=x',
        target: 'StatusBar',
        message: 'Getting Coupons List',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchProvidersProduction?Length=x',
        target: 'StatusBar',
        message: 'Getting Production Report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/MasterChart/SaveFastSale',
        target: 'StatusBar',
        message: 'Saving Lead and Purchase Info',
        page: 'crm > masterchart',
        block: 'btnSaveFastSale'
    },
    {
        url: '/crm/MasterChart/SaveBillingInfo?Length=x',
        target: 'StatusBar',
        message: 'Saving Billing Info',
        page: 'crm > masterchart',
        block: 'btnSaveBillingInfo,btnSaveAndReturnBillingInfo'
    },
    {
        url: '/crm/catalogs/SaveOPC?Length=x',
        target: 'StatusBar',
        message: 'Saving Info',
        page: 'settings > catalogs',
        block: 'btnSaveOPCInfo'
    },
    {
        url: '/MasterChart/DeletePayment',
        target: 'StatusBar',
        message: 'Deleting Payment',
        page: 'crm > masterchart',
        block: ''
    },
    {
        url: '/crm/Catalogs/SaveExchangeRate?Length=x',
        target: 'StatusBar',
        message: 'Saving Exchange Rate',
        page: 'crm > catalogs',
        block: 'btnSaveExchangeRate'
    },
    {
        url: '/crm/Reports/SearchUserPermissions?Length=x',
        target: 'StatusBar',
        message: 'Getting Users List',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/MasterChart/GetNextCouponFolio',
        target: 'StatusBar',
        message: 'Getting Next Folio Available',
        page: 'crm > masterchart',
        block: 'btnSavePurchaseService'
    },
    {
        url: '/MasterChart/GetDDLData',
        target: 'StatusBar',
        message: 'Getting Units & Prices',
        page: 'crm > masterchart',
        block: 'btnAddPrice'
    },
    {
        url: '/MasterChart/SetPurchaseServiceAsIssued',
        target: 'StatusBar',
        message: 'Getting Coupon',
        page: 'crm > masterchart',
        block: '.print-coupon'
    },
    {
        url: '/MasterChart/SendCouponsByEmail',
        target: 'StatusBar',
        message: 'Sending Coupon(s) by email',
        page: 'crm > masterchart',
        block: '.send-coupons'
    },
    {
        url: '/crm/Reports/SaveProviderInvoice?Length=x',
        target: 'StatusBar',
        message: 'Saving Provider Invoice',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchAuditInvoices?Length=x',
        target: 'StatusBar',
        message: 'Searching Saved Invoices',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/GetProviderInvoice/x',
        target: 'StatusBar',
        message: 'Getting Invoice Info',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchPaymentsAssignation?Length=x',
        target: 'StatusBar',
        message: 'Getting Payments Assignation',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/MasterChart/GetExchangeRateOfPurchase',
        target: 'StatusBar',
        message: 'Getting Payments History',
        page: 'crm > masterchart',
        block: ''
    },
    {
        url: '/crm/surveys/SearchSurvey?Length=x',
        target: 'StatusBar',
        message: 'Searching',
        page: 'settings > surveys',
        block: ''
    },
    {
        url: '/crm/surveys/SaveSurvey?Length=x',
        target: 'StatusBar',
        message: 'Saving Survey',
        page: 'settings > surveys',
        block: ''
    },
    {
        url: '/settings/surveys/GetSurvey/x',
        target: 'StatusBar',
        message: 'Getting Survey',
        page: 'settings > surveys',
        block: ''
    },
    {
        url: '/crm/Catalogs/SearchProviders?Length=x',
        target: 'StatusBar',
        message: 'Searching Providers',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/Providers/GetProvider',
        target: 'StatusBar',
        message: 'Getting Provider Info',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/crm/Providers/SaveProvider?Length=x',
        target: 'StatusBar',
        message: 'Saving Provider Info',
        page: 'settings > catalog',
        block: 'btnSaveProvider'
    },
    {
        url: '/settings/surveys/GetStats/x',
        target: 'StatusBar',
        message: 'Getting Stats',
        page: 'settings > surveys',
        block: 'btnGetStats'
    },
    {
        url: '/settings/surveys/SendEmail/',
        target: 'StatusBar',
        message: 'Sending Email',
        page: 'settings > surveys',
        block: 'btnSendMailContent'
    },
    {
        url: '/crm/Activities/SaveActivity?Length=x',
        target: 'StatusBar',
        message: 'Saving Activity Info',
        page: 'cms > activities',
        block: 'btnSaveActivity'
    },
    {
        url: '/crm/AccountingAccounts/SearchAccountingAccounts?Length=x',
        target: 'StatusBar',
        message: 'Getting Accounting Accounts List',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/crm/AccountingAccounts/SaveAccountingAccount?Length=x',
        target: 'StatusBar',
        message: 'Saving Accounting Account',
        page: 'settings > catalogs',
        block: 'btnSaveAccountingAccountInfo'
    },
    {
        url: '/AccountingAccounts/GetAccountingAccount',
        target: 'StatusBar',
        message: 'Getting Accounting Account',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/crm/Activities/SaveActivityAccountingAccount?Length=x',
        target: 'StatusBar',
        message: 'Saving Accounting Account',
        page: 'cms > activities',
        block: 'btnSaveAccountingAccount'
    },
    {
        url: '/Activities/GetActivityAccountingAccount',
        target: 'StatusBar',
        message: 'Getting Activity Accounting Account Info',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/Activities/DeleteActivityAccountingAccount',
        target: 'StatusBar',
        message: 'Deleting Activity Accounting Account',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/crm/Catalogs/SearchBudgets?Length=x',
        target: 'StatusBar',
        message: 'Getting Budgets',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/MasterChart/GetChargeBackTicketInfo',
        target: 'StatusBar',
        message: 'Getting Chargeback Information',
        page: 'crm > masterchart',
        block: '.print-chargeback'
    },
    {
        url: '/crm/Reports/GetAuditDetails/x',
        target: 'StatusBar',
        message: 'Getting Audit Details',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Timeshare/SearchIncomes?Length=x',
        target: 'StatusBar',
        message: 'Getting Incomes',
        page: 'crm > timeshare',
        block: ''
    },
    {
        url: '/Timeshare/GetIncomeInfo',
        target: 'StatusBar',
        message: 'Getting Income Info',
        page: 'crm > timeshare',
        block: ''
    },
    {
        url: '/crm/Reports/SearchBilling?Length=x',
        target: 'StatusBar',
        message: 'Getting Billing Report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/Prices/RenderPricesWizard',
        target: 'StatusBar',
        message: 'Getting Wizard Info',
        page: 'cms > prices',
        block: ''
    },
    {
        url: '/crm/Reports/SearchConcierges?Length=x',
        target: 'StatusBar',
        message: 'Getting Concierge Report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchPipeline?Length=x',
        target: 'StatusBar',
        message: 'Getting Pipeline Report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchIndicators?Length=x',
        target: 'StatusBar',
        message: 'Getting Indicators Report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchExchangeTours?Length=x',
        target: 'StatusBar',
        message: 'Getting Exchange Tours Report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchPreBookedArrivals?Length=x',
        target: 'StatusBar',
        message: 'Getting Prebooked Arrivals Report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchPreBookedContactedLeads?Length=x',
        target: 'StatusBar',
        message: 'Getting Prebooked & Contacted Leads Report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchNewReferrals?Length=x',
        target: 'StatusBar',
        message: 'Getting New Referrals Report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchOptionsSold?Length=x',
        target: 'StatusBar',
        message: 'Getting Options Sold Report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchQuoteRequests?Length=x',
        target: 'StatusBar',
        message: 'Getting Quote Requests',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchSweepstakes?Length=x',
        target: 'StatusBar',
        message: 'Getting Sweepstakes',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchPreCheckIn?Length=x',
        target: 'StatusBar',
        message: 'Getting 22 Day Arrival Report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/MasterChart/GetDDLData?itemType=priceTypeRule&itemID=x',
        target: 'StatusBar',
        message: 'Getting Price Types',
        page: 'crm > masterchart',
        block: 'btnAddPrice'
    },
    {
        url: '/crm/MasterChart/SendConfirmationLetter',
        target: 'StatusBar',
        message: 'Sending Confirmation Letter',
        page: 'crm > masterchart',
        block: 'btnSendConfirmation'
    },
    {
        url: '/crm/Reports/SearchIOBalance?Length=x',
        target: 'StatusBar',
        message: 'Getting Balance',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/masterchart/SaveLead?Length=x&selectedTerminals=x',
        target: 'StatusBar',
        message: 'Saving Lead Information',
        page: 'crm > masterchart',
        block: 'btnSaveLead,btnSaveContinueLead'
    },
    {
        url: '/crm/Reports/SaveCustomCost',
        target: 'StatusBar',
        message: 'Saving Custom Cost',
        page: 'crm > reports > audit coupon',
        block: ''
    },
    {
        url: '/crm/masterchart/SearchCoupons?Length=x',
        target: 'StatusBar',
        message: 'Getting Coupons',
        page: 'crm > masterchart',
        block: ''
    },
    {
        url: '/settings/surveys/PublishField',
        target: 'StatusBar',
        message: 'Saving',
        page: 'settings > surveys',
        block: ''
    },
    {
        url: '/crm/Reports/DeleteCloseOut',
        target: 'StatusBar',
        message: 'Deleting Close Out',
        page: 'crm > reports',
        block: 'btnDeleteCloseOut'
    },
    {
        url: '/crm/Reports/SearchWeekly?Length=x',
        target: 'StatusBar',
        message: 'Getting Report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchDynamic?Length=x',
        target: 'StatusBar',
        message: 'Getting Report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchConfirmedLeads?Length=x',
        target: 'StatusBar',
        message: 'Getting Report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchLegacyUserPermissions?Length=x',
        target: 'StatusBar',
        message: 'Getting Report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchReservationsMade?Length=x',
        target: 'StatusBar',
        message: 'Getting Report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchRoomUpgrades?Length=x',
        target: 'StatusBar',
        message: 'Getting Report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/Pictures/RenderPictureDescription',
        target: 'StatusBar',
        message: 'Getting Picture Descriptions',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/masterchart/SaveReservation?Length=x&selectedTerminals=x',
        target: 'StatusBar',
        message: 'Saving Reservation Info',
        page: 'crm > masterchart',
        block: ''
    },
    {
        url: '/Prices/GetPrices',
        target: 'StatusBar',
        message: 'Getting Service Prices',
        page: 'cms > activities',
        block: ''
    },
    {
        url: '/Activities/GetPriceTypeRules',
        target: 'StatusBar',
        message: 'Getting Prices Rules',
        page: 'cms > activities',
        block: ''
    },
    {
        url: '/crm/catalogs/SaveCommission?Length=x',
        target: 'StatusBar',
        message: 'Saving Commission Rule',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/Prices/SavePrice',
        target: 'StatusBar',
        message: 'Saving Price',
        page: 'settings > catalogs',
        block: 'btnSavePrice'
    },
    {
        url: '/crm/catalogs/SearchBudgets?Length=x',
        target: 'StatusBar',
        message: 'Getting Budgets',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/Masterchart/GetCouponRefObj',
        target: 'StatusBar',
        message: 'Getting Coupon Reference',
        page: 'crm > masterchart',
        block: ''
    },
    {
        url: '/Account/GetWorkGroupsByUser?userName=x',
        target: 'StatusBar',
        message: 'Getting User Workgroup',
        page: 'account > logon',
        block: ''
    },
    {
        url: '/crm/Reports/SearchArrivals?Length=x',
        target: 'StatusBar',
        message: 'Getting Arrivals',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchDuplicateLeads?Length=x',
        target: 'StatusBar',
        message: 'Getting Duplicate Leads',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/MassUpdate?Length=x',
        target: 'StatusBar',
        message: 'Saving Mass Update',
        page: 'crm > reports',
        block: 'btnMassUpdate'
    },
    {
        url: '/crm/MasterChart/GetDDLData?itemType=x&itemID=x',
        target: 'StatusBar',
        message: 'Getting List(s) Info',
        page: 'crm > masterchart',
        block: 'btnMassUpdate'
    },
    {
        url: '/crm/Reports/SearchMasterCloseOut?Length=x',
        target: 'StatusBar',
        message: 'Getting Master Close Out',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/ProcessCouponInfo/x',
        target: 'StatusBar',
        message: '.',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchBudgetsViewModel?Length=x',
        target: 'StatusBar',
        message: 'Getting Budgets',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/MasterChart/RenderPurchaseTicket',
        target: 'StatusBar',
        message: 'Generating/Getting Ticket',
        page: 'crm > masterchart',
        block: '.print-ticket'
    },
    {
        url: '/crm/masterchart/SearchCouponsByDate?Length=x',
        target: 'StatusBar',
        message: 'Getting Coupons',
        page: 'crm > masterchart',
        block: ''
    },
    {
        url: '/Activities/GetActivityMeetingPoint',
        target: 'StatusBar',
        message: 'Getting Meeting Point Info',
        page: 'cms > activities',
        block: ''
    },
    {
        url: '/crm/Hostess/GetArrivals',
        target: 'StatusBar',
        message: 'Searching Guest',
        page: 'crm > hostess',
        block: ''
    },
    {
        url: '/crm/Hostess/GetArrivals?date=x&avoidRequestToFront=x&_=x',
        target: 'StatusBar',
        message: 'Getting Arrivals',
        page: 'crm > hostess',
        block: ''
    },
    {
        url: '/crm/Catalogs/SearchSalesRooms?Length=x',
        target: 'StatusBar',
        message: 'Getting Sales Room Parties',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/Catalogs/GetSalesRoomParties',
        target: 'StatusBar',
        message: 'Getting Sales Room Info',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/crm/emails/SearchEmails?Length=x',
        target: 'StatusBar',
        message: 'Getting Emails',
        page: 'settings > emails',
        block: ''
    },
    {
        url: '/Emails/GetEmailInfo',
        target: 'StatusBar',
        message: 'Getting Email Info',
        page: 'settings > emails',
        block: ''
    },
    {
        url: '/crm/emails/SaveEmail?Length=x',
        target: 'StatusBar',
        message: 'Saving Email',
        page: 'settings > emails',
        block: 'btnSaveEmail'
    },
    {
        url: '/crm/timeshare/SearchIncomes?Length=x',
        target: 'StatusBar',
        message: 'Getting Incomes',
        page: 'crm > timeshare',
        block: ''
    },
    {
        url: '/crm/timeshare/SaveIncome?Length=x',
        target: 'StatusBar',
        message: 'Saving Income',
        page: 'crm > timeshare',
        block: 'btnSaveIncome'
    },
    {
        url: '/TimeShare/GetIncomeInfo',
        target: 'StatusBar',
        message: 'Getting Income Info',
        page: 'crm > timeshare',
        block: ''
    },
    {
        url: '/crm/emails/SearchEmailNotifications?Length=x',
        target: 'StatusBar',
        message: 'Getting Emails Notifications',
        page: 'settings > emails',
        block: ''
    },
    {
        url: '/Emails/GetEmailNotificationInfo',
        target: 'StatusBar',
        message: 'Getting Email Notification Info',
        page: 'settings > emails',
        block: ''
    },
    {
        url: '/crm/emails/SaveEmailNotification?Length=x',
        target: 'StatusBar',
        message: 'Saving Email Notification',
        page: 'settings > emails',
        block: 'btnSaveEmailNotification'
    },
    {
        url: '/crm/Reports/SaveAsNoShow',
        target: 'StatusBar',
        message: 'Saving Status as No Show',
        page: 'crm > reports > coupons history',
        block: 'btnNoShow'
    },
    {
        url: '/crm/Reports/SaveAsConfirmed',
        target: 'StatusBar',
        message: 'Saving Status as Confirmed',
        page: 'crm > reports > coupons history',
        block: 'btnConfirmed'
    },
    {
        url: '/crm/Reports/searchAccountingAccounts?Length=x',
        target: 'StatusBar',
        message: 'Searching Accouting Accounts',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/hostess/SearchArrivalsForecast?Length=x',
        target: 'StatusBar',
        message: 'Getting Forecast',
        page: 'crm > hostess',
        block: 'btnGetForecast'
    },
    {
        url: '/crm/Activities/SaveStockTransaction?Length=x',
        target: 'StatusBar',
        message: 'Saving Stock Transaction',
        page: 'cms > activities',
        block: 'btnSaveStockTransaction'
    },
    {
        url: '/crm/Activities/SearchStockTransactions?Length=x',
        target: 'StatusBar',
        message: 'Getting Stock Transactions',
        page: 'cms > activities',
        block: ''
    },
    {
        url: '/crm/invitations/SaveInvitationInfo?Length=x',
        target: 'StatusBar',
        message: 'Saving Invitation Info',
        page: 'crm > invitations',
        block: 'btnSaveInvitationInfo'
    },
    {
        url: '/crm/Invitations/GetInvitations?date=x&_=x',
        target: 'StatusBar',
        message: 'Updating Invitations List',
        page: 'crm > invitations',
        block: 'btnGetInvitations'
    },
    {
        url: '/MasterChart/GetDDLData?itemType=minimalPrice&itemID=x',
        target: 'StatusBar',
        message: "Getting Unit's Minimal Price",
        page: 'crm > masterchart',
        block: '.edit-item'
    },
    {
        url: '/Activities/DeleteActivityAccountingAccounts',
        target: 'StatusBar',
        message: 'Deleting Relationships',
        page: 'cms > activities',
        block: 'btnDeleteAccounts'
    },
    {
        url: '/crm/Activities/UpdateStock?Length=x',
        target: 'StatusBar',
        message: 'Updating Minimal Stock',
        page: 'cms > activities',
        block: 'btnUpdateStock'
    },
    {
        url: '/crm/Reports/SearchDiamante?Length=x',
        target: 'StatusBar',
        message: 'Getting Report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/hostess/SearchPenetrationReport?Length=x',
        target: 'StatusBar',
        message: 'Getting Penetration Report',
        page: 'crm > hostess',
        block: ''
    },
    {
        url: '/PricesEditor/GetPricesInfo',
        target: 'StatusBar',
        message: 'Getting Prices Info',
        page: 'cms > activities',
        block: 'btnRun'
    },
    {
        url: '/crm/Catalogs/SearchOptions?Length=x',
        target: 'StatusBar',
        message: 'Getting Options',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/catalogs/GetOption',
        target: 'StatusBar',
        message: 'Getting Option Info',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/crm/Catalogs/SaveOption?Length=x',
        target: 'StatusBar',
        message: 'Saving Option',
        page: 'settings > catalogs',
        block: 'btnSaveOption'
    },
    {
        url: '/crm/Catalogs/SearchCouponFolios?Length=x',
        target: 'StatusBar',
        message: 'Getting Coupon Folios',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/Catalogs/GetCouponFolio',
        target: 'StatusBar',
        message: 'Getting Coupon Folio Info',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/crm/Catalogs/SearchCompanies?Length=x',
        target: 'StatusBar',
        message: 'Getting Companies',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/Catalogs/GetCompany',
        target: 'StatusBar',
        message: 'Getting Companies',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/crm/Catalogs/SearchLocations?Length=x',
        target: 'StatusBar',
        message: 'Getting Locations',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/Catalogs/GetLocation',
        target: 'StatusBar',
        message: 'Getting Location Info',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/crm/Catalogs/SearchPlaceClasifications?Length=x',
        target: 'StatusBar',
        message: 'Getting Places Clasification',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/Catalogs/GetPlaceClasification',
        target: 'StatusBar',
        message: 'Getting Place Clasification Info',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/crm/Catalogs/SearchPlaceTypes?Length=x',
        target: 'StatusBar',
        message: 'Getting Place Types',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/Catalogs/GetPlaceType',
        target: 'StatusBar',
        message: 'Getting Place Type Info',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/crm/Catalogs/SearchPointsOfSale?Length=x',
        target: 'StatusBar',
        message: 'Getting Points Of Sale',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/PointsOfSale/GetPointOfSale',
        target: 'StatusBar',
        message: 'Getting Point Of Sale Info',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/crm/Catalogs/SearchPromotionTeams?Length=x',
        target: 'StatusBar',
        message: 'Getting Promotion Teams',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/Catalogs/GetPromotionTeam',
        target: 'StatusBar',
        message: 'Getting Promotion Team Info',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/crm/Catalogs/SearchTransportationZones?Length=x',
        target: 'StatusBar',
        message: 'Getting Transportation Zones',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/Catalogs/GetTransportationZone',
        target: 'StatusBar',
        message: 'Getting Transportation Zone Info',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/crm/Catalogs/SearchZones?Length=x',
        target: 'StatusBar',
        message: 'Getting Zones',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/Catalogs/GetZone',
        target: 'StatusBar',
        message: 'Getting Zone Info',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/TimeShare/ResetVarApp',
        target: 'StatusBar',
        message: 'Resetting Application Variable',
        page: 'timeshare > incomes',
        block: ''
    },
    {
        url: '/TimeShare/GetFundsBalance',
        target: 'StatusBar',
        message: 'Getting Funds Balance',
        page: 'timeshare > funds',
        block: ''
    },
    {
        url: '/crm/Reports/SearchPrearrivalsFeedback?Length=x',
        target: 'StatusBar',
        message: 'Geeting Prearrivals Feedback',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/emails/SearchFieldGroups?Length=x',
        target: 'StatusBar',
        message: 'Searching Field Groups',
        page: 'settings > emails',
        block: ''
    },
    {
        url: '/Emails/GetFieldsPerGroup',
        target: 'StatusBar',
        message: 'Getting Group Fields',
        page: 'settings > emails',
        block: ''
    },
    {
        url: '/Emails/GetFieldGroupInfo',
        target: 'StatusBar',
        message: 'Getting Field Group Info',
        page: 'settings > emails',
        block: ''
    },
    {
        url: '/Emails/GetFieldInfo',
        target: 'StatusBar',
        message: 'Getting Field Info',
        page: 'settings > emails',
        block: ''
    },
    {
        url: '/settings/surveys/GetReferrals?fromDate=x&toDate=x&fieldGroupID=x',
        target: 'StatusBar',
        message: 'Getting Referrals List',
        page: 'settings > survey',
        block: 'btnGetReferrals'
    },
    {
        url: '/crm/Reports/SearchActivityLogs?Length=x',
        target: 'StatusBar',
        message: 'Getting Activity Logs',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchCallsByLocation?Length=x',
        target: 'StatusBar',
        message: 'Getting Report...',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/Notifications/GetNotificationHistory',
        target: 'StatusBar',
        message: 'Getting Notification History',
        page: 'crm > notifications',
        block: ''
    },
    {
        url: '/crm/notifications/SearchForms?Length=x',
        target: 'StatusBar',
        message: 'Searching Notifications',
        page: 'crm > notifications',
        block: ''
    },
    {
        url: '/crm/Reports/GetNotificationsReport',
        target: 'StatusBar',
        message: 'Getting Notifications Report',
        page: 'crm > Reports',
        block: ''
    },
    {
        url: '/crm/hostess/SearchGlobalPenetrationReport?Length=x',
        target: 'StatusBar',
        message: 'Getting Global Penetration',
        page: 'crm > Hostess',
        block: ''
    },
    {
        url: '/Notifications/DeleteNotificationValues',
        target: 'StatusBar',
        message: 'Deleting Transaction Info',
        page: 'crm > notifications',
        block: ''
    },
    {
        url: '/crm/Notifications/SaveFieldValues?Length=x',
        target: 'StatusBar',
        message: 'Sending Notification',
        page: 'crm > notifications',
        block: ''
    },
    {
        url: '/crm/Reports/GetNotificationsReport?Length=x',
        target: 'StatusBar',
        message: 'Getting Notifications Report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/PreArrival/GetInteractions',
        target: 'StatusBar',
        message: 'Getting Interactions',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/PreArrival/SaveLayout',
        target: 'StatusBar',
        message: 'saving layout',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/PreArrival/CopyLayout',
        target: 'StatusBar',
        message: 'copying layout',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/PreArrival/DeleteLayout',
        target: 'StatusBar',
        message: 'Deleting layout',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/crm/prearrival/SearchPreArrival?Length=x',
        target: 'StatusBar',
        message: 'Getting Results',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/MasterChart/GetPromoInfo?promoID=x',
        target: 'StatusBar',
        message: 'Getting Promo Info',
        page: 'crm > masterchart',
        block: 'btnSavePurchase'
    },
    {
        url: '/crm/taskscheduler/SearchTasks?Length=x',
        target: 'StatusBar',
        message: 'Searching Tasks',
        page: 'settings > taskscheduler',
        block: ''
    },
    {
        url: '/PreArrival/GetLead',
        target: 'StatusBar',
        message: 'Getting Lead Info',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/PreArrival/SaveInteractionReply',
        target: 'StatusBar',
        message: 'Saving Reply',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/crm/PreArrival/SaveInteraction?Length=x',
        target: 'StatusBar',
        message: 'Saving Interaction',
        page: 'crm > prearrival',
        block: 'btnSaveInteraction'
    },
    {
        url: '/crm/PreArrival/SaveLead?Length=x',
        target: 'StatusBar',
        message: 'Saving Info',
        page: 'crm > prearrival',
        block: 'btnSaveLead'
    },
    {
        url: '/crm/prearrival/SavePayment?Length=x',
        target: 'StatusBar',
        message: 'Saving Transaction',
        page: 'crm > prearrival',
        block: '.submit'
    },
    {
        url: '/crm/PreArrival/SaveReservation?Length=x',
        target: 'StatusBar',
        message: 'Saving Reservation',
        page: 'crm > prearrival',
        block: '.submit'
    },
    {
        url: '/crm/PreArrival/SavePresentation?Length=x',
        target: 'StatusBar',
        message: 'Saving Presentation',
        page: 'crm > prearrival',
        block: '.submit'
    },
    {
        url: '/crm/PreArrival/SaveFlight?Length=x',
        target: 'StatusBar',
        message: 'Saving Flight',
        page: 'crm > prearrival',
        block: '.submit'
    },
    {
        url: '/crm/PreArrival/SaveBilling?Length=x',
        target: 'StatusBar',
        message: 'Saving Billing',
        page: 'crm > prearrival',
        block: '.submit'
    },
    {
        url: '/PreArrival/ApplyPendingCharges',
        target: 'StatusBar',
        message: 'Applying Charges',
        page: 'crm > prearrival',
        block: 'btnApplyPendingCharges'
    },
    {
        url: '/PreArrival/ApplyCharge',
        target: 'StatusBar',
        message: 'Applying Charge',
        page: 'crm > prearrival',
        block: '.apply-charge'
    },
    {
        url: '/PreArrival/GetReservation',
        target: 'StatusBar',
        message: 'Getting Reservation Info',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/PreArrival/GetFlight',
        target: 'StatusBar',
        message: 'Getting Flight Info',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/PreArrival/GetPayment',
        target: 'StatusBar',
        message: 'Getting Payment Info',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/PreArrival/GetBilling',
        target: 'StatusBar',
        message: 'Getting Billing Info',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/crm/Reports/SearchIncorrectPreBookedTours?Length=x',
        target: 'StatusBar',
        message: 'Getting Results',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/PreArrival/GetOptionSold',
        target: 'StatusBar',
        message: 'Getting Option Info',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/crm/reports/SavePartial',
        target: 'StatusBar',
        message: 'Saving Partial Payment',
        page: 'crm > reports > timeshare operation',
        block: '.save-partial'
    },
    {
        url: '/crm/reports/DeletePartial/x',
        target: 'StatusBar',
        message: 'Deleting Partial Payment',
        page: 'crm > reports > timeshare operation',
        block: '.delete-partial'
    },
    {
        url: '/SPI/GetManifestForAgency',
        target: 'StatusBar',
        message: 'Getting Manifest',
        page: 'crm',
        block: ''
    },
    {
        url: '/crm/Hub/GetGuest',
        target: 'StatusBar',
        message: 'Getting Guest Info',
        page: 'crm > Hub',
        block: ''
    },
    {
        url: '/crm/hub/Search?Length=x',
        target: 'StatusBar',
        message: 'Searching Arrivals...',
        page: 'crm > hub',
        block: ''
    },
    {
        url: '/crm/Reports/SearchSalesByTeam?Length=x',
        target: 'StatusBar',
        message: 'Searching Sales ',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/SPI/GetManifestForVLO',
        target: 'StatusBar',
        message: 'Getting Manifest',
        page: 'crm',
        block: ''
    },
    {
        url: '/Notifications/SendLetter',
        target: 'StatusBar',
        message: 'Sending...',
        page: 'crm',
        block: ''
    },
    {
        url: '/crm/PreArrival/GetEmail',
        target: 'StatusBar',
        message: 'Getting Info',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/crm/PreArrival/GetPhone',
        target: 'StatusBar',
        message: 'Getting Info',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/crm/Catalogs/SearchMarketCodes?Length=x',
        target: 'StatusBar',
        message: 'Searching Market Codes',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/Catalogs/GetMarketCode',
        target: 'StatusBar',
        message: 'Getting Market Code',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/crm/Catalogs/AssignMarketCodes?Length=x',
        target: 'StatusBar',
        message: 'Saving/Assigning Code(s)',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/FollowUp/GetMails?folder=inbox&_=x',
        target: 'StatusBar',
        message: 'Getting Inbox Emails',
        page: 'crm > followup',
        block: 'btnSendAndReceive'
    },
    {
        url: '/FollowUp/GetLog?Date=x&Visible=x&_=x',
        target: 'StatusBar',
        message: 'Getting Log',
        page: 'crm > followup',
        block: 'btnGetLog'
    },
    {
        url: '/crm/Reports/SearchPreArrivalReportingReport?Length=x',
        target: 'StatusBar',
        message: 'Getting Report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/prearrival/SaveOptionSold?Length=x',
        target: 'StatusBar',
        message: 'Saving Option',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/crm/Invitations2/SearchInvitations?Length=x',
        target: 'StatusBar',
        message: 'Getting Invitations',
        page: 'crm > Invitations2',
        block: ''
    },
    {
        url: '/crm/Invitations2/searchGuestToMatch?Length=x',
        target: 'StatusBar',
        message: 'Getting Guests Information',
        page: 'crm > invitations2',
        block: '',
    },
    {
        url: '/crm/PreArrival/GetArrivals?Length=x',
        target: 'StatusBar',
        message: 'Getting Report',
        page: 'crm > reports',
        block: 'btnSearchToImport'
    },
    {
        url: '/PreArrival/PreviewEmail?reservationID=x&emailNotificationID=x&transactionID=x&_=x',
        target: 'StatusBar',
        message: 'Getting Preview',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/crm/Reports2/SearchInvitationBalance?Length=x',
        target: 'StatusBar',
        message: 'Getting Invitations Balance',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/customerExperience/GetArrivals?Length=x',
        target: 'StatusBar',
        message: 'Getting Arrivals',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchPreArrivalManifest?Length=x',
        target: 'StatusBar',
        message: 'Getting Results',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchPreArrivalWeeklyBudget?Length=x',
        target: 'StatusBar',
        message: 'Getting Results',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchPreArrivalSalesByAgent?Length=x',
        target: 'StatusBar',
        message: 'Getting Results',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchIndicatorsPerResort?Length=x',
        target: 'StatusBar',
        message: 'Getting Results',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Users/SearchUsersSubordinates?Length=x',
        target: 'StatusBar',
        message: 'Getting Users',
        page: 'crm > users',
        block: ''
    },
    {
        url: '/crm/Workflows/SearchWorkflows',
        target: 'StatusBar',
        message: 'searching Workflows',
        page: 'crm > workflows',
        block: ''
    },
    {
        url: '/crm/Reports/SearchFrequentGuests?Length=x',
        target: 'StatusBar',
        message: 'Consulting Front Office, please wait',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchInvitationsDeposits?Length=x',
        target: 'StatusBar',
        message: 'Getting Results',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchPreArrivalOptionsPercentageReport?Length=x',
        target: 'StatusBar',
        message: 'Getting Results',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchPreArrivalWeeklyCommissionsReport?Length=x',
        target: 'StatusBar',
        message: 'Getting Results',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchPreArrivalSalesByResort-OptionType?Length=x',
        target: 'StatusBar',
        message: 'Getting Results',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/prearrival/GetArrivalsToImport?Length=x',
        target: 'StatusBar',
        message: 'Getting entries',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/crm/PreArrival/ImportArrivals',
        target: 'StatusBar',
        message: 'Importing entries',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/PreArrival/FillDrpReportLayoutsByUser',
        target: 'StatusBar',
        message: 'getting items',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/crm/PreArrival/GetManifestByDate',
        target: 'StatusBar',
        message: 'getting list',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/crm/PreArrival/GetTourInfo',
        target: 'StatusBar',
        message: 'getting info',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/crm/prearrival/MassUpdate?Length=x',
        target: 'StatusBar',
        message: 'getting info',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/crm/Reports/SearchPreArrivalCommissionsReport?Length=x',
        target: 'StatusBar',
        message: 'getting info',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchTransportations?Length=x',
        target: 'StatusBar',
        message: 'getting info',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchPreArrivalSalesByResortOptionType?Length=x',
        target: 'StatusBar',
        message: 'getting info',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Catalogs/SearchUsersLeadSources?Length=x',
        target: 'StatusBar',
        message: 'getting info',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/Catalogs/GetUserLeadSource',
        target: 'StatusBar',
        message: 'getting info',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/crm/Catalogs/SaveUserLeadSource?Length=x',
        target: 'StatusBar',
        message: 'saving info',
        page: 'settings > catalogs',
        block: ''
    },
    {
        url: '/crm/prearrival/MassSending?Length=x',
        target: 'StatusBar',
        message: 'sending info',
        page: 'crm > prearrival',
        block: ''
    },
    {
        url: '/PreArrival/GetPointsRedemptionRate',
        target: 'StatusBar',
        message: 'getting rates',
        page: 'crm > prearrival',
        block: 'btnSavePayment'
    },
    {
        url: '/crm/Reports/SearchPreArrivalImportHistory?Length=x',
        target: 'StatusBar',
        message: 'getting report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchPreArrivalWeeklyReport?Length=x',
        target: 'StatusBar',
        message: 'getting report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/Reports/SearchPreArrivalInvoices?Length=x',
        target: 'StatusBar',
        message: 'getting report',
        page: 'crm > reports',
        block: ''
    },
    {
        url: '/crm/prearrival/ReservationsToReceive?Length=x',
        target: 'StatusBar',
        message: 'proccesing',
        page: 'crm > prearrival',
        block: ''
    }
]
/*
,
    {
        url: '',
        target: 'StatusBar',
        message: '',
        page: 'crm > reports',
        block: 'element1,elementn' / '.class-name'
    }

 */