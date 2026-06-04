import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './Auth/login/login.component';
import { AuthGuard } from './Auth/auth.guard';
import { HomeLayoutComponent } from './components/layout/home-layout.component';
import { LoginLayoutComponent } from './components/layout/login-layout.component';
import { VendorListComponent } from './components/vendor/vendor-list/vendor-list.component';
import { UserListComponent } from './components/user-management/user/user-list/user-list.component';
import { RoleListComponent } from './components/user-management/role/role-list/role-list.component';
import { CompanyListComponent } from './components/company/company-list/company-list.component';
import { DepartmentListComponent } from './components/department/department-list/department-list.component';
import { StoreListComponent } from './components/store/store-list/store-list.component';
import { UomListComponent } from './components/uom/uom-list/uom-list.component';
import { CategoryListComponent } from './components/category/category-list/category-list.component';
import { SubcategoryListComponent } from './components/subcategory/subcategory-list/subcategory-list.component';
import { ItemtypeListComponent } from './components/itemtype/itemtype-list/itemtype-list.component';
import { ItemListComponent } from './components/item/item-list/item-list.component';
import { LocationListComponent } from './components/location/location-list/location-list.component';
import { ProjectListComponent } from './components/project/project-list/project-list.component';
import { ChartitemsComponent } from './components/chartitems/chartitems.component';
import { IndentrequestTabComponent } from './components/indentrequest/indentrequest-tab/indentrequest-tab.component';
import { IndentTypeListComponent } from './components/indenttype/indenttype-list/indenttype-list.component';
import { PriorityListComponent } from './components/priority/priority-list/priority-list.component';
import { PurchaseDemandTabComponent } from './components/purchasedemand/purchasedemand-tab/purchasedemand-tab.component';
import { PurchaseOrderTabComponent } from './components/purchaseorder/purchaseorder-tab/purchaseorder-tab.component';
import { CurrencyListComponent } from './components/currency/currency-list/currency-list.component';
import { ShipmentModeListComponent } from './components/shipmentmode/shipmentmode-list/shipmentmode-list.component';
import { PaymentModeListComponent } from './components/paymentmode/paymentmode-list/paymentmode-list.component';
import { ComparativeStatementTabComponent } from './components/comparativestatement/comparativestatement-tab/comparativestatement-tab.component';
import { DeliveryTermsListComponent } from './components/deliveryterms/deliveryterms-list/deliveryterms-list.component';
import { GSTListComponent } from './components/gst/gst-list/gst-list.component';
import { IGPTabComponent } from './components/igp/igp-tab/igp-tab.component';
import { AccountCategoryListComponent } from './components/accountcategory/accountcategory-list/accountcategory-list.component';
import { AccountSubcategoryListComponent } from './components/accountsubcategory/accountsubcategory-list/accountsubcategory-list.component';
import { AccountTypeListComponent } from './components/accounttype/accounttype-list/accounttype-list.component';
import { AccountListComponent } from './components/account/account-list/account-list.component';
import { AccountChartComponent } from './components/accountchart/accountchart.component';
import { TransactionTabComponent } from './components/transaction/transaction-tab/transaction-tab.component';
import { BrvTabComponent } from './components/brv/brv-tab/brv-tab.component';
import { BpvTabComponent } from './components/bpv/bpv-tab/bpv-tab.component';
import { CrvTabComponent } from './components/crv/crv-tab/crv-tab.component';
import { CpvTabComponent } from './components/cpv/cpv-tab/cpv-tab.component';
import { DealershipListComponent } from './components/order/dealership/dealership-list/dealership-list.component';
import { RouteListComponent } from './components/order/route/route-list/route-list.component';
import { ShopListComponent } from './components/order/shop/shop-list/shop-list.component';
import { TerritoryListComponent } from './components/order/territory/territory-list/territory-list.component';
import { ZoneListComponent } from './components/order/zone/zone-list/zone-list.component';
import { RegionListComponent } from './components/order/region/region-list/region-list.component';
import { AreaListComponent } from './components/order/area/area-list/area-list.component';
import { ShopTypeListComponent } from './components/order/shop-type/shop-type-list/shop-type-list.component';
import { DSFListComponent } from './components/order/DSF/DSF-list/DSF-list.component';
import { PricingGroupListComponent } from './components/order/pricing-group/pricing-group-list/pricing-group-list.component';
import { InspectionTabComponent } from './components/inspection/inspection-tab/inspection-tab.component';
import { RejectReasonListComponent } from './components/rejectreason/rejectreason-list/rejectreason-list.component';
import { VehicleListComponent } from './components/order/vehicle/vehicle-list/vehicle-list.component';
import { OrderListComponent } from './components/order/primary-order/order-list/order-list.component';
import { PredFieldMapComponent } from './components/order/zone/pred-field-map/pred-field-map.component';
import { UserAttendanceListComponent } from './components/order/user-attendance/user-attendance-list/user-attendance-list.component';
import { UserTerritoryListComponent } from './components/order/user-territory/user-territory-list/user-territory-list.component';
import { GRNTabComponent } from './components/grn/grn-tab/grn-tab.component';
import { AccountFlowListComponent } from './components/accountflow/accountflow-list/accountflow-list.component';
import { RackListComponent } from './components/rack/rack-list/rack-list.component';
import { RowListComponent } from './components/row/row-list/row-list.component';
import { SectionListComponent } from './components/section/section-list/section-list.component';
import { AuditReviewTabComponent } from './components/auditreview/auditreview-tab/auditreview-tab.component';
import { AccountGroupListComponent } from './components/accountgroup/accountgroup-list/accountgroup-list.component';
import { SalesTargetListComponent } from './components/order/sales-target/sales-target-list/sales-target-list.component';
import { IssuanceTabComponent } from './components/issuance/issuance-tab/issuance-tab.component';
import { DispatchTabComponent } from './components/dispatch/dispatch-tab/dispatch-tab.component';
import { SaleUsersListComponent } from './components/order/sale-users/sale-users-list/sale-users-list.component';
import { SJVTabComponent } from './components/sjv/sjv-tab/sjv-tab.component';
import { ReportViewerComponent } from './components/report/report-viewer.component';
import { CostsheetTabComponent } from './components/costsheet/costsheet-tab/costsheet-tab.component';
import { CancelDispatchTabComponent } from './components/canceldispatch/cancel-dispatch-tab/cancel-dispatch-tab.component';
import { PJVTabComponent } from './components/pjv/pjv-tab/pjv-tab.component';
import { SaleMaterialTabComponent } from './components/salematerial/salematerial-tab/salematerial-tab.component';
import { CustomerListComponent } from './components/customer/customer-list/customer-list.component';
import { PurchaseInvoiceTabComponent } from './components/purchaseinvoice/purchaseinvoice-tab/purchaseinvoice-tab.component';
import { EmployeeTypeListComponent } from './components/hr/employee-type/employee-type-list/employee-type-list.component';
import { EmployeeShiftListComponent } from './components/hr/employee-shift/employee-shift-list/employee-shift-list.component';
import { EmployeeGradeListComponent } from './components/hr/employee-grade/employee-grade-list/employee-grade-list.component';
import { EmployeeEducationListComponent } from './components/hr/employee-education/employee-education-list/employee-education-list.component';
import { EmployeeDesignationListComponent } from './components/hr/employee-designation/employee-designation-list/employee-designation-list.component';
import { EmployeeListComponent } from './components/hr/employee/employee-list/employee-list.component';
import { EmployeeBankListComponent } from './components/hr/employee-bank/employee-bank-list/employee-bank-list.component';
import { EmployeeLeaveTypeListComponent } from './components/hr/employee-leave-type/employee-leave-type-list/employee-leave-type-list.component';
import { EmployeeLeaveGroupListComponent } from './components/hr/employee-leave-group/employee-leave-group-list/employee-leave-group-list.component';
import { EmployeeDocumentTypeListComponent } from './components/hr/employee-document-type/employee-document-type-list/employee-document-type-list.component';
import { CityListComponent } from './components/hr/city/city-list/city-list.component';
import { IJVTabComponent } from './components/ijv/ijv-tab/ijv-tab.component';
import { DeviceListComponent } from './components/device/device-list/device-list.component';
import { SaleReturnTabComponent } from './components/salereturn/salereturn-tab/salereturn-tab.component';
import { SRJVTabComponent } from './components/srjv/srjv-tab/srjv-tab.component';
import { EmployeeOvertimeRateListComponent } from './components/hr/employee-overtimerate/employee-overtimerate-list/employee-overtimerate-list.component';
import { ShopOrderReturnTabComponent } from './components/shoporderreturn/shoporderreturn-tab/shoporderreturn-tab.component';
import { PurchaseReturnTabComponent } from './components/purchasereturn/purchasereturn-tab/purchasereturn-tab.component';
import { WarehouseTransferTabComponent } from './components/warehousetransfer/warehousetransfer-tab/warehousetransfer-tab.component';
import { SaleMaterialReturnTabComponent } from './components/salematerialreturn/salematerialreturn-tab/salematerialreturn-tab.component';
import { HRYearListComponent } from './components/hr/hryear/hryear-list/hryear-list.component';
import { EmployeeLeaveListComponent } from './components/hr/employee-leave/employee-leave-list/employee-leave-list.component';
import { ManageEmployeeLeaveListComponent } from './components/hr/manage-employee-leave/manage-employee-leave-list/manage-employee-leave-list.component';
import { ApproveEmployeeLeaveListComponent } from './components/hr/approve-employee-leave/approve-employee-leave-list/approve-employee-leave-list.component';
import { HrDashboardComponent } from './components/hr/dashboards/hr-dashboard/hr-dashboard.component';
import { EmployeeDashboardComponent } from './components/hr/dashboards/employee-dashboard/employee-dashboard.component';
import { ManagerDashboardComponent } from './components/hr/dashboards/manager-dashboard/manager-dashboard.component';
import { EmployeeWorkSiteTypeListComponent } from './components/hr/employee-worksitetype/employee-worksitetype-list/employee-worksitetype-list.component';
import { InterviewListComponent } from './components/interview/interview-list/interview-list.component';
import { RetailOrderListComponent } from './components/order/retail-orders/retail-order-list/retail-order-list.component';
import { HolidayListComponent } from './components/hr/holiday/holiday-list/holiday-list.component';
import { RetailOrderReturnTabComponent } from './components/order/retail-orders/retail-order-return/retail-order-return-tab/retail-order-return-tab.component';
import { AddAppointmentComponent } from './components/opd/appointment/add-appointment/add-appointment.component';
import { AppointmentListComponent } from './components/opd/appointment/appointment-list/appointment-list.component';
import { AppointmentTypeListComponent } from './components/opd/appointment-type/appointment-type-list/appointment-type-list.component';
import { PatientListComponent } from './components/opd/patient/patient-list/patient-list.component';
import { DoctorListComponent } from './components/opd/doctor/doctor-list/doctor-list.component';
import { DoctorAppointmentListComponent } from './components/opd/appointment/doctor-appointment-list/doctor-appointment-list.component';
import { TriageListComponent } from './components/opd/triage/triage-list/triage-list.component';
import { CreateTriageComponent } from './components/opd/triage/create-triage/create-triage.component';
import { AddRosterComponent } from './components/hr/roster/add-roster/add-roster.component';
import { RosterTabComponent } from './components/hr/roster/roster-tab/roster-tab.component';
import { RosterDepartmentListComponent } from './components/hr/roster-department/roster-list-department/roster-department-list.component';
import { AddRosterDepartmentComponent } from './components/hr/roster-department/add-roster-department/add-roster-department.component';
import { CandidateEvaluationCategoryListComponent } from './components/hr/candidateevaluationcategory/candidateevaluationcategory-list/candidateevaluationcategory-list.component';
import { ConductInterviewListComponent } from './components/interview/conduct-interview-list/conduct-interview-list.component';
import { NotificationListComponent } from './components/hr/notification/notification-list/notification-list.component';
import { AddNotificationComponent } from './components/hr/notification/add-notification/add-notification.component';
import { SalaryHeadListComponent } from './components/hr/payroll/salaryhead/salaryhead-list/salaryhead-list.component';
import { SalaryTaxSlabListComponent } from './components/hr/payroll/salarytaxslab/salarytaxslab-list/salarytaxslab-list.component';
import { TriageCategoryListComponent } from './components/opd/triage-category/triage-category-list/triage-category-list.component';
import { LabOrderListComponent } from './components/opd/lab-order/lab-order-list/lab-order-list.component';
import { AddLabOrderComponent } from './components/opd/lab-order/add-lab-order/add-lab-order.component';
import { RadiologyTypeListComponent } from './components/opd/radiologytype/radiologytype-list/radiologytype-list.component';
import { ServiceListComponent } from './components/opd/service/service-list/service-list.component';
import { LabOrderTypeListComponent } from './components/opd/lab-order-type/lab-order-type-list/lab-order-type-list.component';
import { ServiceTypeListComponent } from './components/opd/service-type/service-type-list/service-type-list.component';
import { ReferrerListComponent } from './components/opd/referrer/referrer-list/referrer-list.component';
import { BookAppointmentListComponent } from './components/opd/appointment/book-appointment-list/book-appointment-list.component';

const routes: Routes = [
  {
    path: '', component: HomeLayoutComponent, canActivate: [AuthGuard],
    children: [
      { path: '', component: HomeComponent, canActivate: [AuthGuard], data: { roles: ["hr manager,hr executive,Admin,store manager,store issuer", "purchase manager,purchaser,accounts manager,accounts assistant,retailer,gate clerk,inspection,manager,assistant,sales,distributor", "audit", "sse", "ssm", "mto"] } },
      { path: 'home', component: HomeComponent, canActivate: [AuthGuard], data: { roles: ["hr manager,hr executive,Admin,store manager,store issuer", "purchase manager,purchaser,accounts manager,accounts assistant,retailer,gate clerk,inspection,manager,assistant,sales,distributor", "audit", "sse", "ssm", "mto"] } },
      { path: 'rack', component: RackListComponent, canActivate: [AuthGuard], data: { roles: ["Admin,store manager,store issuer", "purchase manager,purchaser,accounts manager,accounts assistant,retailer,gate clerk,inspection"] } },
      { path: 'row', component: RowListComponent, canActivate: [AuthGuard], data: { roles: ["Admin,store manager,store issuer", "purchase manager,purchaser,accounts manager,accounts assistant,retailer,gate clerk,inspection"] } },
      { path: 'section', component: SectionListComponent, canActivate: [AuthGuard], data: { roles: ["Admin,store manager,store issuer", "purchase manager,purchaser,accounts manager,accounts assistant,retailer,gate clerk,inspection"] } },
      { path: 'roles', component: RoleListComponent, canActivate: [AuthGuard], data: { roles: ["hr manager,Admin"] } },
      { path: 'users', component: UserListComponent, canActivate: [AuthGuard], data: { roles: ["Admin"] } },
      { path: 'companies', component: CompanyListComponent, canActivate: [AuthGuard], data: { roles: ["Admin"] } },
      { path: 'departments', component: DepartmentListComponent, canActivate: [AuthGuard], data: { roles: ["Admin,hr manager,hr executive"] } },
      { path: 'store', component: StoreListComponent, canActivate: [AuthGuard], data: { roles: ["Admin"] } },
      { path: 'vendors', component: VendorListComponent, canActivate: [AuthGuard], data: { roles: ["purchase manager,purchaser"] } },
      { path: 'uoms', component: UomListComponent, canActivate: [AuthGuard], data: { roles: ["store manager,store issuer"] } },
      { path: 'indenttype', component: IndentTypeListComponent, canActivate: [AuthGuard], data: { roles: ["store manager,store issuer"] } },
      { path: 'priority', component: PriorityListComponent, canActivate: [AuthGuard], data: { roles: ["store manager,store issuer"] } },
      { path: 'category', component: CategoryListComponent, canActivate: [AuthGuard], data: { roles: ["store manager,store issuer"] } },
      { path: 'subcategory', component: SubcategoryListComponent, canActivate: [AuthGuard], data: { roles: ["store manager,store issuer"] } },
      { path: 'itemtype', component: ItemtypeListComponent, canActivate: [AuthGuard], data: { roles: ["store manager,store issuer"] } },
      { path: 'items', component: ItemListComponent, canActivate: [AuthGuard], data: { roles: ["store manager,store issuer"] } },
      { path: 'location', component: LocationListComponent, canActivate: [AuthGuard], data: { roles: ["admin"] } },
      { path: 'project', component: ProjectListComponent, canActivate: [AuthGuard], data: { roles: ["Admin,hr manager,hr executive"] } },
      { path: 'indentrequest', component: IndentrequestTabComponent, canActivate: [AuthGuard], data: {  breadcrumb : 'Issue Request', roles: ["manager, assistant"] } },
      { path: 'chartitems', component: ChartitemsComponent, canActivate: [AuthGuard], data: { roles: ["store manager,store issuer"] } },
      { path: 'purchasedemand', component: PurchaseDemandTabComponent, canActivate: [AuthGuard], data: { roles: ["store manager,store issuer,audit"] } },
      { path: 'grn', component: GRNTabComponent, canActivate: [AuthGuard], data: { roles: ["store manager,store issuer,audit"] } },
      { path: 'purchaseorder', component: PurchaseOrderTabComponent, canActivate: [AuthGuard], data: { roles: ["purchase manager,purchaser,audit"] } },
      { path: 'currency', component: CurrencyListComponent, canActivate: [AuthGuard], data: { roles: ["purchase manager,purchaser"] } },
      { path: 'shipmentmode', component: ShipmentModeListComponent, canActivate: [AuthGuard], data: { roles: ["purchase manager,purchaser"] } },
      { path: 'paymentmode', component: PaymentModeListComponent, canActivate: [AuthGuard], data: { roles: ["purchase manager,purchaser"] } },
      { path: 'comparativestatement', component: ComparativeStatementTabComponent, canActivate: [AuthGuard], data: { roles: ["purchase manager,purchaser"] } },
      { path: 'deliveryterms', component: DeliveryTermsListComponent, canActivate: [AuthGuard], data: { roles: ["purchase manager,purchaser"] } },
      { path: 'gst', component: GSTListComponent, canActivate: [AuthGuard], data: { roles: ["purchase manager,purchaser"] } },
      { path: 'igp', component: IGPTabComponent, canActivate: [AuthGuard], data: { roles: ["gate clerk,audit"] } },
      { path: 'inspection', component: InspectionTabComponent, canActivate: [AuthGuard], data: { roles: ["inspection", 'audit'] } },
      { path: 'rejectreason', component: RejectReasonListComponent, canActivate: [AuthGuard], data: { roles: ["inspection"] } },
      { path: 'accountcategory', component: AccountCategoryListComponent, canActivate: [AuthGuard], data: {  breadcrumb : 'Account Category' , roles: ["accounts manager,accounts assistant","admin"] } },
      { path: 'accountsubcategory', component: AccountSubcategoryListComponent, canActivate: [AuthGuard], data: { breadcrumb : 'Account Sub Category' , roles: ["accounts manager,accounts assistant","admin"] } },
      { path: 'accounttype', component: AccountTypeListComponent, canActivate: [AuthGuard], data: { breadcrumb : 'Account Type' , roles: ["accounts manager,accounts assistant","admin"] } },
      { path: 'account', component: AccountListComponent, canActivate: [AuthGuard], data: {breadcrumb : 'Account' , roles: ["accounts manager,accounts assistant,audit","admin"] } },
      { path: 'accountgroup', component: AccountGroupListComponent, canActivate: [AuthGuard], data: { breadcrumb : 'Account Group' , roles: ["accounts manager,accounts assistant,audit","admin"] } },
      { path: 'accountchart', component: AccountChartComponent, canActivate: [AuthGuard], data: { breadcrumb : 'Account Chart' , roles: ["accounts manager,accounts assistant","admin"] } },
      { path: 'jv', component: TransactionTabComponent, canActivate: [AuthGuard], data: {breadcrumb : 'Journal Voucher' ,  roles: ["accounts manager,accounts assistant", "audit","admin"] } },
      { path: 'brv', component: BrvTabComponent, canActivate: [AuthGuard], data: {breadcrumb : 'Bank Receipt Voucher' , roles: ["accounts manager,accounts assistant", "audit","admin"] } },
      { path: 'bpv', component: BpvTabComponent, canActivate: [AuthGuard], data: { breadcrumb : 'Bank Payment Voucher' , roles: ["accounts manager,accounts assistant", "audit","admin"] } },
      { path: 'crv', component: CrvTabComponent, canActivate: [AuthGuard], data: { breadcrumb : 'Cash Receipt Voucher', roles: ["accounts manager,accounts assistant", "audit","admin"] } },
      { path: 'cpv', component: CpvTabComponent, canActivate: [AuthGuard], data: { breadcrumb : 'Cash Payment Voucher' ,roles: ["accounts manager,accounts assistant", "audit","admin"] } },
      { path: 'accountflow', component: AccountFlowListComponent, canActivate: [AuthGuard], data: { breadcrumb: 'Account Flow', roles: ["accounts manager,accounts assistant","admin"] } },
      { path: 'accountreview', component: AuditReviewTabComponent, canActivate: [AuthGuard], data: { breadcrumb : 'Account Review' , roles: ["accounts manager,accounts assistant,audit","admin"] } },
      { path: 'region', component: RegionListComponent, canActivate: [AuthGuard], data: { roles: ["admin,sales,audit,sse,ssm,mto,eds"] } },
      { path: 'area', component: AreaListComponent, canActivate: [AuthGuard], data: { roles: ["admin,sales,audit,sse,ssm,mto,eds"] } },
      { path: 'zone', component: ZoneListComponent, canActivate: [AuthGuard], data: { roles: ["admin,sales,audit,sse,ssm,mto,eds"] } },
      { path: 'territory', component: TerritoryListComponent, canActivate: [AuthGuard], data: { roles: ["admin,sales,audit,sse,ssm,mto,eds"] } },
      { path: 'distributor', component: DealershipListComponent, canActivate: [AuthGuard], data: { roles: ["admin,sales,audit,sse,ssm,mto,eds"] } },
      { path: 'shop', component: ShopListComponent, canActivate: [AuthGuard], data: { roles: ["admin,sales,audit,sse,ssm,mto,eds,audit"] } },
      { path: 'shoptype', component: ShopTypeListComponent, canActivate: [AuthGuard], data: { roles: ["admin,sales,audit,sse,ssm,mto,eds"] } },
      { path: 'route', component: RouteListComponent, canActivate: [AuthGuard], data: { roles: ["admin,sales,audit,sse,ssm,mto,eds"] } },
      { path: 'dsf', component: DSFListComponent, canActivate: [AuthGuard], data: { roles: ["admin,sales,audit"] } },
      { path: 'pricinggroup', component: PricingGroupListComponent, canActivate: [AuthGuard], data: { roles: ["admin,sales,audit"] } },
      { path: 'vehicle', component: VehicleListComponent, canActivate: [AuthGuard], data: { roles: ["store manager,store issuer"] } },
      { path: 'fieldmap', component: PredFieldMapComponent, canActivate: [AuthGuard], data: { roles: ["admin,sales,audit,sse,ssm,mto,eds"] } },
      { path: 'userattendance', component: UserAttendanceListComponent, canActivate: [AuthGuard], data: { roles: ["admin,sales,audit,sse,ssm,mto,eds"] } },
      { path: 'userterritory', component: UserTerritoryListComponent, canActivate: [AuthGuard], data: { roles: ["admin,sales,audit,sse,ssm,mto,eds"] } },
      { path: 'primarysales', component: OrderListComponent, canActivate: [AuthGuard], data: { roles: ["admin,sales,audit,sse,ssm,mto,eds"] } },
      { path: 'retailorders', component: RetailOrderListComponent, canActivate: [AuthGuard], data: { roles: ["retailer,admin,sales,audit,sse,ssm,mto,eds"] }},
      { path: 'saleuser', component: SaleUsersListComponent, canActivate: [AuthGuard], data: { roles: ["admin,sales,audit,sse,ssm,mto,eds"] } },
      { path: 'salestarget', component: SalesTargetListComponent, canActivate: [AuthGuard] },
      { path: 'issuance', component: IssuanceTabComponent, canActivate: [AuthGuard] },
      { path: 'dispatch', component: DispatchTabComponent, canActivate: [AuthGuard], data: { roles: ["store manager,store issuer,audit"] } },
      { path: 'sjv', component: SJVTabComponent, canActivate: [AuthGuard], data: { breadcrumb : 'Sale Journal Voucher' ,roles: ["accounts manager,accounts assistant", "audit","admin"] } },
      { path: 'pjv', component: PJVTabComponent, canActivate: [AuthGuard], data: { breadcrumb : 'Purchase Journal Voucher' ,roles: ["accounts manager,accounts assistant", "audit","admin"] } },
      { path: 'report', component: ReportViewerComponent, canActivate: [AuthGuard], data: { roles: ["accounts manager,accounts assistant", "audit","admin"] } },
      { path: 'canceldispatch', component: CancelDispatchTabComponent, canActivate: [AuthGuard], data: { roles: ["store manager,store issuer", "sales", "accounts assistant", "accounts manager","admin"] } },
      { path: 'accountledgerRpt', component: ReportViewerComponent, canActivate: [AuthGuard], data: { roles: ["accounts manager,accounts assistant","admin"] } },
      { path: 'costsheet', component: CostsheetTabComponent, canActivate: [AuthGuard], data: { roles: ["store manager,store issuer","audit","admin"] } },
      { path: 'salematerial', component: SaleMaterialTabComponent, canActivate: [AuthGuard], data: { roles: ["store manager,store issuer","admin"] } },
      { path: 'customer', component: CustomerListComponent, canActivate: [AuthGuard], data: { roles: ["store manager,store issuer","admin"] } },
      { path: 'purchaseinvoice', component: PurchaseInvoiceTabComponent, canActivate: [AuthGuard], data: { roles: ["accounts manager,accounts assistant", "audit","admin"] } },
      { path: 'employeedesignation', component: EmployeeDesignationListComponent, canActivate: [AuthGuard], data: { roles: ["hr manager,hr executive,admin","admin"] } },
      { path: 'employeeeducation', component: EmployeeEducationListComponent, canActivate: [AuthGuard], data: { roles: ["hr manager,hr executive,admin"] } },
      { path: 'employeegrade', component: EmployeeGradeListComponent, canActivate: [AuthGuard], data: { roles: ["hr manager,hr executive,admin"] } },
      { path: 'employeeshift', component: EmployeeShiftListComponent, canActivate: [AuthGuard], data: { roles: ["hr manager,hr executive,admin"] } },
      { path: 'employeetype', component: EmployeeTypeListComponent, canActivate: [AuthGuard], data: { roles: ["hr manager,hr executive,admin"] } },
      { path: 'employee', component: EmployeeListComponent, canActivate: [AuthGuard], data: { roles: ["hr manager,hr executive,admin"] } },
      { path: 'employeebank', component: EmployeeBankListComponent, canActivate: [AuthGuard], data: { roles: ["hr manager,hr executive,admin"] } },
      { path: 'employeeleavetype', component: EmployeeLeaveTypeListComponent, canActivate: [AuthGuard], data: { roles: ["hr manager,hr executive,admin"] } },
      { path: 'employeeleavegroup', component: EmployeeLeaveGroupListComponent, canActivate: [AuthGuard], data: { roles: ["hr manager,hr executive,admin"] } },
      { path: 'employeedocumenttype', component: EmployeeDocumentTypeListComponent, canActivate: [AuthGuard], data: { roles: ["hr manager,hr executive,admin"] } },
      { path: 'cities', component: CityListComponent, canActivate: [AuthGuard], data: { roles: ["hr manager,hr executive,admin"] } },
      { path: 'ijv', component: IJVTabComponent, canActivate: [AuthGuard], data: {breadcrumb : 'Invoice Journal Voucher' , roles: ["accounts manager,accounts assistant", "audit","admin"] } },
      { path: 'device', component: DeviceListComponent, canActivate: [AuthGuard], data: { roles: ["hr manager,hr executive,admin"] } },
      { path: 'salereturn', component: SaleReturnTabComponent, canActivate: [AuthGuard], data: { roles: ["store manager,store issuer"] } },
      { path: 'srjv', component: SRJVTabComponent, canActivate: [AuthGuard], data: { roles: ["accounts manager,accounts assistant", "audit","admin"] } },
      { path: 'employeeovertimerate', component: EmployeeOvertimeRateListComponent, canActivate: [AuthGuard], data: { roles: ["hr manager,hr executive,admin"] } },
      // { path: 'shoporderreturn', component: ShopOrderReturnTabComponent, canActivate: [AuthGuard], data: { roles: ["retailer,admin"] } },
      { path: 'purchasereturn', component: PurchaseReturnTabComponent, canActivate: [AuthGuard], data: { roles: ["admin,store manager,store issuer,audit"] } },
      { path: 'warehousetransfer', component: WarehouseTransferTabComponent, canActivate: [AuthGuard], data: { roles: ["store manager,store issuer"] } },
      { path: 'salematerialreturn', component: SaleMaterialReturnTabComponent, canActivate: [AuthGuard], data: { roles: ["store manager,store issuer"] } },
      { path: 'hryear', component: HRYearListComponent, canActivate: [AuthGuard], data: {roles: ["hr manager,hr executive,admin"] } },
      { path: 'holiday', component: HolidayListComponent, canActivate: [AuthGuard], data: {roles: ["hr manager,hr executive,admin"] } },
      { path: 'notification', component: NotificationListComponent, canActivate: [AuthGuard], data: {roles: ["hr manager,hr executive,admin"] } },
      { path: 'employeeleave', component: EmployeeLeaveListComponent, canActivate: [AuthGuard] },
      { path: 'manageemployeeleave', component: ManageEmployeeLeaveListComponent, canActivate: [AuthGuard], data: {roles: ["hr manager,hr executive,admin"] } },
      { path: 'approveemployeeleave', component: ApproveEmployeeLeaveListComponent, canActivate: [AuthGuard], data: {roles: ["manager"] } },
      { path: 'hrdashboard', component: HrDashboardComponent, canActivate: [AuthGuard], data: {roles: ["hr manager,hr executive,admin"] } },
      { path: 'employeedashboard', component: EmployeeDashboardComponent, canActivate: [AuthGuard] },
      { path: 'managerdashboard', component: ManagerDashboardComponent, canActivate: [AuthGuard], data: {roles: ["manager","admin"] } },
      { path: 'employeeworksitetype', component: EmployeeWorkSiteTypeListComponent, canActivate: [AuthGuard], data: { roles: ["hr manager,hr executive,admin"] } },
      { path: 'interview', component: InterviewListComponent, canActivate: [AuthGuard] },
      { path: 'retailorderreturn', component: RetailOrderReturnTabComponent, canActivate: [AuthGuard], data: { roles: ["retailer,admin"] } },
      { path: 'appointment', component: AppointmentListComponent, canActivate: [AuthGuard] },
      { path: 'newappointment', component: AddAppointmentComponent, canActivate: [AuthGuard] },
      { path: 'bookappointment', component: BookAppointmentListComponent, canActivate: [AuthGuard] },
      { path: 'booknewappointment', component: AddAppointmentComponent, canActivate: [AuthGuard] },
      { path: 'appointmenttype', component: AppointmentTypeListComponent, canActivate: [AuthGuard] },
      { path: 'patient', component: PatientListComponent, canActivate: [AuthGuard] },
      { path: 'doctor', component: DoctorListComponent, canActivate: [AuthGuard] },
      { path: 'doctorappointment', component: DoctorAppointmentListComponent, canActivate: [AuthGuard] },
      { path: 'triagecategory', component: TriageCategoryListComponent, canActivate: [AuthGuard], data: {roles: ["receptionist"] } },
       { path: 'triage', component: CreateTriageComponent, canActivate: [AuthGuard] },
       { path: 'laborder', component: LabOrderListComponent, canActivate: [AuthGuard] },
       { path: 'newlaborder', component: AddLabOrderComponent, canActivate: [AuthGuard] },
       { path: 'radiologytype', component: RadiologyTypeListComponent, canActivate: [AuthGuard] },
       { path: 'labordertype', component: LabOrderTypeListComponent, canActivate: [AuthGuard] },
       { path: 'newtriage', component: CreateTriageComponent, canActivate: [AuthGuard] },
      { path: 'adddepartmentroster', component: AddRosterDepartmentComponent, canActivate: [AuthGuard] },
      { path: 'rosterdepartment', component: RosterDepartmentListComponent, canActivate: [AuthGuard] },
      { path: 'addroster', component: AddRosterComponent, canActivate: [AuthGuard] },
      { path: 'roster', component: RosterTabComponent, canActivate: [AuthGuard] },
      { path: 'candidateevaluationcategory', component: CandidateEvaluationCategoryListComponent, canActivate: [AuthGuard] },
      { path: 'conductinterview', component: ConductInterviewListComponent, canActivate: [AuthGuard] },
      { path: 'salaryhead', component: SalaryHeadListComponent, canActivate: [AuthGuard], data: {roles: ["hr manager,hr executive,admin"] } },
      { path: 'salarytaxslab', component: SalaryTaxSlabListComponent, canActivate: [AuthGuard], data: {roles: ["hr manager,hr executive,admin"] } },
      { path: 'services', component: ServiceListComponent, canActivate: [AuthGuard] },
      { path: 'servicetype', component: ServiceTypeListComponent, canActivate: [AuthGuard] },
      { path: 'referrer', component: ReferrerListComponent, canActivate: [AuthGuard], data: {roles: ["receptionist"] } },
    ]
  },
  {
    path: '', component: LoginLayoutComponent,
    children: [
      {
        path: 'login', component: LoginComponent
      }
    ]
  },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})

export class AppRoutingModule { }
