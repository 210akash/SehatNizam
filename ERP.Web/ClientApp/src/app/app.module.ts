import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NgbDatepickerModule, NgbTypeaheadModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule, JsonPipe } from '@angular/common';
import { AuthenticationService } from './Auth/authentication.service';
import { JWT_OPTIONS, JwtHelperService } from '@auth0/angular-jwt';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { AuthInterceptor } from './Auth/auth.interceptor';
import { AuthEndPoints } from './Auth/auth.endpoints';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormField, MatFormFieldDefaultOptions, MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { HomeLayoutComponent } from './components/layout/home-layout.component';
import { LoginLayoutComponent } from './components/layout/login-layout.component';
import { MatCardModule } from '@angular/material/card';
import { SidemenuComponent } from './components/sidemenu/sidemenu.component';
import { LoginComponent } from './Auth/login/login.component';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatDialogModule } from '@angular/material/dialog';
import { LoaderService } from './Service/loader.service';
import { MyLoaderComponent } from './components/Shared/my-loader/my-loader.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { LoaderInterceptor } from './Service/loader-interceptor.service';
import { NotificationsService } from './Service/notification.service';
import { ConstantService } from './Service/constant.service';
import { GeneralService } from './Service/general.service';
import { GeneralEndPoints } from './Service/general.endpoints';
import { ControllerEndpoints } from './components/Shared/ControllerEndpoints';
import { MatSort } from '@angular/material/sort';
import { MatSortModule } from '@angular/material/sort';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSelectModule } from '@angular/material/select';
import { MatMenuModule } from '@angular/material/menu';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { provideNativeDateAdapter } from '@angular/material/core';
import { InputMaskModule } from '@ngneat/input-mask';
import { MediaService } from './Service/media.service';
import { MatListModule } from '@angular/material/list';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { VendorService } from './components/vendor/vendor.service';
import { VendorEndPoints } from './components/vendor/vendor.endpoints';
import { AddVendorComponent } from './components/vendor/add-vendor/add-vendor.component';
import { VendorListComponent } from './components/vendor/vendor-list/vendor-list.component';
import { ViewVendorComponent } from './components/vendor/view-vendor/view-vendor.component';
import { DeleteVendorComponent } from './components/vendor/delete-vendor/delete-vendor.component';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { AddRoleComponent } from './components/user-management/role/add-role/add-role.component';
import { RoleListComponent } from './components/user-management/role/role-list/role-list.component';
import { UserEndPoints } from './components/user-management/user.endpoints';
import { UserService } from './components/user-management/user.service';
import { AddUserComponent } from './components/user-management/user/add-user/add-user.component';
import { ResetpasswordComponent } from './components/user-management/user/reset-password/reset-password.component';
import { UserListComponent } from './components/user-management/user/user-list/user-list.component';
import { AddCompanyComponent } from './components/company/add-company/add-company.component';
import { ViewCompanyComponent } from './components/company/view-company/view-company.component';
import { CompanyListComponent } from './components/company/company-list/company-list.component';
import { DeleteCompanyComponent } from './components/company/delete-company/delete-company.component';
import { CompanyEndPoints } from './components/company/company.endpoints';
import { CompanyService } from './components/company/company.service';
import { DepartmentEndPoints } from './components/department/department.endpoints';
import { DepartmentService } from './components/department/department.service';
import { AddDepartmentComponent } from './components/department/add-department/add-department.component';
import { ViewDepartmentComponent } from './components/department/view-department/view-department.component';
import { DepartmentListComponent } from './components/department/department-list/department-list.component';
import { DeleteDepartmentComponent } from './components/department/delete-department/delete-department.component';

import { StoreEndPoints } from './components/store/store.endpoints';
import { StoreService } from './components/store/store.service';
import { AddStoreComponent } from './components/store/add-store/add-store.component';
import { ViewStoreComponent } from './components/store/view-store/view-store.component';
import { StoreListComponent } from './components/store/store-list/store-list.component';
import { DeleteStoreComponent } from './components/store/delete-store/delete-store.component';
import { ViewUomComponent } from './components/uom/view-uom/view-uom.component';
import { UomListComponent } from './components/uom/uom-list/uom-list.component';
import { DeleteUomComponent } from './components/uom/delete-uom/delete-uom.component';
import { AddUomComponent } from './components/uom/add-uom/add-uom.component';
import { UomService } from './components/uom/uom.service';
import { UomEndPoints } from './components/uom/uom.endpoints';
import { AddCategoryComponent } from './components/category/add-category/add-category.component';
import { ViewCategoryComponent } from './components/category/view-category/view-category.component';
import { DeleteCategoryComponent } from './components/category/delete-category/delete-category.component';
import { CategoryListComponent } from './components/category/category-list/category-list.component';
import { CategoryService } from './components/category/category.service';
import { CategoryEndPoints } from './components/category/category.endpoints';
import { AddSubcategoryComponent } from './components/subcategory/add-subcategory/add-subcategory.component';
import { DeleteSubcategoryComponent } from './components/subcategory/delete-subcategory/delete-subcategory.component';
import { ViewSubcategoryComponent } from './components/subcategory/view-subcategory/view-subcategory.component';
import { SubcategoryListComponent } from './components/subcategory/subcategory-list/subcategory-list.component';
import { SubcategoryEndPoints } from './components/subcategory/subcategory.endpoints';
import { SubcategoryService } from './components/subcategory/subcategory.service';
import { ItemtypeEndPoints } from './components/itemtype/itemtype.endpoints';
import { ItemtypeService } from './components/itemtype/itemtype.service';
import { AddItemtypeComponent } from './components/itemtype/add-itemtype/add-itemtype.component';
import { DeleteItemtypeComponent } from './components/itemtype/delete-itemtype/delete-itemtype.component';
import { ViewItemtypeComponent } from './components/itemtype/view-itemtype/view-itemtype.component';
import { ItemtypeListComponent } from './components/itemtype/itemtype-list/itemtype-list.component';
import { AddItemComponent } from './components/item/add-item/add-item.component';
import { DeleteItemComponent } from './components/item/delete-item/delete-item.component';
import { ViewItemComponent } from './components/item/view-item/view-item.component';
import { ItemListComponent } from './components/item/item-list/item-list.component';
import { ItemService } from './components/item/item.service';
import { ItemEndPoints } from './components/item/item.endpoints';
import { LocationService } from './components/location/location.service';
import { LocationEndPoints } from './components/location/location.endpoints';
import { AddLocationComponent } from './components/location/add-location/add-location.component';
import { DeleteLocationComponent } from './components/location/delete-location/delete-location.component';
import { LocationListComponent } from './components/location/location-list/location-list.component';
import { ViewLocationComponent } from './components/location/view-location/view-location.component';
import { ProjectEndPoints } from './components/project/project.endpoints';
import { ProjectService } from './components/project/project.service';
import { AddProjectComponent } from './components/project/add-project/add-project.component';
import { DeleteProjectComponent } from './components/project/delete-project/delete-project.component';
import { ProjectListComponent } from './components/project/project-list/project-list.component';
import { ViewProjectComponent } from './components/project/view-project/view-project.component';
import { HeaderComponent } from './components/header/header.component';
import { AddIndentrequestComponent } from './components/indentrequest/add-indentrequest/add-indentrequest.component';
import { DeleteIndentrequestComponent } from './components/indentrequest/delete-indentrequest/delete-indentrequest.component';
import { IndentrequestListComponent } from './components/indentrequest/indentrequest-list/indentrequest-list.component';
import { ViewIndentrequestComponent } from './components/indentrequest/view-indentrequest/view-indentrequest.component';
import { IndentrequestService } from './components/indentrequest/indentrequest.service';
import { IndentrequestEndPoints } from './components/indentrequest/indentrequest.endpoints';
import { ChartitemsComponent } from './components/chartitems/chartitems.component';
import {MatAutocompleteModule} from '@angular/material/autocomplete';
import { ProcessIndentrequestComponent } from './components/indentrequest/process-indentrequest/process-indentrequest.component';
import { IndentrequestTabComponent } from './components/indentrequest/indentrequest-tab/indentrequest-tab.component';
import { MatTab, MatTabGroup, MatTabsModule } from '@angular/material/tabs';
import { ApproveIndentrequestComponent } from './components/indentrequest/approve-indentrequest/approve-indentrequest.component';
import { AddIndentTypeComponent } from './components/indenttype/add-indenttype/add-indenttype.component';
import { ViewIndentTypeComponent } from './components/indenttype/view-indenttype/view-indenttype.component';
import { IndentTypeListComponent } from './components/indenttype/indenttype-list/indenttype-list.component';
import { DeleteIndentTypeComponent } from './components/indenttype/delete-indenttype/delete-indenttype.component';
import { IndentTypeService } from './components/indenttype/indenttype.service';
import { IndentTypeEndPoints } from './components/indenttype/indenttype.endpoints';
import { PriorityService } from './components/priority/priority.service';
import { PriorityEndPoints } from './components/priority/priority.endpoints';
import { AddPriorityComponent } from './components/priority/add-priority/add-priority.component';
import { DeletePriorityComponent } from './components/priority/delete-priority/delete-priority.component';
import { ViewPriorityComponent } from './components/priority/view-priority/view-priority.component';
import { PriorityListComponent } from './components/priority/priority-list/priority-list.component';
import { PurchaseDemandListComponent } from './components/purchasedemand/purchasedemand-list/purchasedemand-list.component';
import { AddPurchaseDemandComponent } from './components/purchasedemand/add-purchasedemand/add-purchasedemand.component';
import { ViewPurchaseDemandComponent } from './components/purchasedemand/view-purchasedemand/view-purchasedemand.component';
import { DeletePurchaseDemandComponent } from './components/purchasedemand/delete-purchasedemand/delete-purchasedemand.component';
import { ProcessPurchaseDemandComponent } from './components/purchasedemand/process-purchasedemand/process-purchasedemand.component';
import { ApprovePurchaseDemandComponent } from './components/purchasedemand/approve-purchasedemand/approve-purchasedemand.component';
import { PurchaseDemandService } from './components/purchasedemand/purchasedemand.service';
import { PurchaseDemandEndPoints } from './components/purchasedemand/purchasedemand.endpoints';
import { PurchaseDemandTabComponent } from './components/purchasedemand/purchasedemand-tab/purchasedemand-tab.component';
import { PrintIndentrequestComponent } from './components/indentrequest/print-indentrequest/print-indentrequest.component';
import { AddPurchaseOrderComponent } from './components/purchaseorder/add-purchaseorder/add-purchaseorder.component';
import { DeletePurchaseOrderComponent } from './components/purchaseorder/delete-purchaseorder/delete-purchaseorder.component';
import { ViewPurchaseOrderComponent } from './components/purchaseorder/view-purchaseorder/view-purchaseorder.component';
import { ApprovePurchaseOrderComponent } from './components/purchaseorder/approve-purchaseorder/approve-purchaseorder.component';
import { ProcessPurchaseOrderComponent } from './components/purchaseorder/process-purchaseorder/process-purchaseorder.component';
import { PurchaseOrderTabComponent } from './components/purchaseorder/purchaseorder-tab/purchaseorder-tab.component';
import { PurchaseOrderListComponent } from './components/purchaseorder/purchaseorder-list/purchaseorder-list.component';
import { PurchaseOrderEndPoints } from './components/purchaseorder/purchaseorder.endpoints';
import { PurchaseOrderService } from './components/purchaseorder/purchaseorder.service';
import { PrintPurchaseDemandComponent } from './components/purchasedemand/print-purchasedemand/print-purchasedemand.component';
import { AddCurrencyComponent } from './components/currency/add-currency/add-currency.component';
import { CurrencyListComponent } from './components/currency/currency-list/currency-list.component';
import { DeleteCurrencyComponent } from './components/currency/delete-currency/delete-currency.component';
import { ViewCurrencyComponent } from './components/currency/view-currency/view-currency.component';
import { CurrencyEndPoints } from './components/currency/currency.endpoints';
import { CurrencyService } from './components/currency/currency.service';
import { ShipmentModeEndPoints } from './components/shipmentmode/shipmentmode.endpoints';
import { ShipmentModeService } from './components/shipmentmode/shipmentmode.service';
import { AddShipmentModeComponent } from './components/shipmentmode/add-shipmentmode/add-shipmentmode.component';
import { DeleteShipmentModeComponent } from './components/shipmentmode/delete-shipmentmode/delete-shipmentmode.component';
import { ViewShipmentModeComponent } from './components/shipmentmode/view-shipmentmode/view-shipmentmode.component';
import { ShipmentModeListComponent } from './components/shipmentmode/shipmentmode-list/shipmentmode-list.component';
import { AddPaymentModeComponent } from './components/paymentmode/add-paymentmode/add-paymentmode.component';
import { DeletePaymentModeComponent } from './components/paymentmode/delete-paymentmode/delete-paymentmode.component';
import { ViewPaymentModeComponent } from './components/paymentmode/view-paymentmode/view-paymentmode.component';
import { PaymentModeListComponent } from './components/paymentmode/paymentmode-list/paymentmode-list.component';
import { PaymentModeEndPoints } from './components/paymentmode/paymentmode.endpoints';
import { PaymentModeService } from './components/paymentmode/paymentmode.service';
import { ComparativeStatementEndPoints } from './components/comparativestatement/comparativestatement.endpoints';
import { ComparativeStatementService } from './components/comparativestatement/comparativestatement.service';
import { AddComparativeStatementComponent } from './components/comparativestatement/add-comparativestatement/add-comparativestatement.component';
import { DeleteComparativeStatementComponent } from './components/comparativestatement/delete-comparativestatement/delete-comparativestatement.component';
import { ViewComparativeStatementComponent } from './components/comparativestatement/view-comparativestatement/view-comparativestatement.component';
import { ProcessComparativeStatementComponent } from './components/comparativestatement/process-comparativestatement/process-comparativestatement.component';
import { ApproveComparativeStatementComponent } from './components/comparativestatement/approve-comparativestatement/approve-comparativestatement.component';
import { ComparativeStatementTabComponent } from './components/comparativestatement/comparativestatement-tab/comparativestatement-tab.component';
import { ComparativeStatementListComponent } from './components/comparativestatement/comparativestatement-list/comparativestatement-list.component';
import { PrintComparativeStatementComponent } from './components/comparativestatement/print-comparativestatement/print-comparativestatement.component';
import { DeliveryTermsEndPoints } from './components/deliveryterms/deliveryterms.endpoints';
import { DeliveryTermsService } from './components/deliveryterms/deliveryterms.service';
import { AddDeliveryTermsComponent } from './components/deliveryterms/add-deliveryterms/add-deliveryterms.component';
import { DeleteDeliveryTermsComponent } from './components/deliveryterms/delete-deliveryterms/delete-deliveryterms.component';
import { ViewDeliveryTermsComponent } from './components/deliveryterms/view-deliveryterms/view-deliveryterms.component';
import { DeliveryTermsListComponent } from './components/deliveryterms/deliveryterms-list/deliveryterms-list.component';
import { GSTService } from './components/gst/gst.service';
import { GSTEndPoints } from './components/gst/gst.endpoints';
import { GSTListComponent } from './components/gst/gst-list/gst-list.component';
import { AddGSTComponent } from './components/gst/add-gst/add-gst.component';
import { DeleteGSTComponent } from './components/gst/delete-gst/delete-gst.component';
import { ViewGSTComponent } from './components/gst/view-gst/view-gst.component';
import {MatRadioModule} from '@angular/material/radio';
import { PrintPurchaseOrderComponent } from './components/purchaseorder/print-purchaseorder/print-purchaseorder.component';
import { IGPService } from './components/igp/igp.service';
import { IGPEndPoints } from './components/igp/igp.endpoints';
import { AddIGPComponent } from './components/igp/add-igp/add-igp.component';
import { DeleteIGPComponent } from './components/igp/delete-igp/delete-igp.component';
import { IGPListComponent } from './components/igp/igp-list/igp-list.component';
import { IGPTabComponent } from './components/igp/igp-tab/igp-tab.component';
import { PrintIGPComponent } from './components/igp/print-igp/print-igp.component';
import { ProcessIGPComponent } from './components/igp/process-igp/process-igp.component';
import { ViewIGPComponent } from './components/igp/view-igp/view-igp.component';
import { AddAccountCategoryComponent } from './components/accountcategory/add-accountcategory/add-accountcategory.component';
import { ViewAccountCategoryComponent } from './components/accountcategory/view-accountcategory/view-accountcategory.component';
import { AccountCategoryListComponent } from './components/accountcategory/accountcategory-list/accountcategory-list.component';
import { AccountCategoryService } from './components/accountcategory/accountcategory.service';
import { AccountCategoryEndPoints } from './components/accountcategory/accountcategory.endpoints';
import { AccountSubcategoryEndPoints } from './components/accountsubcategory/accountsubcategory.endpoints';
import { AccountSubcategoryService } from './components/accountsubcategory/accountsubcategory.service';
import { AddAccountSubcategoryComponent } from './components/accountsubcategory/add-accountsubcategory/add-accountsubcategory.component';
import { DeleteAccountCategoryComponent } from './components/accountcategory/delete-accountcategory/delete-accountcategory.component';
import { DeleteAccountSubcategoryComponent } from './components/accountsubcategory/delete-accountsubcategory/delete-accountsubcategory.component';
import { ViewAccountSubcategoryComponent } from './components/accountsubcategory/view-accountsubcategory/view-accountsubcategory.component';
import { AccountSubcategoryListComponent } from './components/accountsubcategory/accountsubcategory-list/accountsubcategory-list.component';
import { AddAccountTypeComponent } from './components/accounttype/add-accounttype/add-accounttype.component';
import { DeleteAccountTypeComponent } from './components/accounttype/delete-accounttype/delete-accounttype.component';
import { ViewAccountTypeComponent } from './components/accounttype/view-accounttype/view-accounttype.component';
import { AccountTypeListComponent } from './components/accounttype/accounttype-list/accounttype-list.component';
import { AccountTypeService } from './components/accounttype/accounttype.service';
import { AccountTypeEndPoints } from './components/accounttype/accounttype.endpoints';
import { AccountService } from './components/account/account.service';
import { AccountEndPoints } from './components/account/account.endpoints';
import { AddAccountComponent } from './components/account/add-account/add-account.component';
import { AccountListComponent } from './components/account/account-list/account-list.component';
import { ViewAccountComponent } from './components/account/view-account/view-account.component';
import { DeleteAccountComponent } from './components/account/delete-account/delete-account.component';
import { AccountHeadEndPoints } from './components/accounthead/accounthead.endpoints';
import { AccountHeadService } from './components/accounthead/accounthead.service';
import { AccountFlowEndPoints } from './components/accountflow/accountflow.endpoints';
import { AccountFlowService } from './components/accountflow/accountflow.service';
import { AccountChartComponent } from './components/accountchart/accountchart.component';
import { AddTransactionComponent } from './components/transaction/add-transaction/add-transaction.component';
import { DeleteTransactionComponent } from './components/transaction/delete-transaction/delete-transaction.component';
import { ApproveTransactionComponent } from './components/transaction/approve-transaction/approve-transaction.component';
import { ProcessTransactionComponent } from './components/transaction/process-transaction/process-transaction.component';
import { ViewTransactionComponent } from './components/transaction/view-transaction/view-transaction.component';
import { TransactionTabComponent } from './components/transaction/transaction-tab/transaction-tab.component';
import { PrintTransactionComponent } from './components/transaction/print-transaction/print-transaction.component';
import { TransactionListComponent } from './components/transaction/transaction-list/transaction-list.component';
import { TransactionService } from './components/transaction/transaction.service';
import { TransactionEndPoints } from './components/transaction/transaction.endpoints';
import { AddBrvComponent } from './components/brv/add-brv/add-brv.component';
import { ProcessBrvComponent } from './components/brv/process-brv/process-brv.component';
import { ViewBrvComponent } from './components/brv/view-brv/view-brv.component';
import { PrintBrvComponent } from './components/brv/print-brv/print-brv.component';
import { BrvListComponent } from './components/brv/brv-list/brv-list.component';
import { BrvTabComponent } from './components/brv/brv-tab/brv-tab.component';
import { ApproveBrvComponent } from './components/brv/approve-brv/approve-brv.component';
import { AddBpvComponent } from './components/bpv/add-bpv/add-bpv.component';
import { ApproveBpvComponent } from './components/bpv/approve-bpv/approve-bpv.component';
import { ProcessBpvComponent } from './components/bpv/process-bpv/process-bpv.component';
import { DeleteBrvComponent } from './components/brv/delete-brv/delete-brv.component';
import { DeleteBpvComponent } from './components/bpv/delete-bpv/delete-bpv.component';
import { ViewBpvComponent } from './components/bpv/view-bpv/view-bpv.component';
import { PrintBpvComponent } from './components/bpv/print-bpv/print-bpv.component';
import { BpvListComponent } from './components/bpv/bpv-list/bpv-list.component';
import { BpvTabComponent } from './components/bpv/bpv-tab/bpv-tab.component';
import { AddCrvComponent } from './components/crv/add-crv/add-crv.component';
import { ApproveCrvComponent } from './components/crv/approve-crv/approve-crv.component';
import { ViewCrvComponent } from './components/crv/view-crv/view-crv.component';
import { ProcessCrvComponent } from './components/crv/process-crv/process-crv.component';
import { DeleteCrvComponent } from './components/crv/delete-crv/delete-crv.component';
import { PrintCrvComponent } from './components/crv/print-crv/print-crv.component';
import { CrvListComponent } from './components/crv/crv-list/crv-list.component';
import { CrvTabComponent } from './components/crv/crv-tab/crv-tab.component';
import { AddCpvComponent } from './components/cpv/add-cpv/add-cpv.component';
import { ApproveCpvComponent } from './components/cpv/approve-cpv/approve-cpv.component';
import { ProcessCpvComponent } from './components/cpv/process-cpv/process-cpv.component';
import { DeleteCpvComponent } from './components/cpv/delete-cpv/delete-cpv.component';
import { ViewCpvComponent } from './components/cpv/view-cpv/view-cpv.component';
import { PrintCpvComponent } from './components/cpv/print-cpv/print-cpv.component';
import { CpvListComponent } from './components/cpv/cpv-list/cpv-list.component';
import { CpvTabComponent } from './components/cpv/cpv-tab/cpv-tab.component';

const matFormFieldDefaults: MatFormFieldDefaultOptions = {
    appearance: 'outline'
     ,subscriptSizing: 'dynamic'
  };

@NgModule({
    declarations: [
        ChartitemsComponent,
        LoginLayoutComponent,
        HomeLayoutComponent,
        SidemenuComponent,
        HeaderComponent,
        LoginComponent,
        AppComponent,
        MyLoaderComponent,
        AddVendorComponent,
        VendorListComponent,
        ViewVendorComponent,
        DeleteVendorComponent,
        AddUserComponent,
        ResetpasswordComponent,
        UserListComponent,
        AddRoleComponent,
        RoleListComponent,
        AddCompanyComponent,
        ViewCompanyComponent,
        CompanyListComponent,
        DeleteCompanyComponent,
        AddDepartmentComponent,
        ViewDepartmentComponent,
        DepartmentListComponent,
        DeleteDepartmentComponent,
        AddStoreComponent,
        ViewStoreComponent,
        StoreListComponent,
        DeleteStoreComponent,
        AddUomComponent,
        ViewUomComponent,
        UomListComponent,
        DeleteUomComponent,
        AddCategoryComponent,
        DeleteCategoryComponent,
        ViewCategoryComponent,
        CategoryListComponent,
        AddSubcategoryComponent,
        DeleteSubcategoryComponent,
        ViewSubcategoryComponent,
        SubcategoryListComponent,
        AddItemtypeComponent,
        DeleteItemtypeComponent,
        ViewItemtypeComponent,
        ItemtypeListComponent,
        AddItemComponent,
        DeleteItemComponent,
        ViewItemComponent,
        ItemListComponent,
        AddLocationComponent,
        DeleteLocationComponent,
        LocationListComponent,
        ViewLocationComponent,
        AddProjectComponent,
        DeleteProjectComponent,
        ProjectListComponent,
        ViewProjectComponent,
        AddIndentrequestComponent,
        DeleteIndentrequestComponent,
        IndentrequestListComponent,
        DeleteIndentrequestComponent,
        ViewIndentrequestComponent,
        ProcessIndentrequestComponent,
        IndentrequestTabComponent,
        PrintIndentrequestComponent,
        ApproveIndentrequestComponent,
        AddIndentTypeComponent,
        DeleteIndentTypeComponent,
        ViewIndentTypeComponent,
        IndentTypeListComponent,
        AddPriorityComponent,
        DeletePriorityComponent,
        ViewPriorityComponent,
        PriorityListComponent,
        PurchaseDemandListComponent,
        AddPurchaseDemandComponent,
        ViewPurchaseDemandComponent,
        DeletePurchaseDemandComponent,
        ProcessPurchaseDemandComponent,
        ApprovePurchaseDemandComponent,
        PurchaseDemandTabComponent,
        AddPurchaseOrderComponent,
        DeletePurchaseOrderComponent,
        ViewPurchaseOrderComponent,
        ApprovePurchaseOrderComponent,
        ProcessPurchaseOrderComponent,
        PurchaseOrderTabComponent,
        PurchaseOrderListComponent,
        PrintPurchaseOrderComponent,
        PrintPurchaseDemandComponent,
        AddCurrencyComponent,
        CurrencyListComponent,
        DeleteCurrencyComponent,
        ViewCurrencyComponent,
        AddShipmentModeComponent,
        DeleteShipmentModeComponent,
        ViewShipmentModeComponent,
        ShipmentModeListComponent,
        AddPaymentModeComponent,
        DeletePaymentModeComponent,
        ViewPaymentModeComponent,
        PaymentModeListComponent,
        AddComparativeStatementComponent,
        DeleteComparativeStatementComponent,
        ViewComparativeStatementComponent,
        ProcessComparativeStatementComponent,
        ApproveComparativeStatementComponent,
        PrintComparativeStatementComponent,
        ComparativeStatementTabComponent,
        ComparativeStatementListComponent,
        AddDeliveryTermsComponent,
        DeleteDeliveryTermsComponent,
        ViewDeliveryTermsComponent,
        DeliveryTermsListComponent,
        GSTListComponent,
        AddGSTComponent,
        DeleteGSTComponent,
        ViewGSTComponent,
        AddIGPComponent,
        DeleteIGPComponent,
        IGPListComponent,
        IGPTabComponent,
        PrintIGPComponent,
        ProcessIGPComponent,
        ViewIGPComponent,
        AddAccountCategoryComponent,
        ViewAccountCategoryComponent,
        DeleteAccountCategoryComponent,
        AccountCategoryListComponent,
        AddAccountSubcategoryComponent,
        DeleteAccountSubcategoryComponent,
        ViewAccountSubcategoryComponent,
        AccountSubcategoryListComponent,
        AddAccountTypeComponent,
        DeleteAccountTypeComponent,
        ViewAccountTypeComponent,
        AccountTypeListComponent,
        AccountListComponent,
        AddAccountComponent,
        ViewAccountComponent,
        DeleteAccountComponent,
        AccountChartComponent,
        AddTransactionComponent,
        DeleteTransactionComponent,
        ApproveTransactionComponent,
        ProcessTransactionComponent,
        ViewTransactionComponent,
        TransactionTabComponent,
        PrintTransactionComponent,
        TransactionListComponent,
        AddBrvComponent,
        DeleteBrvComponent,
        ProcessBrvComponent,
        ViewBrvComponent,
        PrintBrvComponent,
        BrvListComponent,
        BrvTabComponent,
        ApproveBrvComponent,
        AddBpvComponent,
        ApproveBpvComponent,
        ProcessBpvComponent,
        DeleteBpvComponent,
        ViewBpvComponent,
        PrintBpvComponent,
        BpvListComponent,
        BpvTabComponent,
        AddCrvComponent,
        ApproveCrvComponent,
        ViewCrvComponent,
        ProcessCrvComponent,
        DeleteCrvComponent,
        PrintCrvComponent,
        CrvListComponent,
        CrvTabComponent,
        AddCpvComponent,
        ApproveCpvComponent,
        ProcessCpvComponent,
        DeleteCpvComponent,
        ViewCpvComponent,
        PrintCpvComponent,
        CpvListComponent,
        CpvTabComponent
    ],
    bootstrap: [AppComponent], imports: [
        InputMaskModule,
        BrowserModule,
        AppRoutingModule,
        NgbDatepickerModule,
        FormsModule,
        NgbTypeaheadModule,
        CommonModule,
        JsonPipe,
        ReactiveFormsModule,
        MatInputModule,
        MatIconModule,
        BrowserAnimationsModule,
        MatToolbarModule,
        MatSidenavModule,
        MatButtonModule,
        MatDividerModule,
        MatFormFieldModule,
        RouterModule,
        MatCardModule,
        MatTableModule,
        MatDialogModule,
        MatPaginatorModule,
        MatCardModule,
        MatProgressSpinnerModule,
        MatCardModule,
        MatSortModule,
        MatTooltipModule,
        MatSelectModule,
        MatButtonModule,
        MatMenuModule,
        MatGridListModule,
        MatDatepickerModule,
        MatSort,
        MatListModule,
        MatFormField,
        MatCheckboxModule,
        MatSlideToggleModule,
        MatAutocompleteModule,
        MatTabGroup,
        MatTab,
        MatTabsModule,
        MatRadioModule
    ],
    providers: [
        provideNativeDateAdapter(),
        JwtHelperService,
        NotificationsService,
        ConstantService,
        GeneralService,
        MediaService,
        GeneralEndPoints,
        ControllerEndpoints,
        AuthEndPoints,
        VendorService,
        VendorEndPoints,
        UserService,
        UserEndPoints,
        CompanyEndPoints,
        CompanyService,
        DepartmentEndPoints,
        DepartmentService,
        StoreEndPoints,
        StoreService,
        UomService,
        UomEndPoints,
        CategoryService,
        CategoryEndPoints,
        SubcategoryEndPoints,
        SubcategoryService,
        ItemtypeEndPoints,
        ItemtypeService,
        ItemService,
        ItemEndPoints,
        LocationService,
        LocationEndPoints,
        ProjectEndPoints,
        ProjectService,
        IndentrequestService,
        IndentrequestEndPoints,
        IndentTypeService,
        IndentTypeEndPoints,
        PriorityService,
        PriorityEndPoints,
        PurchaseDemandService,
        PurchaseDemandEndPoints,
        PurchaseOrderEndPoints,
        PurchaseOrderService,
        CurrencyEndPoints,
        CurrencyService,
        ShipmentModeEndPoints,
        ShipmentModeService,
        PaymentModeEndPoints,
        PaymentModeService,
        ComparativeStatementEndPoints,
        ComparativeStatementService,
        DeliveryTermsEndPoints,
        DeliveryTermsService,
        GSTService,
        GSTEndPoints,
        IGPService,
        IGPEndPoints,
        AccountCategoryService,
        AccountCategoryEndPoints,
        AccountSubcategoryEndPoints,
        AccountSubcategoryService,
        AccountTypeService,
        AccountTypeEndPoints,
        AccountService,
        AccountEndPoints,
        AccountHeadEndPoints,
        AccountHeadService,
        AccountFlowEndPoints,
        AccountFlowService,
        TransactionService,
        TransactionEndPoints,
        { provide: HTTP_INTERCEPTORS, useClass: LoaderInterceptor, multi: true },
        { provide: JWT_OPTIONS, useValue: JWT_OPTIONS },
        AuthenticationService, {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthInterceptor,
            multi: true
        },
        { provide: MAT_FORM_FIELD_DEFAULT_OPTIONS, useValue: matFormFieldDefaults },
        LoaderService,
        provideHttpClient(withInterceptorsFromDi())
    ]
})
export class AppModule { }
