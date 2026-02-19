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

const routes: Routes = [
  {
    path: '', component: HomeLayoutComponent, canActivate: [AuthGuard],
    children: [
      { path: '', component: HomeComponent, canActivate: [AuthGuard], data: { roles: ["Admin,store manager,store issuer","purchase manager,purchaser,accounts manager,accounts assistant"] } },
      { path: 'home', component: HomeComponent, canActivate: [AuthGuard], data: { roles: ["Admin,store manager,store issuer","purchase manager,purchaser,accounts manager,accounts assistant"] } },
      { path: 'roles', component: RoleListComponent, canActivate: [AuthGuard], data: { roles: ["Admin"] } },
      { path: 'users', component: UserListComponent, canActivate: [AuthGuard], data: { roles: ["Admin"] } },
      { path: 'companies', component: CompanyListComponent, canActivate: [AuthGuard], data: { roles: ["Admin"] } },
      { path: 'departments', component: DepartmentListComponent, canActivate: [AuthGuard], data: { roles: ["Admin,accounts manager,accounts assistant"] } },
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
      { path: 'project', component: ProjectListComponent, canActivate: [AuthGuard], data: { roles: ["admin,accounts manager,accounts assistant"] } },
      { path: 'indentrequest', component: IndentrequestTabComponent, canActivate: [AuthGuard], data: { roles: ["manager, assistant"] } },
      { path: 'chartitems', component: ChartitemsComponent, canActivate: [AuthGuard], data: { roles: ["store manager,store issuer"] } },
      { path: 'purchasedemand', component: PurchaseDemandTabComponent, canActivate: [AuthGuard], data: { roles: ["store manager,store issuer"] } },
      { path: 'purchaseorder', component: PurchaseOrderTabComponent, canActivate: [AuthGuard], data: { roles: ["purchase manager,purchaser"] } },
      { path: 'currency', component: CurrencyListComponent, canActivate: [AuthGuard], data: { roles: ["purchase manager,purchaser"] } },
      { path: 'shipmentmode', component: ShipmentModeListComponent, canActivate: [AuthGuard], data: { roles: ["purchase manager,purchaser"] } },
      { path: 'paymentmode', component: PaymentModeListComponent, canActivate: [AuthGuard], data: { roles: ["purchase manager,purchaser"] } },
      { path: 'comparativestatement', component: ComparativeStatementTabComponent, canActivate: [AuthGuard], data: { roles: ["purchase manager,purchaser"] } },
      { path: 'deliveryterms', component: DeliveryTermsListComponent, canActivate: [AuthGuard], data: { roles: ["purchase manager,purchaser"] } },
      { path: 'gst', component: GSTListComponent, canActivate: [AuthGuard], data: { roles: ["purchase manager,purchaser"] } },
      { path: 'igp', component: IGPTabComponent, canActivate: [AuthGuard], data: { roles: ["purchase manager,purchaser"] } },
      { path: 'accountcategory', component: AccountCategoryListComponent, canActivate: [AuthGuard], data: { roles: ["accounts manager,accounts assistant"] } },
      { path: 'accountsubcategory', component: AccountSubcategoryListComponent, canActivate: [AuthGuard], data: { roles: ["accounts manager,accounts assistant"] } },
      { path: 'accounttype', component: AccountTypeListComponent, canActivate: [AuthGuard], data: { roles: ["accounts manager,accounts assistant"] } },
      { path: 'account', component: AccountListComponent, canActivate: [AuthGuard], data: { roles: ["accounts manager,accounts assistant"] } },
      { path: 'accountchart', component: AccountChartComponent, canActivate: [AuthGuard], data: { roles: ["accounts manager,accounts assistant"] } },
      { path: 'jv', component: TransactionTabComponent, canActivate: [AuthGuard], data: { roles: ["accounts manager,accounts assistant"] } },
      { path: 'brv', component: BrvTabComponent, canActivate: [AuthGuard], data: { roles: ["accounts manager,accounts assistant"] } },
      { path: 'bpv', component: BpvTabComponent, canActivate: [AuthGuard], data: { roles: ["accounts manager,accounts assistant"] } },
      { path: 'crv', component: CrvTabComponent, canActivate: [AuthGuard], data: { roles: ["accounts manager,accounts assistant"] } },
      { path: 'cpv', component: CpvTabComponent, canActivate: [AuthGuard], data: { roles: ["accounts manager,accounts assistant"] } },
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
