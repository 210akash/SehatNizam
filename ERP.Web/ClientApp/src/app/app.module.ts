import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NgbDatepickerModule, NgbTypeaheadModule } from '@ng-bootstrap/ng-bootstrap';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule, DatePipe, JsonPipe } from '@angular/common';
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
import { MatAutocompleteModule } from '@angular/material/autocomplete';
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
import { MatRadioModule } from '@angular/material/radio';
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
import { AreaListComponent } from './components/order/area/area-list/area-list.component';
import { CreateAreaComponent } from './components/order/area/create-area/create-area.component';
import { DeleteAreaComponent } from './components/order/area/delete-area/delete-area.component';
import { ViewAreaComponent } from './components/order/area/view-area/view-area.component';
import { CreateDealershipComponent } from './components/order/dealership/create-dealership/create-dealership.component';
import { DealershipListComponent } from './components/order/dealership/dealership-list/dealership-list.component';
import { DeleteDealershipComponent } from './components/order/dealership/delete-dealership/delete-dealership.component';
import { ViewDealershipComponent } from './components/order/dealership/view-dealership/view-dealership.component';
import { DrawMapComponent } from './components/order/gmap/draw-map/draw-map.component';
import { DrawRouteShopsComponent } from './components/order/gmap/draw-route-shops/draw-route-shops.component';
import { GmapviewerComponent } from './components/order/gmap/gmapviewer/gmapviewer.component';
import { CreateRegionComponent } from './components/order/region/create-region/create-region.component';
import { DeleteRegionComponent } from './components/order/region/delete-region/delete-region.component';
import { RegionListComponent } from './components/order/region/region-list/region-list.component';
import { ViewRegionComponent } from './components/order/region/view-region/view-region.component';
import { AddShopsRouteFrequencyComponent } from './components/order/route/add-shops-route-frequency/add-shops-route-frequency.component';
import { AddShopsRouteComponent } from './components/order/route/add-shops-route/add-shops-route.component';
import { CreateRouteComponent } from './components/order/route/create-route/create-route.component';
import { DeleteRouteComponent } from './components/order/route/delete-route/delete-route.component';
import { RouteListComponent } from './components/order/route/route-list/route-list.component';
import { ViewRouteComponent } from './components/order/route/view-route/view-route.component';
import { CreateShopTypeComponent } from './components/order/shop-type/create-shop-type/create-shop-type.component';
import { DeleteShopTypeComponent } from './components/order/shop-type/delete-shop-type/delete-shop-type.component';
import { ShopTypeListComponent } from './components/order/shop-type/shop-type-list/shop-type-list.component';
import { ViewShopTypeComponent } from './components/order/shop-type/view-shop-type/view-shop-type.component';
import { CreateShopComponent } from './components/order/shop/create-shop/create-shop.component';
import { DeleteShopComponent } from './components/order/shop/delete-shop/delete-shop.component';
import { ShopListComponent } from './components/order/shop/shop-list/shop-list.component';
import { ViewShopComponent } from './components/order/shop/view-shop/view-shop.component';
import { CreateTerritoryComponent } from './components/order/territory/create-territory/create-territory.component';
import { DeleteTerritoryComponent } from './components/order/territory/delete-territory/delete-territory.component';
import { TerritoryListComponent } from './components/order/territory/territory-list/territory-list.component';
import { ViewTerritoryComponent } from './components/order/territory/view-territory/view-territory.component';
import { CreateZoneComponent } from './components/order/zone/create-zone/create-zone.component';
import { DeleteZoneComponent } from './components/order/zone/delete-zone/delete-zone.component';
import { FieldMapComponent } from './components/order/zone/field-map/field-map.component';
import { PredFieldMapComponent } from './components/order/zone/pred-field-map/pred-field-map.component';
import { ViewZoneComponent } from './components/order/zone/view-zone/view-zone.component';
import { ZoneListComponent } from './components/order/zone/zone-list/zone-list.component';
import { AreaEndPoints } from './components/order/area/area.endpoints';
import { AreaService } from './components/order/area/area.service';
import { DealershipEndPoints } from './components/order/dealership/dealership.endpoints';
import { DealershipService } from './components/order/dealership/dealership.service';
import { GmapEndPoints } from './components/order/gmap/gmap.endpoints';
import { GmapService } from './components/order/gmap/gmap.service';
import { RegionEndPoints } from './components/order/region/region.endpoints';
import { RegionService } from './components/order/region/region.service';
import { RouteEndPoints } from './components/order/route/route.endpoints';
import { RouteService } from './components/order/route/route.service';
import { ShopTypeEndPoints } from './components/order/shop-type/shop-type.endpoints';
import { ShopTypeService } from './components/order/shop-type/shop-type.service';
import { ShopEndPoints } from './components/order/shop/shop.endpoints';
import { ShopService } from './components/order/shop/shop.service';
import { TerritoryEndPoints } from './components/order/territory/territory.endpoints';
import { TerritoryService } from './components/order/territory/territory.service';
import { ZoneEndPoints } from './components/order/zone/zone.endpoints';
import { ZoneService } from './components/order/zone/zone.service';
import { AddInspectionComponent } from './components/inspection/add-inspection/add-inspection.component';
import { DeleteInspectionComponent } from './components/inspection/delete-inspection/delete-inspection.component';
import { ViewInspectionComponent } from './components/inspection/view-inspection/view-inspection.component';
import { ProcessInspectionComponent } from './components/inspection/process-inspection/process-inspection.component';
import { PrintInspectionComponent } from './components/inspection/print-inspection/print-inspection.component';
import { InspectionListComponent } from './components/inspection/inspection-list/inspection-list.component';
import { InspectionTabComponent } from './components/inspection/inspection-tab/inspection-tab.component';
import { InspectionService } from './components/inspection/inspection.service';
import { InspectionEndPoints } from './components/inspection/inspection.endpoints';
import { RejectReasonEndPoints } from './components/rejectreason/rejectreason.endpoints';
import { RejectReasonService } from './components/rejectreason/rejectreason.service';
import { AddRejectReasonComponent } from './components/rejectreason/add-rejectreason/add-rejectreason.component';
import { DeleteRejectReasonComponent } from './components/rejectreason/delete-rejectreason/delete-rejectreason.component';
import { ViewRejectReasonComponent } from './components/rejectreason/view-rejectreason/view-rejectreason.component';
import { RejectReasonListComponent } from './components/rejectreason/rejectreason-list/rejectreason-list.component';
import { DSFEndPoints } from './components/order/DSF/DSF.endpoints';
import { DSFService } from './components/order/DSF/DSF.service';
import { AddDSFRouteComponent } from './components/order/DSF/add-DSF-route/add-DSF-route.component';
import { DSFListComponent } from './components/order/DSF/DSF-list/DSF-list.component';
import { ViewDSFComponent } from './components/order/DSF/view-DSF/view-DSF.component';
import { CreateDistributorPriceGroupComponent } from './components/order/pricing-group/create-distributor-price-group/create-distributor-price-group.component';
import { CreatePricingGroupComponent } from './components/order/pricing-group/create-pricing-group/create-pricing-group.component';
import { CreatePricingGroupDetailsComponent } from './components/order/pricing-group/create-pricing-group-details/create-pricing-group-details.component';
import { PricingGroupListComponent } from './components/order/pricing-group/pricing-group-list/pricing-group-list.component';
import { PricingGroupEndPoints } from './components/order/pricing-group/pricing-group.endpoints';
import { PricingGroupService } from './components/order/pricing-group/pricing-group.service';
import { VehicleService } from './components/order/vehicle/vehicle.service';
import { VehicleEndPoints } from './components/order/vehicle/vehicle.endpoints';
import { VehicleListComponent } from './components/order/vehicle/vehicle-list/vehicle-list.component';
import { ViewVehicleComponent } from './components/order/vehicle/view-vehicle/view-vehicle.component';
import { DeleteVehicleComponent } from './components/order/vehicle/delete-vehicle/delete-vehicle.component';
import { CreateVehicleComponent } from './components/order/vehicle/create-vehicle/create-vehicle.component';
import { PrimaryOrderEndPoints } from './components/order/primary-order/order.endpoints';
import { PrimaryOrderService } from './components/order/primary-order/order.service';
import { CreateOrderComponent } from './components/order/primary-order/create-order/create-order.component';
import { DeleteOrderComponent } from './components/order/primary-order/delete-order/delete-order.component';
import { OrderHistoryComponent } from './components/order/primary-order/order-history/order-history.component';
import { OrderListComponent } from './components/order/primary-order/order-list/order-list.component';
import { OrderStatusChangeComponent } from './components/order/primary-order/order-status-change/order-status-change.component';
import { ViewOrderComponent } from './components/order/primary-order/view-order/view-order.component';
import { UserAttendanceEndPoints } from './components/order/user-attendance/user-attendance.endpoints';
import { UserAttendanceService } from './components/order/user-attendance/user-attendance.service';
import { ViewUserattendanceComponent } from './components/order/user-attendance/view-user-attendance/view-user-attendance.component';
import { UserAttendanceListComponent } from './components/order/user-attendance/user-attendance-list/user-attendance-list.component';
import { UserTerritoryEndPoints } from './components/order/user-territory/user-territory.endpoints';
import { UserTerritoryService } from './components/order/user-territory/user-territory.service';
import { UserTerritoryListComponent } from './components/order/user-territory/user-territory-list/user-territory-list.component';
import { ViewUserTerritoryComponent } from './components/order/user-territory/view-user-territory/view-user-territory.component';
import { CreateUserTerritoryComponent } from './components/order/user-territory/create-user-territory/create-user-territory.component';
import { DeleteUserTerritoryComponent } from './components/order/user-territory/delete-user-territory/delete-user-territory.component';
import { ApproveIGPComponent } from './components/igp/approve-igp/approve-igp.component';
import { ApproveInspectionComponent } from './components/inspection/approve-inspection/approve-inspection.component';
import { AddTemplateComponent } from './components/order/templates/add-template/add-template.component';
import { TemplateService } from './components/order/templates/template.service';
import { TemplateEndPoints } from './components/order/templates/template.endpoints';
import { TemplateListComponent } from './components/order/templates/template-list/template-list.component';
import { SafeHtml } from "./components/Shared/SafeHtml";
import { GRNService } from './components/grn/grn.service';
import { GRNEndPoints } from './components/grn/grn.endpoints';
import { AddGRNComponent } from './components/grn/add-grn/add-grn.component';
import { DeleteGRNComponent } from './components/grn/delete-grn/delete-grn.component';
import { ViewGRNComponent } from './components/grn/view-grn/view-grn.component';
import { ProcessGRNComponent } from './components/grn/process-grn/process-grn.component';
import { ApproveGRNComponent } from './components/grn/approve-grn/approve-grn.component';
import { GRNListComponent } from './components/grn/grn-list/grn-list.component';
import { GRNTabComponent } from './components/grn/grn-tab/grn-tab.component';
import { PrintGRNComponent } from './components/grn/print-grn/print-grn.component';
import { AddAccountFlowComponent } from './components/accountflow/add-accountflow/add-accountflow.component';
import { AccountFlowListComponent } from './components/accountflow/accountflow-list/accountflow-list.component';
import { DeleteAccountFlowComponent } from './components/accountflow/delete-accountflow/delete-accountflow.component';
import { ViewAccountFlowComponent } from './components/accountflow/view-accountflow/view-accountflow.component';
import { RackListComponent } from './components/rack/rack-list/rack-list.component';
import { AddRackComponent } from './components/rack/add-rack/add-rack.component';
import { DeleteRackComponent } from './components/rack/delete-rack/delete-rack.component';
import { ViewRackComponent } from './components/rack/view-rack/view-rack.component';
import { RackEndPoints } from './components/rack/rack.endpoints';
import { RackService } from './components/rack/rack.service';
import { RowListComponent } from './components/row/row-list/row-list.component';
import { AddRowComponent } from './components/row/add-row/add-row.component';
import { ViewRowComponent } from './components/row/view-row/view-row.component';
import { DeleteRowComponent } from './components/row/delete-row/delete-row.component';
import { RowEndPoints } from './components/row/row.endpoints';
import { RowService } from './components/row/row.service';
import { SectionEndPoints } from './components/section/section.endpoints';
import { SectionService } from './components/section/section.service';
import { SectionListComponent } from './components/section/section-list/section-list.component';
import { AddSectionComponent } from './components/section/add-section/add-section.component';
import { ViewSectionComponent } from './components/section/view-section/view-section.component';
import { DeleteSectionComponent } from './components/section/delete-section/delete-section.component';
import { ApproveAuditReviewComponent } from './components/auditreview/approve-auditreview/approve-auditreview.component';
import { AuditReviewTabComponent } from './components/auditreview/auditreview-tab/auditreview-tab.component';
import { AuditReviewListComponent } from './components/auditreview/auditreview-list/auditreview-list.component';
import { DeleteAuditReviewComponent } from './components/auditreview/delete-auditreview/delete-auditreview.component';
import { ProcessAuditReviewComponent } from './components/auditreview/process-auditreview/process-auditreview.component';
import { PrintAuditReviewComponent } from './components/auditreview/print-auditreview/print-auditreview.component';
import { ViewAuditReviewComponent } from './components/auditreview/view-auditreview/view-auditreview.component';
import { AuditReviewService } from './components/auditreview/auditreview.service';
import { AuditReviewEndPoints } from './components/auditreview/auditreview.endpoints';
import { AccountGroupService } from './components/accountgroup/accountgroup.service';
import { AccountGroupEndPoints } from './components/accountgroup/accountgroup.endpoints';
import { AddAccountGroupComponent } from './components/accountgroup/add-accountgroup/add-accountgroup.component';
import { AccountGroupListComponent } from './components/accountgroup/accountgroup-list/accountgroup-list.component';
import { DeleteAccountGroupComponent } from './components/accountgroup/delete-accountgroup/delete-accountgroup.component';
import { ViewAccountGroupComponent } from './components/accountgroup/view-accountgroup/view-accountgroup.component';
import { MatTimepickerModule } from '@angular/material/timepicker';
import { CreateDSFTargetComponent } from './components/order/sales-target/create-dsf-target/create-dsf-target.component';
import { CreateSalesTargetComponent } from './components/order/sales-target/create-sales-target/create-sales-target.component';
import { CreateTerritoryTargetComponent } from './components/order/sales-target/create-territory-target/create-territory-target.component';
import { CreateZoneTargetComponent } from './components/order/sales-target/create-zone-target/create-zone-target.component';
import { DeleteSalesTargetComponent } from './components/order/sales-target/delete-sales-target/delete-sales-target.component';
import { SalesTargetListComponent } from './components/order/sales-target/sales-target-list/sales-target-list.component';
import { ViewSalesTargetComponent } from './components/order/sales-target/view-sales-target/view-sales-target.component';
import { SalesTargetEndPoints } from './components/order/sales-target/sales-target.endpoints';
import { SalesTargetService } from './components/order/sales-target/sales-target.service';
import { IssuanceService } from './components/issuance/issuance.service';
import { IssuanceEndPoints } from './components/issuance/issuance.endpoints';
import { AddIssuanceComponent } from './components/issuance/add-issuance/add-issuance.component';
import { ApproveIssuanceComponent } from './components/issuance/approve-issuance/approve-issuance.component';
import { DeleteIssuanceComponent } from './components/issuance/delete-issuance/delete-issuance.component';
import { IssuanceListComponent } from './components/issuance/issuance-list/issuance-list.component';
import { IssuanceTabComponent } from './components/issuance/issuance-tab/issuance-tab.component';
import { PrintIssuanceComponent } from './components/issuance/print-issuance/print-issuance.component';
import { ProcessIssuanceComponent } from './components/issuance/process-issuance/process-issuance.component';
import { ViewIssuanceComponent } from './components/issuance/view-issuance/view-issuance.component';
import { AddDispatchComponent } from './components/dispatch/add-dispatch/add-dispatch.component';
import { ApproveDispatchComponent } from './components/dispatch/approve-dispatch/approve-dispatch.component';
import { DeleteDispatchComponent } from './components/dispatch/delete-dispatch/delete-dispatch.component';
import { DispatchListComponent } from './components/dispatch/dispatch-list/dispatch-list.component';
import { DispatchTabComponent } from './components/dispatch/dispatch-tab/dispatch-tab.component';
import { ProcessDispatchComponent } from './components/dispatch/process-dispatch/process-dispatch.component';
import { ViewDispatchComponent } from './components/dispatch/view-dispatch/view-dispatch.component';
import { DispatchEndPoints } from './components/dispatch/dispatch.endpoints';
import { DispatchService } from './components/dispatch/dispatch.service';
import { PrintDispatchOrdersPopupComponent } from './components/dispatch/print-dispatch-orders-popup/print-dispatch-orders-popup.component';
import { PrintDispatchOrderReceiptComponent } from './components/dispatch/print-dispatch-order-receipt/print-dispatch-order-receipt.component';
import { LedgerService } from './components/ledger/ledger.service';
import { LedgerEndPoints } from './components/ledger/ledger.endpoints';
import { AddSaleUserComponent } from './components/order/sale-users/add-sale-user/add-sale-user.component';
import { SaleUsersListComponent } from './components/order/sale-users/sale-users-list/sale-users-list.component';
import { ReportViewerComponent } from './components/report/report-viewer.component';
import { ReceiveDispatchComponent } from './components/order/primary-order/receive-dispatch/receive-dispatch.component';
import { PrintOrderReceiptComponent } from './components/order/primary-order/print-order-receipt/print-order-receipt.component';
import { PendingDispatchOrderListComponent } from './components/dispatch/pending-dispatch-order-list/pending-dispatch-order-list.component';
import { SJVListComponent } from './components/sjv/sjv-list/sjv-list.component';
import { SJVTabComponent } from './components/sjv/sjv-tab/sjv-tab.component';
import { ViewSJVComponent } from './components/sjv/view-sjv/view-sjv.component';
import { PrintDispatchOrderInvoiceComponent } from './components/dispatch/print-dispatch-order-invoice/print-dispatch-order-invoice.component';
import { ResetPasswordComponent } from './Auth/reset-password/reset-password.component';
import { NumberToWordsPipe } from './components/Shared/number-to-words.pipe';
import { AddCostSheetComponent } from './components/costsheet/add-costsheet/add-costsheet.component';
import { CostsheetTabComponent } from './components/costsheet/costsheet-tab/costsheet-tab.component';
import { CostsheetListComponent } from './components/costsheet/costsheet-list/costsheet-list.component';
import { ProcessCostSheetComponent } from './components/costsheet/process-costsheet/process-costsheet.component';
import { DeleteCostSheetComponent } from './components/costsheet/delete-costsheet/delete-costsheet.component';
import { ViewCostSheetComponent } from './components/costsheet/view-costsheet/view-costsheet.component';
import { ApproveCostSheetComponent } from './components/costsheet/approve-costsheet/approve-costsheet.component';
import { PrintCostSheetComponent } from './components/costsheet/print-costsheet/print-costsheet.component';
import { CostSheetService } from './components/costsheet/costsheet.service';
import { CostSheetEndPoints } from './components/costsheet/costsheet.endpoints';
import { NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { CancelDispatchService } from './components/canceldispatch/canceldispatch.service';
import { CancelDispatchEndPoints } from './components/canceldispatch/canceldispatch.endpoints';
import { CancelDispatchTabComponent } from './components/canceldispatch/cancel-dispatch-tab/cancel-dispatch-tab.component';
import { CancelDispatchListComponent } from './components/canceldispatch/cancel-dispatch-list/cancel-dispatch-list.component';
import { AddCancelDispatchComponent } from './components/canceldispatch/add-cancel-dispatch/add-cancel-dispatch.component';
import { ProcessCancelDispatchComponent } from './components/canceldispatch/process-cancel-dispatch/process-cancel-dispatch.component';
import { ViewCancelDispatchComponent } from './components/canceldispatch/view-cancel-dispatch/view-cancel-dispatch.component';
import { DeleteCancelDispatchComponent } from './components/canceldispatch/delete-cancel-dispatch/delete-cancel-dispatch.component';
import { RejectCancelDispatchComponent } from './components/canceldispatch/reject-cancel-dispatch/reject-cancel-dispatch.component';
import { CancelDispatchHistoryComponent } from './components/canceldispatch/cancel-dispatch-history/cancel-dispatch-history.component';
import { PJVTabComponent } from './components/pjv/pjv-tab/pjv-tab.component';
import { PJVListComponent } from './components/pjv/pjv-list/pjv-list.component';
import { ViewPJVComponent } from './components/pjv/view-pjv/view-pjv.component';
import { PrintPJVComponent } from './components/pjv/print-pjv/print-pjv.component';
import { PrintSJVComponent } from './components/sjv/print-sjv/print-sjv.component';
import { AddSaleMaterialComponent } from './components/salematerial/add-salematerial/add-salematerial.component';
import { ApproveSaleMaterialComponent } from './components/salematerial/approve-salematerial/approve-salematerial.component';
import { DeleteSaleMaterialComponent } from './components/salematerial/delete-salematerial/delete-salematerial.component';
import { PrintSaleMaterialComponent } from './components/salematerial/print-salematerial/print-salematerial.component';
import { ProcessSaleMaterialComponent } from './components/salematerial/process-salematerial/process-salematerial.component';
import { ViewSaleMaterialComponent } from './components/salematerial/view-salematerial/view-salematerial.component';
import { SaleMaterialEndPoints } from './components/salematerial/salematerial.endpoints';
import { SaleMaterialService } from './components/salematerial/salematerial.service';
import { SaleMaterialListComponent } from './components/salematerial/salematerial-list/salematerial-list.component';
import { SaleMaterialTabComponent } from './components/salematerial/salematerial-tab/salematerial-tab.component';
import { CreateCustomerComponent } from './components/customer/create-customer/create-customer.component';
import { ViewCustomerComponent } from './components/customer/view-customer/view-customer.component';
import { DeleteCustomerComponent } from './components/customer/delete-customer/delete-customer.component';
import { CustomerListComponent } from './components/customer/customer-list/customer-list.component';
import { ApprovePurchaseInvoiceComponent } from './components/purchaseinvoice/approve-purchaseinvoice/approve-purchaseinvoice.component';
import { PurchaseInvoiceListComponent } from './components/purchaseinvoice/purchaseinvoice-list/purchaseinvoice-list.component';
import { PurchaseInvoiceTabComponent } from './components/purchaseinvoice/purchaseinvoice-tab/purchaseinvoice-tab.component';
import { ViewPurchaseInvoiceComponent } from './components/purchaseinvoice/view-purchaseinvoice/view-purchaseinvoice.component';
import { PrintPurchaseInvoiceComponent } from './components/purchaseinvoice/print-purchaseinvoice/print-purchaseinvoice.component';
import { DocumentViewerComponent } from './components/document-viewer/document-viewer.component';
import { EmployeeDesignationService } from './components/hr/employee-designation/employee-designation.service';
import { EmployeeDesignationEndPoints } from './components/hr/employee-designation/employee-designation.endpoints';
import { EmployeeEducationService } from './components/hr/employee-education/employee-education.service';
import { EmployeeEducationEndPoints } from './components/hr/employee-education/employee-education.endpoints';
import { EmployeeGradeService } from './components/hr/employee-grade/employee-grade.service';
import { EmployeeGradeEndPoints } from './components/hr/employee-grade/employee-grade.endpoints';
import { EmployeeShiftService } from './components/hr/employee-shift/employee-shift.service';
import { EmployeeShiftEndPoints } from './components/hr/employee-shift/employee-shift.endpoints';
import { EmployeeTypeService } from './components/hr/employee-type/employee-type.service';
import { EmployeeTypeEndPoints } from './components/hr/employee-type/employee-type.endpoints';
import { AddEmployeeDesignationComponent } from './components/hr/employee-designation/add-employee-designation/add-employee-designation.component';
import { DeleteEmployeeDesignationComponent } from './components/hr/employee-designation/delete-employee-designation/delete-employee-designation.component';
import { EmployeeDesignationListComponent } from './components/hr/employee-designation/employee-designation-list/employee-designation-list.component';
import { ViewEmployeeDesignationComponent } from './components/hr/employee-designation/view-employee-designation/view-employee-designation.component';
import { AddEmployeeEducationComponent } from './components/hr/employee-education/add-employee-education/add-employee-education.component';
import { DeleteEmployeeEducationComponent } from './components/hr/employee-education/delete-employee-education/delete-employee-education.component';
import { EmployeeEducationListComponent } from './components/hr/employee-education/employee-education-list/employee-education-list.component';
import { ViewEmployeeEducationComponent } from './components/hr/employee-education/view-employee-education/view-employee-education.component';
import { AddEmployeeGradeComponent } from './components/hr/employee-grade/add-employee-grade/add-employee-grade.component';
import { DeleteEmployeeGradeComponent } from './components/hr/employee-grade/delete-employee-grade/delete-employee-grade.component';
import { EmployeeGradeListComponent } from './components/hr/employee-grade/employee-grade-list/employee-grade-list.component';
import { ViewEmployeeGradeComponent } from './components/hr/employee-grade/view-employee-grade/view-employee-grade.component';
import { AddEmployeeShiftComponent } from './components/hr/employee-shift/add-employee-shift/add-employee-shift.component';
import { DeleteEmployeeShiftComponent } from './components/hr/employee-shift/delete-employee-shift/delete-employee-shift.component';
import { EmployeeShiftListComponent } from './components/hr/employee-shift/employee-shift-list/employee-shift-list.component';
import { ViewEmployeeShiftComponent } from './components/hr/employee-shift/view-employee-shift/view-employee-shift.component';
import { AddEmployeeTypeComponent } from './components/hr/employee-type/add-employee-type/add-employee-type.component';
import { DeleteEmployeeTypeComponent } from './components/hr/employee-type/delete-employee-type/delete-employee-type.component';
import { EmployeeTypeListComponent } from './components/hr/employee-type/employee-type-list/employee-type-list.component';
import { ViewEmployeeTypeComponent } from './components/hr/employee-type/view-employee-type/view-employee-type.component';
import { AddEmployeeComponent } from './components/hr/employee/add-employee/add-employee.component';
import { EmployeeListComponent } from './components/hr/employee/employee-list/employee-list.component';
import { EmployeeBankListComponent } from './components/hr/employee-bank/employee-bank-list/employee-bank-list.component';
import { AddEmployeeBankComponent } from './components/hr/employee-bank/add-employee-bank/add-employee-bank.component';
import { ViewEmployeeBankComponent } from './components/hr/employee-bank/view-employee-bank/view-employee-bank.component';
import { DeleteEmployeeBankComponent } from './components/hr/employee-bank/delete-employee-bank/delete-employee-bank.component';
import { EmployeeBankService } from './components/hr/employee-bank/employee-bank.service';
import { EmployeeBankEndPoints } from './components/hr/employee-bank/employee-bank.endpoints';
import { EmployeeLeaveGroupEndPoints } from './components/hr/employee-leave-group/employee-leave-group.endpoints';
import { EmployeeLeaveGroupService } from './components/hr/employee-leave-group/employee-leave-group.service';
import { EmployeeLeaveTypeEndPoints } from './components/hr/employee-leave-type/employee-leave-type.endpoints';
import { EmployeeLeaveTypeService } from './components/hr/employee-leave-type/employee-leave-type.service';
import { AddEmployeeLeaveGroupComponent } from './components/hr/employee-leave-group/add-employee-leave-group/add-employee-leave-group.component';
import { DeleteEmployeeLeaveGroupComponent } from './components/hr/employee-leave-group/delete-employee-leave-group/delete-employee-leave-group.component';
import { EmployeeLeaveGroupListComponent } from './components/hr/employee-leave-group/employee-leave-group-list/employee-leave-group-list.component';
import { ViewEmployeeLeaveGroupComponent } from './components/hr/employee-leave-group/view-employee-leave-group/view-employee-leave-group.component';
import { AddEmployeeLeaveTypeComponent } from './components/hr/employee-leave-type/add-employee-leave-type/add-employee-leave-type.component';
import { DeleteEmployeeLeaveTypeComponent } from './components/hr/employee-leave-type/delete-employee-leave-type/delete-employee-leave-type.component';
import { EmployeeLeaveTypeListComponent } from './components/hr/employee-leave-type/employee-leave-type-list/employee-leave-type-list.component';
import { ViewEmployeeLeaveTypeComponent } from './components/hr/employee-leave-type/view-employee-leave-type/view-employee-leave-type.component';
import { EmployeeDocumentTypeEndPoints } from './components/hr/employee-document-type/employee-document-type.endpoints';
import { EmployeeDocumentTypeService } from './components/hr/employee-document-type/employee-document-type.service';
import { AddEmployeeDocumentTypeComponent } from './components/hr/employee-document-type/add-employee-document-type/add-employee-document-type.component';
import { EmployeeDocumentTypeListComponent } from './components/hr/employee-document-type/employee-document-type-list/employee-document-type-list.component';
import { ViewEmployeeDocumentTypeComponent } from './components/hr/employee-document-type/view-employee-document-type/view-employee-document-type.component';
import { DeleteEmployeeDocumentTypeComponent } from './components/hr/employee-document-type/delete-employee-document-type/delete-employee-document-type.component';
import { AddCityComponent } from './components/hr/city/add-city/add-city.component';
import { CityListComponent } from './components/hr/city/city-list/city-list.component';
import { ViewCityComponent } from './components/hr/city/view-city/view-city.component';
import { DeleteCityComponent } from './components/hr/city/delete-city/delete-city.component';
import { CityEndPoints } from './components/hr/city/city.endpoints';
import { CityService } from './components/hr/city/city.service';
import { IJVListComponent } from './components/ijv/ijv-list/ijv-list.component';
import { IJVTabComponent } from './components/ijv/ijv-tab/ijv-tab.component';
import { ViewIJVComponent } from './components/ijv/view-ijv/view-ijv.component';
import { PrintIJVComponent } from './components/ijv/print-ijv/print-ijv.component';
import { CreateDeviceComponent } from './components/device/create-device/create-device.component';
import { DeviceListComponent } from './components/device/device-list/device-list.component';
import { DeleteDeviceComponent } from './components/device/delete-device/delete-device.component';
import { ViewDeviceComponent } from './components/device/view-device/view-device.component';
import { DeviceEndPoints } from './components/device/device.endpoints';
import { DeviceService } from './components/device/device.service';
import { CreateOrderWithPGComponent } from './components/order/primary-order/create-order-with-pg/create-order-with-pg.component';
import { CancelPurchaseOrderComponent } from './components/purchaseorder/cancel-purchaseorder/cancel-purchaseorder.component';
import { CopyPricingGroupDetailsComponent } from './components/order/pricing-group/copy-pricing-group-details/copy-pricing-group-details.component';
import { IGPTypeService } from './components/igptype/igptype.service';
import { IGPTypeEndPoints } from './components/igptype/igpigptype.endpoints';
import { SaleReturnService } from './components/salereturn/salereturn.service';
import { SaleReturnEndPoints } from './components/salereturn/salereturn.endpoints';
import { SaleReturnListComponent } from './components/salereturn/salereturn-list/salereturn-list.component';
import { SaleReturnTabComponent } from './components/salereturn/salereturn-tab/salereturn-tab.component';
import { AddSaleReturnComponent } from './components/salereturn/add-salereturn/add-salereturn.component';
import { DeleteSaleReturnComponent } from './components/salereturn/delete-salereturn/delete-salereturn.component';
import { ViewSaleReturnComponent } from './components/salereturn/view-salereturn/view-salereturn.component';
import { ProcessSaleReturnComponent } from './components/salereturn/process-salereturn/process-salereturn.component';
import { PrintSaleReturnComponent } from './components/salereturn/print-salereturn/print-salereturn.component';
import { ApproveSaleReturnComponent } from './components/salereturn/approve-salereturn/approve-salereturn.component';
import { PrintSRJVComponent } from './components/srjv/print-srjv/print-srjv.component';
import { ViewSRJVComponent } from './components/srjv/view-srjv/view-srjv.component';
import { SRJVListComponent } from './components/srjv/srjv-list/srjv-list.component';
import { SRJVTabComponent } from './components/srjv/srjv-tab/srjv-tab.component';
import { ViewEmployeeComponent } from './components/hr/employee/view-employee/view-employee.component';
import { AddEmployeeOvertimeRateComponent } from './components/hr/employee-overtimerate/add-employee-overtimerate/add-employee-overtimerate.component';
import { EmployeeOvertimeRateListComponent } from './components/hr/employee-overtimerate/employee-overtimerate-list/employee-overtimerate-list.component';
import { ViewEmployeeOvertimeRateComponent } from './components/hr/employee-overtimerate/view-employee-overtimerate/view-employee-overtimerate.component';
import { DeleteEmployeeOvertimeRateComponent } from './components/hr/employee-overtimerate/delete-employee-overtimerate/delete-employee-overtimerate.component';
import { EmployeeOvertimeRateService } from './components/hr/employee-overtimerate/employee-overtimerate.service';
import { EmployeeOvertimeRateEndPoints } from './components/hr/employee-overtimerate/employee-overtimerate.endpoints';
import { EditOrderComponent } from './components/order/primary-order/edit-order/edit-order.component';
import { AddShopOrderReturnComponent } from './components/shoporderreturn/add-shoporderreturn/add-shoporderreturn.component';
import { ShopOrderReturnEndPoints } from './components/shoporderreturn/shoporderreturn.endpoints';
import { ShopOrderReturnService } from './components/shoporderreturn/shoporderreturn.service';
import { ShopOrderReturnListComponent } from './components/shoporderreturn/shoporderreturn-list/shoporderreturn-list.component';
import { ShopOrderReturnTabComponent } from './components/shoporderreturn/shoporderreturn-tab/shoporderreturn-tab.component';
import { DeleteShopOrderReturnComponent } from './components/shoporderreturn/delete-shoporderreturn/delete-shoporderreturn.component';
import { ViewShopOrderReturnComponent } from './components/shoporderreturn/view-shoporderreturn/view-shoporderreturn.component';
import { ProcessShopOrderReturnComponent } from './components/shoporderreturn/process-shoporderreturn/process-shoporderreturn.component';
import { PrintShopOrderReturnComponent } from './components/shoporderreturn/print-shoporderreturn/print-shoporderreturn.component';
import { PurchaseReturnListComponent } from './components/purchasereturn/purchasereturn-list/purchasereturn-list.component';
import { PurchaseReturnTabComponent } from './components/purchasereturn/purchasereturn-tab/purchasereturn-tab.component';
import { AddPurchaseReturnComponent } from './components/purchasereturn/add-purchasereturn/add-purchasereturn.component';
import { DeletePurchaseReturnComponent } from './components/purchasereturn/delete-purchasereturn/delete-purchasereturn.component';
import { ViewPurchaseReturnComponent } from './components/purchasereturn/view-purchasereturn/view-purchasereturn.component';
import { ProcessPurchaseReturnComponent } from './components/purchasereturn/process-purchasereturn/process-purchasereturn.component';
import { PrintPurchaseReturnComponent } from './components/purchasereturn/print-purchasereturn/print-purchasereturn.component';
import { ApprovePurchaseReturnComponent } from './components/purchasereturn/approve-purchasereturn/approve-purchasereturn.component';
import { PurchaseReturnEndPoints } from './components/purchasereturn/purchasereturn.endpoints';
import { PurchaseReturnService } from './components/purchasereturn/purchasereturn.service';
import { AddWarehouseTransferComponent } from './components/warehousetransfer/add-warehousetransfer/add-warehousetransfer.component';
import { DeleteWarehouseTransferComponent } from './components/warehousetransfer/delete-warehousetransfer/delete-warehousetransfer.component';
import { WarehouseTransferListComponent } from './components/warehousetransfer/warehousetransfer-list/warehousetransfer-list.component';
import { WarehouseTransferTabComponent } from './components/warehousetransfer/warehousetransfer-tab/warehousetransfer-tab.component';
import { ApproveWarehouseTransferComponent } from './components/warehousetransfer/approve-warehousetransfer/approve-warehousetransfer.component';
import { ProcessWarehouseTransferComponent } from './components/warehousetransfer/process-warehousetransfer/process-warehousetransfer.component';
import { ViewWarehouseTransferComponent } from './components/warehousetransfer/view-warehousetransfer/view-warehousetransfer.component';
import { PrintWarehouseTransferComponent } from './components/warehousetransfer/print-warehousetransfer/print-warehousetransfer.component';
import { WarehouseTransferEndPoints } from './components/warehousetransfer/warehousetransfer.endpoints';
import { WarehouseTransferService } from './components/warehousetransfer/warehousetransfer.service';
import { AddDealershipUserComponent } from './components/order/dealership/add-user/add-dealershipuser.component';
import { EmployeeDeviceComponent } from './components/hr/employee-device/save-employee-device/employee-device.component';
import { EmployeeDeviceService } from './components/hr/employee-device/employee-device.service';
import { EmployeeDeviceEndPoints } from './components/hr/employee-device/employee-device.endpoints';
import { SaleMaterialReturnEndPoints } from './components/salematerialreturn/salematerialreturn.endpoints';
import { SaleMaterialReturnService } from './components/salematerialreturn/salematerialreturn.service';
import { AddSaleMaterialReturnComponent } from './components/salematerialreturn/add-salematerialreturn/add-salematerialreturn.component';
import { DeleteSaleMaterialReturnComponent } from './components/salematerialreturn/delete-salematerialreturn/delete-salematerialreturn.component';
import { ViewSaleMaterialReturnComponent } from './components/salematerialreturn/view-salematerialreturn/view-salematerialreturn.component';
import { SaleMaterialReturnTabComponent } from './components/salematerialreturn/salematerialreturn-tab/salematerialreturn-tab.component';
import { SaleMaterialReturnListComponent } from './components/salematerialreturn/salematerialreturn-list/salematerialreturn-list.component';
import { ProcessSaleMaterialReturnComponent } from './components/salematerialreturn/process-salematerialreturn/process-salematerialreturn.component';
import { ApproveSaleMaterialReturnComponent } from './components/salematerialreturn/approve-salematerialreturn/approve-salematerialreturn.component';
import { PrintSaleMaterialReturnComponent } from './components/salematerialreturn/print-salematerialreturn/print-salematerialreturn.component';
import { ApproveShopComponent } from './components/order/shop/approve-shop/approve-shop.component';
import { ShowUserAttendanceComponent } from './components/order/user-attendance/show-user-attendance/show-user-attendance.component';
import { AddLeaveTypeComponent } from './components/hr/employee-leave-group/add-leave-type/add-leave-type.component';
import { EmployeeLeaveEndPoints } from './components/hr/employee-leave/employee-leave.endpoints';
import { EmployeeLeaveService } from './components/hr/employee-leave/employee-leave.service';
import { AddEmployeeLeaveComponent } from './components/hr/employee-leave/add-employee-leave/add-employee-leave.component';
import { EmployeeLeaveListComponent } from './components/hr/employee-leave/employee-leave-list/employee-leave-list.component';
import { DeleteEmployeeLeaveComponent } from './components/hr/employee-leave/delete-employee-leave/delete-employee-leave.component';
import { ViewEmployeeLeaveComponent } from './components/hr/employee-leave/view-employee-leave/view-employee-leave.component';
import { AddHRYearComponent } from './components/hr/hryear/add-hryear/add-hryear.component';
import { DeleteHRYearComponent } from './components/hr/hryear/delete-hryear/delete-hryear.component';
import { ViewHRYearComponent } from './components/hr/hryear/view-hryear/view-hryear.component';
import { HRYearEndPoints } from './components/hr/hryear/hryear.endpoints';
import { HRYearService } from './components/hr/hryear/hryear.service';
import { HRYearListComponent } from './components/hr/hryear/hryear-list/hryear-list.component';
import { ProcessEmployeeLeaveComponent } from './components/hr/employee-leave/process-employee-leave/process-employee-leave.component';
import { AddManageEmployeeLeaveComponent } from './components/hr/manage-employee-leave/add-manage-employee-leave/add-manage-employee-leave.component';
import { DeleteManageEmployeeLeaveComponent } from './components/hr/manage-employee-leave/delete-manage-employee-leave/delete-manage-employee-leave.component';
import { ViewManageEmployeeLeaveComponent } from './components/hr/manage-employee-leave/view-manage-employee-leave/view-manage-employee-leave.component';
import { ManageEmployeeLeaveListComponent } from './components/hr/manage-employee-leave/manage-employee-leave-list/manage-employee-leave-list.component';
import { ProcessManageEmployeeLeaveComponent } from './components/hr/manage-employee-leave/process-manage-employee-leave/process-manage-employee-leave.component';
import { ApproveEmployeeLeaveListComponent } from './components/hr/approve-employee-leave/approve-employee-leave-list/approve-employee-leave-list.component';
import { ProcessApproveEmployeeLeaveComponent } from './components/hr/approve-employee-leave/process-approve-employee-leave/process-approve-employee-leave.component';
import { HrDashboardComponent } from './components/hr/dashboards/hr-dashboard/hr-dashboard.component';
import { EmployeeDashboardComponent } from './components/hr/dashboards/employee-dashboard/employee-dashboard.component';
import { DashboardEndPoints } from './components/hr/dashboards/dashboard.endpoints';
import { DashboardService } from './components/hr/dashboards/dashboard.service';
import { AddEmployeeWorkSiteTypeComponent } from './components/hr/employee-worksitetype/add-employee-worksitetype/add-employee-worksitetype.component';
import { DeleteEmployeeWorkSiteTypeComponent } from './components/hr/employee-worksitetype/delete-employee-worksitetype/delete-employee-worksitetype.component';
import { ViewEmployeeWorkSiteTypeComponent } from './components/hr/employee-worksitetype/view-employee-worksitetype/view-employee-worksitetype.component';
import { EmployeeWorkSiteTypeListComponent } from './components/hr/employee-worksitetype/employee-worksitetype-list/employee-worksitetype-list.component';
import { EmployeeWorkSiteTypeService } from './components/hr/employee-worksitetype/employee-worksitetype.service';
import { EmployeeWorkSiteTypeEndPoints } from './components/hr/employee-worksitetype/employee-worksitetype.endpoints';
import { AddInterviewComponent } from './components/interview/add-interview/add-interview.component';
import { InterviewListComponent } from './components/interview/interview-list/interview-list.component';
import { ViewInterviewComponent } from './components/interview/view-interview/view-interview.component';
import { DeleteInterviewComponent } from './components/interview/delete-interview/delete-interview.component';
import { InterviewService } from './components/interview/interview.service';
import { InterviewEndPoints } from './components/interview/interview.endpoints';
import { AddCommentsComponent } from './components/interview/add-comments/add-comments.component';
import { MatChipsModule } from '@angular/material/chips';
import { HasRoleDirective } from './components/Shared/has-role.directive';
import { ProcessPurchaseInvoiceComponent } from './components/purchaseinvoice/process-purchaseinvoice/process-purchaseinvoice.component';
import { RetailOrderEndPoints } from './components/order/retail-orders/retail-order.endpoints';
import { RetailOrderService } from './components/order/retail-orders/retail-order.service';
import { CreateRetailOrderComponent } from './components/order/retail-orders/create-retail-order/create-retail-order.component';
import { PrintRetailOrderReceiptComponent } from './components/order/retail-orders/print-retail-order-receipt/print-retail-order-receipt.component';
import { ConfirmRetailOrderQuantityComponent } from './components/order/retail-orders/confirm-retail-order-quantity/confirm-retail-order-quantity.component';
import { RetailOrderHistoryComponent } from './components/order/retail-orders/retail-order-history/retail-order-history.component';
import { ViewRetailOrderComponent } from './components/order/retail-orders/view-retail-orders/view-retail-order.component';
import { DeleteRetailOrderComponent } from './components/order/retail-orders/delete-retail-order/delete-retail-order.component';
import { RetailOrderStatusChangeComponent } from './components/order/retail-orders/retail-order-status-change/retail-order-status-change.component';
import { RetailOrderListComponent } from './components/order/retail-orders/retail-order-list/retail-order-list.component';
import { DeviceAttendanceEndPoints } from './components/hr/device-attendance/device-attendance.endpoints';
import { DeviceAttendanceService } from './components/hr/device-attendance/device-attendance.service';
import { EmployeeEndPoints } from './components/hr/employee/employee.endpoints';
import { EmployeeService } from './components/hr/employee/employee.service';
import { ViewHolidayComponent } from './components/hr/holiday/view-holiday/view-holiday.component';
import { AddHolidayComponent } from './components/hr/holiday/add-holiday/add-holiday.component';
import { DeleteHolidayComponent } from './components/hr/holiday/delete-holiday/delete-holiday.component';
import { HolidayListComponent } from './components/hr/holiday/holiday-list/holiday-list.component';
import { HolidayService } from './components/hr/holiday/holiday.service';
import { HolidayEndPoints } from './components/hr/holiday/holiday.endpoints';
import { UpdateUserAttendanceComponent } from './components/order/user-attendance/update-user-attendance/update-user-attendance.component';
import { RegisterMobileDeviceComponent } from './components/hr/register-mobile-device/register-mobile-device.component';
import { AddRetailOrderReturnComponent } from './components/order/retail-orders/retail-order-return/add-retail-order-return/add-retail-order-return.component';
import { DeleteRetailOrderReturnComponent } from './components/order/retail-orders/retail-order-return/delete-retail-order-return/delete-retail-order-return.component';
import { PrintRetailOrderReturnComponent } from './components/order/retail-orders/retail-order-return/print-retail-order-return/print-retail-order-return.component';
import { ProcessRetailOrderReturnComponent } from './components/order/retail-orders/retail-order-return/process-retail-order-return/process-retail-order-return.component';
import { RetailOrderReturnListComponent } from './components/order/retail-orders/retail-order-return/retail-order-return-list/retail-order-return-list.component';
import { RetailOrderReturnTabComponent } from './components/order/retail-orders/retail-order-return/retail-order-return-tab/retail-order-return-tab.component';
import { RetailOrderReturnEndPoints } from './components/order/retail-orders/retail-order-return/retail-order-return.endpoints';
import { RetailOrderReturnService } from './components/order/retail-orders/retail-order-return/retail-order-return.service';
import { ViewRetailOrderReturnComponent } from './components/order/retail-orders/retail-order-return/view-retail-order-return/view-retail-order-return.component';
import { ModernSidebarComponent } from './components/modern-sidebar.component/modern-sidebar.component';
import { AddAppointmentComponent } from './components/opd/appointment/add-appointment/add-appointment.component';
import { AppointmentService } from './components/opd/appointment/appointment.service';
import { AppointmentEndPoints } from './components/opd/appointment/appointment.endpoints';
import { AppointmentListComponent } from './components/opd/appointment/appointment-list/appointment-list.component';
import { AppointmentTypeService } from './components/opd/appointment-type/appointment-type.service';
import { AppointmentTypeEndPoints } from './components/opd/appointment-type/appointment-type.endpoints';
import { AppointmentTypeListComponent } from './components/opd/appointment-type/appointment-type-list/appointment-type-list.component';
import { DeleteAppointmentTypeComponent } from './components/opd/appointment-type/delete-appointment-type/delete-appointment-type.component';
import { ViewAppointmentTypeComponent } from './components/opd/appointment-type/view-appointment-type/view-appointment-type.component';
import { CreateAppointmentTypeComponent } from './components/opd/appointment-type/create-appointment-type/create-appointment-type.component';
import { VisitTypeListComponent } from './components/opd/visit-type/visit-type-list/visit-type-list.component';
import { ViewVisitTypeComponent } from './components/opd/visit-type/view-visit-type/view-visit-type.component';
import { CreateVisitTypeComponent } from './components/opd/visit-type/create-visit-type/create-visit-type.component';
import { DeleteVisitTypeComponent } from './components/opd/visit-type/delete-visit-type/delete-visit-type.component';
import { VisitTypeService } from './components/opd/visit-type/visit-type.service';
import { VisitTypeEndPoints } from './components/opd/visit-type/visit-type.endpoints';
import { PriorityLevelService } from './components/opd/prioritylevel/prioritylevel.service';
import { PriorityLevelEndPoints } from './components/opd/prioritylevel/prioritylevel.endpoints';
import { PatientEndPoints } from './components/opd/patient/patient.endpoints';
import { PatientService } from './components/opd/patient/patient.service';
import { PatientListComponent } from './components/opd/patient/patient-list/patient-list.component';
import { ViewPatientComponent } from './components/opd/patient/view-patient/view-patient.component';
import { DoctorEndPoints } from './components/opd/doctor/doctor.endpoints';
import { DoctorService } from './components/opd/doctor/doctor.service';
import { DoctorListComponent } from './components/opd/doctor/doctor-list/doctor-list.component';

const matFormFieldDefaults: MatFormFieldDefaultOptions = {
    appearance: 'outline'
    , subscriptSizing: 'dynamic'
};

@NgModule({
    declarations: [
        ChartitemsComponent,
        LoginLayoutComponent,
        ReportViewerComponent,
        HomeLayoutComponent,
        SidemenuComponent,
        ModernSidebarComponent,
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
        ApproveIGPComponent,
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
        CpvTabComponent,
        GmapviewerComponent,
        ZoneListComponent,
        ViewZoneComponent,
        CreateZoneComponent,
        DeleteZoneComponent,
        DrawMapComponent,
        TerritoryListComponent,
        ViewTerritoryComponent,
        CreateTerritoryComponent,
        DeleteTerritoryComponent,
        DealershipListComponent,
        DeleteDealershipComponent,
        CreateDealershipComponent,
        ViewDealershipComponent,
        CreateShopComponent,
        ViewShopComponent,
        ShopListComponent,
        DeleteShopComponent,
        RouteListComponent,
        CreateRouteComponent,
        DeleteRouteComponent,
        ViewRouteComponent,
        AddShopsRouteComponent,
        FieldMapComponent,
        DrawRouteShopsComponent,
        PredFieldMapComponent,
        RoleListComponent,
        CreateRegionComponent,
        ViewRegionComponent,
        RegionListComponent,
        DeleteRegionComponent,
        CreateAreaComponent,
        ViewAreaComponent,
        AreaListComponent,
        DeleteAreaComponent,
        CreateShopTypeComponent,
        ViewShopTypeComponent,
        ShopTypeListComponent,
        DeleteShopTypeComponent,
        AddShopsRouteFrequencyComponent,
        ApproveShopComponent,
        AccountListComponent,
        DeleteAccountComponent,
        ViewAccountComponent,
        AddInspectionComponent,
        DeleteInspectionComponent,
        ViewInspectionComponent,
        ProcessInspectionComponent,
        PrintInspectionComponent,
        InspectionListComponent,
        InspectionTabComponent,
        ApproveInspectionComponent,
        AddRejectReasonComponent,
        DeleteRejectReasonComponent,
        ViewRejectReasonComponent,
        RejectReasonListComponent,
        AddDSFRouteComponent,
        DSFListComponent,
        ViewDSFComponent,
        CreateDistributorPriceGroupComponent,
        CreatePricingGroupComponent,
        CreatePricingGroupDetailsComponent,
        PricingGroupListComponent,
        VehicleListComponent,
        ViewVehicleComponent,
        DeleteVehicleComponent,
        CreateOrderComponent,
        CreateOrderWithPGComponent,
        DeleteOrderComponent,
        OrderHistoryComponent,
        OrderListComponent,
        OrderStatusChangeComponent,
        ViewOrderComponent,
        ViewUserattendanceComponent,
        UserAttendanceListComponent,
        ShowUserAttendanceComponent,
        CreateVehicleComponent,
        UserTerritoryListComponent,
        ViewUserTerritoryComponent,
        CreateUserTerritoryComponent,
        DeleteUserTerritoryComponent,
        TemplateListComponent,
        AddTemplateComponent,
        AddGRNComponent,
        DeleteGRNComponent,
        ViewGRNComponent,
        ProcessGRNComponent,
        ApproveGRNComponent,
        GRNListComponent,
        GRNTabComponent,
        PrintGRNComponent,
        ConfirmRetailOrderQuantityComponent,
        CreateRetailOrderComponent,
        DeleteRetailOrderComponent,
        PrintRetailOrderReceiptComponent,
        RetailOrderHistoryComponent,
        RetailOrderListComponent,
        RetailOrderStatusChangeComponent,
        ViewRetailOrderComponent,
        AddAccountFlowComponent,
        DeleteAccountFlowComponent,
        ViewAccountFlowComponent,
        AccountFlowListComponent,
        RackListComponent,
        AddRackComponent,
        DeleteRackComponent,
        ViewRackComponent,
        RowListComponent,
        AddRowComponent,
        ViewRowComponent,
        DeleteRowComponent,
        SectionListComponent,
        AddSectionComponent,
        ViewSectionComponent,
        DeleteSectionComponent,
        AccountFlowListComponent,
        ApproveAuditReviewComponent,
        AuditReviewTabComponent,
        AuditReviewListComponent,
        DeleteAuditReviewComponent,
        ProcessAuditReviewComponent,
        PrintAuditReviewComponent,
        ViewAuditReviewComponent,
        AddAccountGroupComponent,
        AccountGroupListComponent,
        DeleteAccountGroupComponent,
        ViewAccountGroupComponent,
        CreateDSFTargetComponent,
        CreateSalesTargetComponent,
        CreateTerritoryTargetComponent,
        CreateZoneTargetComponent,
        DeleteSalesTargetComponent,
        SalesTargetListComponent,
        ViewSalesTargetComponent,
        AddIssuanceComponent,
        ApproveIssuanceComponent,
        DeleteIssuanceComponent,
        IssuanceListComponent,
        IssuanceTabComponent,
        PrintIssuanceComponent,
        ProcessIssuanceComponent,
        ViewIssuanceComponent,
        AddDispatchComponent,
        DeleteDispatchComponent,
        ViewDispatchComponent,
        ProcessDispatchComponent,
        ApproveDispatchComponent,
        DispatchTabComponent,
        DispatchListComponent,
        CancelDispatchTabComponent,
        CancelDispatchListComponent,
        AddCancelDispatchComponent,
        PrintDispatchOrdersPopupComponent,
        PrintDispatchOrderReceiptComponent,
        AddSaleUserComponent,
        SaleUsersListComponent,
        ReceiveDispatchComponent,
        PrintOrderReceiptComponent,
        PendingDispatchOrderListComponent,
        SJVListComponent,
        SJVTabComponent,
        ViewSJVComponent,
        PrintDispatchOrderInvoiceComponent,
        ResetPasswordComponent,
        AddCostSheetComponent,
        CostsheetTabComponent,
        CostsheetListComponent,
        ProcessCostSheetComponent,
        DeleteCostSheetComponent,
        ViewCostSheetComponent,
        ApproveCostSheetComponent,
        PrintCostSheetComponent,
        ProcessCancelDispatchComponent,
        ViewCancelDispatchComponent,
        DeleteCancelDispatchComponent,
        RejectCancelDispatchComponent,
        CancelDispatchHistoryComponent,
        CancelDispatchHistoryComponent,
        PJVTabComponent,
        PJVListComponent,
        ViewPJVComponent,
        PrintPJVComponent,
        PrintSJVComponent,
        AddSaleMaterialComponent,
        SaleMaterialTabComponent,
        SaleMaterialListComponent,
        ProcessSaleMaterialComponent,
        DeleteSaleMaterialComponent,
        ViewSaleMaterialComponent,
        ApproveSaleMaterialComponent,
        PrintSaleMaterialComponent,
        CreateCustomerComponent,
        ViewCustomerComponent,
        DeleteCustomerComponent,
        CustomerListComponent,
        ApprovePurchaseInvoiceComponent,
        PurchaseInvoiceListComponent,
        PurchaseInvoiceTabComponent,
        ViewPurchaseInvoiceComponent,
        PrintPurchaseInvoiceComponent,
        DocumentViewerComponent,
        AddEmployeeDesignationComponent,
        ViewEmployeeDesignationComponent,
        EmployeeDesignationListComponent,
        DeleteEmployeeDesignationComponent,
        AddEmployeeEducationComponent,
        ViewEmployeeEducationComponent,
        EmployeeEducationListComponent,
        DeleteEmployeeEducationComponent,
        AddEmployeeGradeComponent,
        ViewEmployeeGradeComponent,
        EmployeeGradeListComponent,
        DeleteEmployeeGradeComponent,
        AddEmployeeShiftComponent,
        ViewEmployeeShiftComponent,
        EmployeeShiftListComponent,
        DeleteEmployeeShiftComponent,
        AddEmployeeTypeComponent,
        ViewEmployeeTypeComponent,
        EmployeeTypeListComponent,
        DeleteEmployeeTypeComponent,
        AddEmployeeComponent,
        EmployeeListComponent,
        ViewEmployeeComponent,
        EmployeeBankListComponent,
        AddEmployeeBankComponent,
        ViewEmployeeBankComponent,
        DeleteEmployeeBankComponent,
        EmployeeLeaveGroupListComponent,
        AddEmployeeLeaveGroupComponent,
        ViewEmployeeLeaveGroupComponent,
        DeleteEmployeeLeaveGroupComponent,
        EmployeeLeaveTypeListComponent,
        AddEmployeeLeaveTypeComponent,
        ViewEmployeeLeaveTypeComponent,
        DeleteEmployeeLeaveTypeComponent,
        AddEmployeeDocumentTypeComponent,
        EmployeeDocumentTypeListComponent,
        ViewEmployeeDocumentTypeComponent,
        DeleteEmployeeDocumentTypeComponent,
        AddCityComponent,
        CityListComponent,
        ViewCityComponent,
        DeleteCityComponent,
        IJVListComponent,
        IJVTabComponent,
        ViewIJVComponent,
        PrintIJVComponent,
        CreateDeviceComponent,
        DeviceListComponent,
        DeleteDeviceComponent,
        ViewDeviceComponent,
        CancelPurchaseOrderComponent,
        CopyPricingGroupDetailsComponent,
        SaleReturnListComponent,
        SaleReturnTabComponent,
        AddSaleReturnComponent,
        DeleteSaleReturnComponent,
        ViewSaleReturnComponent,
        ProcessSaleReturnComponent,
        PrintSaleReturnComponent,
        ApproveSaleReturnComponent,
        PrintSRJVComponent,
        ViewSRJVComponent,
        SRJVListComponent,
        SRJVTabComponent,
        AddEmployeeOvertimeRateComponent,
        EmployeeOvertimeRateListComponent,
        ViewEmployeeOvertimeRateComponent,
        DeleteEmployeeOvertimeRateComponent,
        EditOrderComponent,
        AddShopOrderReturnComponent,
        ShopOrderReturnListComponent,
        ShopOrderReturnTabComponent,
        DeleteShopOrderReturnComponent,
        ViewShopOrderReturnComponent,
        ProcessShopOrderReturnComponent,
        PrintShopOrderReturnComponent,
        PurchaseReturnListComponent,
        PurchaseReturnTabComponent,
        AddPurchaseReturnComponent,
        DeletePurchaseReturnComponent,
        ViewPurchaseReturnComponent,
        ProcessPurchaseReturnComponent,
        PrintPurchaseReturnComponent,
        ApprovePurchaseReturnComponent,
        AddWarehouseTransferComponent,
        DeleteWarehouseTransferComponent,
        WarehouseTransferListComponent,
        WarehouseTransferTabComponent,
        ApproveWarehouseTransferComponent,
        ProcessWarehouseTransferComponent,
        ViewWarehouseTransferComponent,
        PrintWarehouseTransferComponent,
        AddDealershipUserComponent,
        EmployeeDeviceComponent,
        AddSaleMaterialReturnComponent,
        DeleteSaleMaterialReturnComponent,
        ViewSaleMaterialReturnComponent,
        SaleMaterialReturnTabComponent,
        SaleMaterialReturnListComponent,
        ProcessSaleMaterialReturnComponent,
        ApproveSaleMaterialReturnComponent,
        PrintSaleMaterialReturnComponent,
        AddLeaveTypeComponent,
        AddEmployeeLeaveComponent,
        EmployeeLeaveListComponent,
        DeleteEmployeeLeaveComponent,
        ViewEmployeeLeaveComponent,
        AddHRYearComponent,
        DeleteHRYearComponent,
        HRYearListComponent,
        ViewHRYearComponent,
        ProcessEmployeeLeaveComponent,
        AddManageEmployeeLeaveComponent,
        DeleteManageEmployeeLeaveComponent,
        ViewManageEmployeeLeaveComponent,
        ManageEmployeeLeaveListComponent,
        ProcessManageEmployeeLeaveComponent,
        ApproveEmployeeLeaveListComponent,
        ProcessApproveEmployeeLeaveComponent,
        ProcessManageEmployeeLeaveComponent,
        HrDashboardComponent,
        EmployeeDashboardComponent,
        AddEmployeeWorkSiteTypeComponent,
        DeleteEmployeeWorkSiteTypeComponent,
        ViewEmployeeWorkSiteTypeComponent,
        EmployeeWorkSiteTypeListComponent,
        AddInterviewComponent,
        InterviewListComponent,
        ViewInterviewComponent,
        DeleteInterviewComponent,
        AddCommentsComponent,
        ProcessPurchaseInvoiceComponent,
        ViewHolidayComponent,
        AddHolidayComponent,
        DeleteHolidayComponent,
        HolidayListComponent,
        UpdateUserAttendanceComponent,
        RegisterMobileDeviceComponent,
        AddRetailOrderReturnComponent,
        RetailOrderReturnListComponent,
        RetailOrderReturnTabComponent,
        DeleteRetailOrderReturnComponent,
        ViewRetailOrderReturnComponent,
        ProcessRetailOrderReturnComponent,
        PrintRetailOrderReturnComponent,
        AddAppointmentComponent,
        AppointmentListComponent,
        AppointmentTypeListComponent,
        CreateAppointmentTypeComponent,
        DeleteAppointmentTypeComponent,
        ViewAppointmentTypeComponent,
        VisitTypeListComponent,
        ViewVisitTypeComponent,
        CreateVisitTypeComponent,
        DeleteVisitTypeComponent,
        VisitTypeListComponent,
        PatientListComponent,
        ViewPatientComponent,
        DoctorListComponent
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
        MatRadioModule,
        SafeHtml,
        MatTimepickerModule,
        MatDatepickerModule,
        NumberToWordsPipe,
        MatChipsModule,
        HasRoleDirective,
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
        UserService,
        UserEndPoints,
        GmapEndPoints,
        GmapService,
        ZoneService,
        ZoneEndPoints,
        TerritoryService,
        TerritoryEndPoints,
        DealershipService,
        DealershipEndPoints,
        ShopEndPoints,
        ShopService,
        RouteService,
        RouteEndPoints,
        RegionEndPoints,
        VehicleService,
        VehicleEndPoints,
        UserAttendanceEndPoints,
        UserAttendanceService,
        RegionService,
        AreaEndPoints,
        AreaService,
        ShopTypeEndPoints,
        ShopTypeService,
        AccountService,
        InspectionService,
        InspectionEndPoints,
        RejectReasonEndPoints,
        RejectReasonService,
        DSFEndPoints,
        DSFService,
        PricingGroupEndPoints,
        UserTerritoryEndPoints,
        UserTerritoryService,
        PricingGroupService,
        PrimaryOrderService,
        PrimaryOrderEndPoints,
        TemplateService,
        TemplateEndPoints,
        GRNService,
        GRNEndPoints,
        RackEndPoints,
        RackService,
        RowEndPoints,
        RowService,
        SectionEndPoints,
        SectionService,
        AuditReviewService,
        AuditReviewEndPoints,
        AccountGroupEndPoints,
        AccountGroupService,
        SalesTargetService,
        SalesTargetEndPoints,
        IssuanceService,
        IssuanceEndPoints,
        DispatchEndPoints,
        DispatchService,
        CancelDispatchService,
        CancelDispatchEndPoints,
        LedgerService,
        LedgerEndPoints,
        CostSheetService,
        CostSheetEndPoints,
        SaleMaterialService,
        SaleMaterialEndPoints,
        EmployeeDesignationService,
        EmployeeDesignationEndPoints,
        EmployeeEducationService,
        EmployeeEducationEndPoints,
        EmployeeGradeService,
        EmployeeGradeEndPoints,
        EmployeeShiftService,
        EmployeeShiftEndPoints,
        EmployeeTypeService,
        EmployeeTypeEndPoints,
        EmployeeBankService,
        EmployeeBankEndPoints,
        EmployeeLeaveGroupEndPoints,
        EmployeeLeaveGroupService,
        EmployeeLeaveTypeEndPoints,
        EmployeeLeaveTypeService,
        EmployeeDocumentTypeEndPoints,
        EmployeeDocumentTypeService,
        CityEndPoints,
        CityService,
        DeviceEndPoints,
        DeviceService,
        IGPTypeService,
        IGPTypeEndPoints,
        SaleReturnService,
        SaleReturnEndPoints,
        EmployeeOvertimeRateService,
        EmployeeOvertimeRateEndPoints,
        ShopOrderReturnEndPoints,
        ShopOrderReturnService,
        PurchaseReturnEndPoints,
        PurchaseReturnService,
        WarehouseTransferEndPoints,
        WarehouseTransferService,
        EmployeeDeviceService,
        EmployeeDeviceEndPoints,
        SaleMaterialReturnEndPoints,
        SaleMaterialReturnService,
        EmployeeLeaveEndPoints,
        EmployeeLeaveService,
        HRYearEndPoints,
        HRYearService,
        DashboardEndPoints,
        DashboardService,
        EmployeeWorkSiteTypeService,
        EmployeeWorkSiteTypeEndPoints,
        InterviewService,
        InterviewEndPoints,
        RetailOrderEndPoints,
        RetailOrderService,
        DeviceAttendanceEndPoints,
        DeviceAttendanceService,
        EmployeeService,
        EmployeeEndPoints,
        HolidayService,
        HolidayEndPoints,
        RetailOrderReturnEndPoints,
        RetailOrderReturnService,
        AppointmentService,
        AppointmentEndPoints,
        AppointmentTypeService,
        AppointmentTypeEndPoints,
        VisitTypeService,
        VisitTypeEndPoints,
        PriorityLevelService,
        PriorityLevelEndPoints,
        PatientEndPoints,
        PatientService,
        DoctorEndPoints,
        DoctorService,
        { provide: JWT_OPTIONS, useValue: JWT_OPTIONS },
        { provide: HTTP_INTERCEPTORS, useClass: LoaderInterceptor, multi: true },
        AuthenticationService, {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthInterceptor,
            multi: true
        },
        { provide: MAT_FORM_FIELD_DEFAULT_OPTIONS, useValue: matFormFieldDefaults },
        LoaderService,
        DatePipe,
        provideHttpClient(withInterceptorsFromDi())
    ]
})

export class AppModule {}
