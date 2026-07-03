import { BreakpointObserver } from '@angular/cdk/layout';
import { Component, EventEmitter, Input, Output, ViewChild, OnDestroy, HostListener, ViewChildren, QueryList, ElementRef } from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';
import { Subject, filter, takeUntil } from 'rxjs';
import { NavigationEnd, Router, Event, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from '../../Auth/authentication.service';
import { Location } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { ResetPasswordComponent } from '../../Auth/reset-password/reset-password.component';
import { environment } from '../../../environments/environment';
import { UserService } from '../user-management/user.service';
import { ViewEmployeeComponent } from '../hr/employee/view-employee/view-employee.component';
import { UserAttendanceService } from '../order/user-attendance/user-attendance.service';
import { ShowUserAttendanceComponent } from '../order/user-attendance/show-user-attendance/show-user-attendance.component';

@Component({
  selector: 'app-sidemenu',
  templateUrl: './sidemenu.component.html',
  styleUrls: ['./sidemenu.component.css'],
  standalone: false
})
export class SidemenuComponent implements OnDestroy {
   title = environment.production ? '(Live Server)' : '(Testing Server)';
  isEstimator = false;
  isLoading = false;
  ticketData: any;
    isSidebarOpen = true;
  @Input() isExpanded: boolean = false;
  @ViewChild(MatSidenav)
  sidenav!: MatSidenav;
  location: Location;
  roleList!: any;
  warehouseList: any;
  currentUser: any;
  openSubMenus: Set<string> = new Set();
  reportsUrl: any;
  fileSource: any;
  selectedWarehouseId: any;
  profile: any;
  breadcrumbCurrent : any;
  isWarehouseDropdownOpen = false;
  isMobile = false;

  constructor(private dialog: MatDialog, location: Location,
    private router: Router,
    private authenticationService: AuthenticationService,
    private userService: UserService,
    private breakpointObserver: BreakpointObserver,
    private route: ActivatedRoute) {
    this.location = location;
    this.reportsUrl = environment.reports_uri;
  }



  private ngUnsubscribe = new Subject<void>();

  ngOnInit(): void {
    this.currentUser = this.authenticationService.currentUserValue;
    this.profile = JSON.parse(localStorage.getItem('profile') || 'null');
    this.roleList = this.currentUser.role
      .split(',')
      .map((role: string) => role.trim().toLowerCase())
      .filter((role: string) => role !== '');
    if (this.profile?.isEmployee) {
      this.fileSource = this.profile.attachments[0]?.imageName;
    }
    // Initialize warehouseList from userProject
    this.warehouseList = this.currentUser.userProject.map((p: any) => ({
      id: p.projectId,
      name: p.project?.name?.trim()
    })).filter((item: { id: any; name: any; }) => item.id && item.name);

    if (this.currentUser.selectedWarehouseId == 0) {
      // Load from localStorage or set to first item
      if (this.warehouseList.length > 0) {
        this.selectedWarehouseId = this.warehouseList[0].id;
      }
    }
    else {
      this.selectedWarehouseId = this.currentUser.selectedWarehouseId;
    }

    this.router.events
  .pipe(filter(event => event instanceof NavigationEnd))
  .subscribe(() => {
    let route = this.route;

    while (route.firstChild) {
      route = route.firstChild;
    }

    route.data.subscribe(data => {
      this.breadcrumbCurrent = data['breadcrumb'] || 'Dashboard';
    });
  });

  }

  checkScreenSize() {
  this.isMobile = window.innerWidth <= 800;
}

  onWarehouseSelect(selectedId: any) {
    const currentUser = JSON.parse(localStorage.getItem("currentUser") || '{}');
    currentUser.selectedWarehouseId = selectedId;
    this.selectedWarehouseId = selectedId;
    localStorage.setItem("currentUser", JSON.stringify(currentUser));
    
    // Close dropdown
    this.isWarehouseDropdownOpen = false;
    
    this.authenticationService.updateSelectedWarehouse(selectedId).subscribe({
      next: (response: any) => {
        if (response?.token) {
          const currentUserupdate = JSON.parse(localStorage.getItem("currentUser") || '{}');
          currentUserupdate.token = response.token;
          localStorage.setItem("currentUser", JSON.stringify(currentUserupdate));
          this.authenticationService.updateToken(response.token);
        }
        const currentUrl = this.router.url;
        this.router.navigateByUrl('/refresh', { skipLocationChange: true }).then(() => {
          this.router.navigate([currentUrl]);
        });
      },
      error: (err) => {
        console.error('Failed to update warehouse on server:', err);
      }
    });
  }

ngAfterViewInit() {
  this.breakpointObserver
    .observe(['(max-width: 800px)'])
    .subscribe(result => {
      this.isMobile = result.matches;

      if (result.matches) {
        this.isSidebarOpen = false; // ✅ close sidebar on mobile
        this.openSubMenus.clear();
      } else {
        this.isSidebarOpen = true; // optional default desktop open
      }
    });
}
  
toggleSidebar() {
  this.isSidebarOpen = !this.isSidebarOpen;

  if (!this.isSidebarOpen) {
    this.openSubMenus.clear();
  }
}

  toggleSubMenu(menuKey: string): void {
    if (!this.isSidebarOpen) {
      this.isSidebarOpen = true;
      this.openSubMenus.add(menuKey);
      return;
    }
    if (this.openSubMenus.has(menuKey)) {
      this.openSubMenus.delete(menuKey);
    } else {
      this.openSubMenus.add(menuKey);
    }
  }

  isSubMenuVisible(menuKey: string): boolean {
    return this.openSubMenus.has(menuKey);
  }

  toggleWarehouseDropdown(): void {
    this.isWarehouseDropdownOpen = !this.isWarehouseDropdownOpen;
  }

   isActive(routePath: string): boolean {
     return this.router.url === routePath;
   }

   get selectedWarehouseName(): string {
     const warehouse = this.warehouseList.find((w: { id: any; }) => w.id === this.selectedWarehouseId);
     return warehouse?.name || 'Building/Site';
   }

   get companyTooltip(): string {
     const company = this.currentUser?.department?.company?.name;
     return company ? `HMS — ${company}` : 'HMS System';
   }

   get userTooltip(): string {
     if (!this.currentUser) {
       return '';
     }
     const name = `${this.currentUser.firstName || ''} ${this.currentUser.lastName || ''}`.trim();
     const dept = this.currentUser.department?.name;
     return dept ? `${name} · ${dept}` : name;
   }

  hasRequiredRole(requiredRoles: string[]): boolean {
    return requiredRoles.some(role => this.roleList.includes(role.toLowerCase()));
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const clickedElement = event.target as HTMLElement;
    if (!clickedElement.closest('.menu-item-container')) {
      // Optional: close submenus on outside click
      // this.openSubMenus.clear();
    }
  }


  logout() {
    this.authenticationService.logout();
    window.location.href = '/login';
  }

  viewEmployeeDialog() {
    const dialogRef = this.dialog.open(ViewEmployeeComponent, {
      width: '60%',
      height: 'auto',
      maxHeight: '95vh',
      data: {
        element: this.profile,
      },
      disableClose: true
    });
  }

  viewAttendanceDialog() {
    const dialogRef = this.dialog.open(ShowUserAttendanceComponent, {
      width: '60%',
      height: 'auto',
      maxHeight: '95vh',
      disableClose: true
    });
  }


  openResetDialog(element: any) {
    const dialogRef = this.dialog.open(ResetPasswordComponent, {
      panelClass: 'cstm_width_800',
      height: 'auto',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
    });
  }


  ngOnDestroy() {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }


  goToDashboard() {
    if (this.roleList.includes('hr manager')) {
      window.location.href = '/hrdashboard';
    } else if (this.roleList.includes('employee')) {
      window.location.href = '/employeedashboard';
    } else {
      window.location.href = '/home';
    }
  }

  redirectTochartofaccount() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FChartOfAccount&rs%3AClearSession=true&rc%3AView=955b82da-9d4c-41a3-8fd2-995b91d5efd8&CompanyId=' + this.currentUser.department.companyId;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  redirectTotrailbalance() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FTrialBalance&rs%3AClearSession=true&rc%3AView=955b82da-9d4c-41a3-8fd2-995b91d5efd8&CompanyId=' + this.currentUser.department.companyId;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  redirectToaccountledger() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FAccountLedger&rs%3AClearSession=true&rc%3AView=955b82da-9d4c-41a3-8fd2-995b91d5efd8&CompanyId=' + this.currentUser.department.companyId;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  redirectToaccounttypeledger() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FAccountTypeLedger&rs%3AClearSession=true&rc%3AView=955b82da-9d4c-41a3-8fd2-995b91d5efd8&CompanyId=' + this.currentUser.department.companyId;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  redirectTochartofitem() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FChartOfItem&rs%3AClearSession=true&rc%3AView=955b82da-9d4c-41a3-8fd2-995b91d5efd8&CompanyId=' + this.currentUser.department.companyId;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  redirectToitemwisestock() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FItemWiseStock&rs%3AClearSession=true&rc%3AView=955b82da-9d4c-41a3-8fd2-995b91d5efd8&CompanyId=' + this.currentUser.department.companyId;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  redirectToUserAttendance() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FUserAttendance&rs%3AClearSession=true&rc%3AView=65c43b2e-ba44-4e45-9763-d7a13a79f718';
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  redirectToitemledgerdistributor() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FItemLedgerDistributor&rs%3AClearSession=true&rc%3AView=955b82da-9d4c-41a3-8fd2-995b91d5efd8&CompanyId=' + this.currentUser.department.companyId + '&DistributorId=' + 7;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

    redirectToitemledgershop() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FItemLedgerShop&rs%3AClearSession=true&rc%3AView=955b82da-9d4c-41a3-8fd2-995b91d5efd8&CompanyId=' + this.currentUser.department.companyId + '&ShopId=' + this.currentUser.retailUserShopId;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  redirectTobanksummary() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FBankSummary&rs%3AClearSession=true&rc%3AView=955b82da-9d4c-41a3-8fd2-995b91d5efd8&CompanyId=' + this.currentUser.department.companyId;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  redirectTodispatchdetail() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FDispatchOrderDetail&rs%3AClearSession=true&rc%3AView=955b82da-9d4c-41a3-8fd2-995b91d5efd8&CompanyId=' + this.currentUser.department.companyId;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  redirectTosaleSummary() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FSaleSummary&rs%3AClearSession=true&rc%3AView=3b59db1c-44af-4a00-ab80-735021507bd8&CompanyId=' + this.currentUser.department.companyId + '&ShopId=' + this.currentUser.retailUserShopId;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  redirectToorderdetail() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FOrderDetail&rs%3AClearSession=true&rc%3AView=ade36e85-3e78-4627-a766-518e19f540f9&CompanyId=' + this.currentUser.department.companyId;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  redirectTostockbalance() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FStockBalance&rs%3AClearSession=true&rc%3AView=ade36e85-3e78-4627-a766-518e19f540f9&CompanyId=' + this.currentUser.department.companyId;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  redirectToorderdetailSupplyChain() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FDispatchOrderSupplyChain&rs%3AClearSession=true&rc%3AView=ade36e85-3e78-4627-a766-518e19f540f9&CompanyId=' + this.currentUser.department.companyId;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  redirectTostockbalancebyvalue() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FStockBalanceByValue&rs%3AClearSession=true&rc%3AView=ade36e85-3e78-4627-a766-518e19f540f9&CompanyId=' + this.currentUser.department.companyId;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  redirectToshopdetail() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FShopDetail&rs%3AClearSession=true&rc%3AView=af7578f5-cebd-4b18-ac9f-43c11e11f867&CompanyId=' + this.currentUser.department.companyId;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  redirectTodistributorbalance() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FDistributorBalance&rs%3AClearSession=true&rc%3AView=af7578f5-cebd-4b18-ac9f-43c11e11f867&CompanyId=' + this.currentUser.department.companyId;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  redirectTosaledetail() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FSaleDetail&rs%3AClearSession=true&rc%3AView=af7578f5-cebd-4b18-ac9f-43c11e11f867&CompanyId=' + this.currentUser.department.companyId + '&ShopId=' + this.currentUser.retailUserShopId;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  redirectToissuancereport() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FIssuanceRequest&rs%3AClearSession=true&rc%3AView=af7578f5-cebd-4b18-ac9f-43c11e11f867&CompanyId=' + this.currentUser.department.companyId;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  redirectTomaterialissuance() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FMaterialIssuance&rs%3AClearSession=true&rc%3AView=af7578f5-cebd-4b18-ac9f-43c11e11f867&CompanyId=' + this.currentUser.department.companyId;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  redirectToAttendanceHr() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FUserAttendanceHR&rs%3AClearSession=true&rc%3AView=af7578f5-cebd-4b18-ac9f-43c11e11f867';
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  redirectToAttendanceRegister() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FUserAttendanceRegister&rs%3AClearSession=true&rc%3AView=af7578f5-cebd-4b18-ac9f-43c11e11f867';
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  redirectToAttendanceDepartment() {
    // Step 1: Get the department ID
    const departmentId = this.currentUser.department.id;
    // Step 2: Base64 encode the department ID
    const encodedDepartment = btoa(departmentId.toString());
    // Step 3: Construct the URL with the encoded department value
    const url = `${this.reportsUrl}ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FUserAttendanceDepartmentWise&rs%3AClearSession=true&rc%3AView=af7578f5-cebd-4b18-ac9f-43c11e11f867&Department=${encodedDepartment}`;
    // Step 4: Open the URL in a new tab
    window.open(url, '_blank');
  }

  redirectToEmployeeReport() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FEmployeeReport&rs%3AClearSession=true&rc%3AView=af7578f5-cebd-4b18-ac9f-43c11e11f867';
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

    redirectToEmployeeJoinerAndLeaverReport() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FEmployeeJoinerAndLeaverReport&rs%3AClearSession=true&rc%3AView=af7578f5-cebd-4b18-ac9f-43c11e11f867';
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

      redirectToLeaveBalanceReport() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FLeaveBalance&rs%3AClearSession=true&rc%3AView=af7578f5-cebd-4b18-ac9f-43c11e11f867';
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

    redirectToJournal() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FJournal&rs%3AClearSession=true&rc%3AView=af7578f5-cebd-4b18-ac9f-43c11e11f867&CompanyId=' + this.currentUser.department.companyId;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

     redirectToRevenue() {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FSehatNizam%2FRevenue&rs%3AClearSession=true&rc%3AView=af7578f5-cebd-4b18-ac9f-43c11e11f867&CompanyId=' + this.currentUser.department.companyId;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }

  private readonly submenuIconMap: Record<string, string> = {
    '/doctorappointment': 'fa-calendar-check',
    '/bookappointment': 'fa-calendar-plus',
    '/surgicalorder': 'fa-procedures',
    '/doctorplan': 'fa-calendar-alt',
    '/appointment': 'fa-user-plus',
    '/patient': 'fa-user-injured',
    '/referrer': 'fa-user-md',
    '/triage': 'fa-heartbeat',
    '/servicetype': 'fa-list-alt',
    '/services': 'fa-notes-medical',
    '/admissionpackage': 'fa-box-open',
    '/ward': 'fa-hospital',
    '/room': 'fa-door-open',
    '/bed': 'fa-bed',
    '/admission': 'fa-procedures',
    '/appointmentpayment': 'fa-money-bill-wave',
    '/advancepayment': 'fa-hand-holding-usd',
    '/bloodcomponenttype': 'fa-vial',
    '/bloodgroup': 'fa-tint',
    '/bloodfridge': 'fa-snowflake',
    '/bloodrack': 'fa-th',
    '/bloodcollection': 'fa-syringe',
    '/bloodstock': 'fa-warehouse',
    '/bloodtransfusion': 'fa-exchange-alt',
    '/labordertype': 'fa-flask',
    '/laborder': 'fa-vials',
    '/radiologytype': 'fa-x-ray',
    '/radiologyorder': 'fa-file-medical-alt',
    '/indentrequest': 'fa-file-alt',
    '/issuance': 'fa-dolly',
    '/inspection': 'fa-clipboard-check',
    '/rejectreason': 'fa-ban',
    '/roles': 'fa-user-shield',
    '/users': 'fa-users',
    '/companies': 'fa-building',
    '/location': 'fa-map-marker-alt',
    '/store': 'fa-store',
    '/uoms': 'fa-balance-scale',
    '/indenttype': 'fa-tags',
    '/priority': 'fa-flag',
    '/category': 'fa-folder',
    '/subcategory': 'fa-folder-open',
    '/itemtype': 'fa-cubes',
    '/items': 'fa-box',
    '/chartitems': 'fa-chart-bar',
    '/currency': 'fa-dollar-sign',
    '/shipmentmode': 'fa-shipping-fast',
    '/paymentmode': 'fa-credit-card',
    '/deliveryterms': 'fa-truck-loading',
    '/gst': 'fa-percent',
    '/vendors': 'fa-truck',
    '/accountflow': 'fa-project-diagram',
    '/accountcategory': 'fa-sitemap',
    '/accountsubcategory': 'fa-stream',
    '/accounttype': 'fa-layer-group',
    '/account': 'fa-book',
    '/accountgroup': 'fa-object-group',
    '/accountchart': 'fa-chart-pie',
    '/crv': 'fa-receipt',
    '/brv': 'fa-university',
    '/cpv': 'fa-money-check-alt',
    '/bpv': 'fa-landmark',
    '/jv': 'fa-journal-whills',
    '/pjv': 'fa-file-invoice',
    '/purchaseinvoice': 'fa-file-invoice-dollar',
    '/vehicle': 'fa-car',
    '/customer': 'fa-user-tie',
    '/rack': 'fa-archive',
    '/row': 'fa-grip-lines',
    '/section': 'fa-th-large',
    '/purchasedemand': 'fa-shopping-basket',
    '/dispatch': 'fa-shipping-fast',
    '/canceldispatch': 'fa-times-circle',
    '/salematerial': 'fa-cash-register',
    '/salereturn': 'fa-undo',
    '/purchasereturn': 'fa-undo-alt',
    '/salematerialreturn': 'fa-reply',
    '/warehousetransfer': 'fa-exchange-alt',
    '/grn': 'fa-clipboard-list',
    '/costsheet': 'fa-calculator',
    '/comparativestatement': 'fa-balance-scale-right',
    '/purchaseorder': 'fa-file-signature',
    '/userattendance': 'fa-fingerprint',
    '/saleuser': 'fa-user-tag',
    '/userterritory': 'fa-map',
    '/region': 'fa-globe-asia',
    '/area': 'fa-map-marked',
    '/zone': 'fa-map-pin',
    '/territory': 'fa-draw-polygon',
    '/distributor': 'fa-people-carry',
    '/route': 'fa-route',
    '/shop': 'fa-store-alt',
    '/shoptype': 'fa-tags',
    '/pricinggroup': 'fa-tags',
    '/primarysales': 'fa-chart-line',
    '/shoporders': 'fa-shopping-bag',
    '/fieldmap': 'fa-map-marked-alt',
    '/salestarget': 'fa-bullseye',
    '/project': 'fa-building',
    '/departments': 'fa-sitemap',
    '/employeedesignation': 'fa-id-badge',
    '/employeeeducation': 'fa-graduation-cap',
    '/employeegrade': 'fa-star',
    '/employeeshift': 'fa-clock',
    '/employeetype': 'fa-users-cog',
    '/employeebank': 'fa-university',
    '/employeedocumenttype': 'fa-file-alt',
    '/employeeovertimerate': 'fa-business-time',
    '/employeeworksitetype': 'fa-hard-hat',
    '/device': 'fa-microchip',
    '/cities': 'fa-city',
    '/hryear': 'fa-calendar',
    '/holiday': 'fa-umbrella-beach',
    '/employeeleavetype': 'fa-plane-departure',
    '/employeeleavegroup': 'fa-users',
    '/employee': 'fa-id-card',
    '/doctor': 'fa-stethoscope',
    '/roster': 'fa-calendar-week',
    '/manageemployeeleave': 'fa-calendar-minus',
    '/notification': 'fa-bell',
    '/candidateevaluationcategory': 'fa-clipboard-list',
    '/interview': 'fa-user-graduate',
    '/conductinterview': 'fa-comments',
    '/salaryhead': 'fa-coins',
    '/salarytaxslab': 'fa-percentage',
    '/approveemployeeleave': 'fa-check-circle',
    '/attendanceHr': 'fa-user-clock',
    '/rosterdepartment': 'fa-calendar-alt',
    '/igp': 'fa-truck',
  };

  getSubmenuIcon(route: string): string {
    return this.submenuIconMap[route] ?? 'fa-angle-right';
  }

}
