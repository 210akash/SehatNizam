import { BreakpointObserver } from '@angular/cdk/layout';
import { Component, EventEmitter, Input, Output, ViewChild,OnDestroy, HostListener } from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';
import { Subject, delay, filter, takeUntil } from 'rxjs';
import { NavigationEnd, Router, Event, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from '../../Auth/authentication.service';
import { Location } from '@angular/common';

@Component({
    selector: 'app-sidemenu',
    templateUrl: './sidemenu.component.html',
    styleUrls: ['./sidemenu.component.css'],
    standalone: false
})
export class SidemenuComponent implements OnDestroy {
  // private listTitles: any;
  isEstimator = false;
  isLoading = false;
  ticketData: any;
  @Input() isExpanded: boolean = false;
  @Output() toggleSidebar: EventEmitter<boolean> = new EventEmitter<boolean>();
  @ViewChild(MatSidenav)
  sidenav!: MatSidenav;
  location: Location;
  roleList! : any;
  currentUser: any;

  isSubMenuVisible: { [key: string]: boolean } = {
    'menu1': false,
    'menu2': false,
    'menu3': false,
    'menu4': false,
    'menu5': false
  };

  constructor(location: Location,private observer: BreakpointObserver, private router: Router, private authenticationService :  AuthenticationService, private route: ActivatedRoute) {
    this.location = location;
  }

private ngUnsubscribe = new Subject<void>();

ngOnInit(): void {
  this.currentUser = this.authenticationService.currentUserValue;
  this.roleList = this.currentUser.role
  .split(',')
  .map((role: string) => role.trim().toLowerCase())
  .filter((role: string) => role !== '');
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

logout() {
  this.authenticationService.logout();
  //this.router.navigate(['/login']);
  window.location.href = '/login';
}

toggleSubMenu(menuKey: string): void {
  Object.keys(this.isSubMenuVisible).forEach((key) => {
    if (key !== menuKey) {
      this.isSubMenuVisible[key] = false;
    }
  });

  this.isSubMenuVisible[menuKey] = !this.isSubMenuVisible[menuKey];
}

@HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const clickedElement = event.target as HTMLElement;
    const menuElement = clickedElement.closest('.menu-item');
    if (!menuElement) {
      this.isSubMenuVisible = {};
    }
  }

  closeSubMenu(menuKey: string): void {
    this.isSubMenuVisible[menuKey] = false;
  }

// getTitle() {
//   var titlee = this.location.prepareExternalUrl(this.location.path());
//   for (var item = 0; item < this.listTitles.length; item++) {
//       if (this.listTitles[item].path === titlee) {
//           return this.listTitles[item].title;
//       }
//       else{
//           if (this.listTitles[item].hasOwnProperty('subMenu')) {
//               for (var item1 = 0; item1 < this.listTitles[item].subMenu.length; item1++) {
//               if (this.listTitles[item].subMenu[item1].path === titlee) {
//                   return this.listTitles[item].subMenu[item1].title;
//               }
//           }
//           }
//       }
//   }
//   return 'ERP';
// }

isActive(routePath: string): boolean {
  return this.router.url == routePath;
}

ngOnDestroy() {
  this.ngUnsubscribe.next();
  this.ngUnsubscribe.complete();
}

  handleSidebarToggle = () => this.toggleSidebar.emit(!this.isExpanded);

// Method to check if the user has any of the specified roles
hasRequiredRole(requiredRoles: string[]): boolean {
  return requiredRoles.some(role => this.roleList.includes(role.toLowerCase()));
}

redirectTochartofaccount() {
  const url = 'http://report:Network%123@202.166.160.200:9097/ReportServer/Pages/ReportViewer.aspx?%2FERPReports%2FChartOfAccount&rs%3AClearSession=true&rc%3AView=955b82da-9d4c-41a3-8fd2-995b91d5efd8&CompanyId='+ this.currentUser.department.companyId;
  window.open(url, '_blank');  // Opens the URL in a new tab
}

redirectTotrailbalance() {
  const url = 'http://report:Network%123@202.166.160.200:9097/ReportServer/Pages/ReportViewer.aspx?%2FERPReports%2FTrialBalance&rs%3AClearSession=true&rc%3AView=955b82da-9d4c-41a3-8fd2-995b91d5efd8&CompanyId='+ this.currentUser.department.companyId;
  window.open(url, '_blank');  // Opens the URL in a new tab
}

redirectToaccountledger() {
  const url = 'http://report:Network%123@202.166.160.200:9097/ReportServer/Pages/ReportViewer.aspx?%2FERPReports%2FAccountLedger&rs%3AClearSession=true&rc%3AView=955b82da-9d4c-41a3-8fd2-995b91d5efd8&CompanyId='+ this.currentUser.department.companyId;
  window.open(url, '_blank');  // Opens the URL in a new tab
}

}
