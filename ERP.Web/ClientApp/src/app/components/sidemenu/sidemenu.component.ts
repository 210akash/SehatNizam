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
  constructor(private dialog: MatDialog, location: Location,
    private observer: BreakpointObserver,
    private router: Router,
    private authenticationService: AuthenticationService,
    private userService: UserService,
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
    this.observer
    // .observe(['(max-width: 800px)'])
    // .pipe(delay(1), takeUntil(this.ngUnsubscribe))
    // .subscribe((res) => {
    //   if (res.matches) {
    //     console.log(`Viewport matches max-width: 801px?`, res.matches);
    //     this.sidenav.mode = 'over';
    //     this.sidenav.open();
    //   } else {
    //     console.log(`Viewport matches max-width: 799px?`, res.matches);
    //     this.sidenav.mode = 'side';
    //     this.sidenav.close();
    //   }
    // });

    this.router.events
      .pipe(
        filter((e: Event) => e instanceof NavigationEnd),
        takeUntil(this.ngUnsubscribe)
      )
      .subscribe(() => {
        if (this.sidenav.mode === 'over') {
          this.sidenav.close();
        }
      });
  }


  
  toggleSidebar() {
    this.isSidebarOpen = !this.isSidebarOpen;
  }

  toggleSubMenu(menuKey: string): void {
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

}
