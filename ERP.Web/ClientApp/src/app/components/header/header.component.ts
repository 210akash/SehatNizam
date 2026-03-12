import { Component, EventEmitter, Input, ViewChild, OnDestroy, OnInit, Output } from '@angular/core';
import { NavigationEnd, Router, RouterEvent } from '@angular/router';
import { AuthenticationService } from '../../Auth/authentication.service';
import { ConstantService } from '../../Service/constant.service';
import { MatSidenav } from '@angular/material/sidenav';
import { delay, filter, Subject, takeUntil } from 'rxjs';
import { BreakpointObserver } from '@angular/cdk/layout';
import { Location } from '@angular/common';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css'],
  standalone: false
})
export class HeaderComponent implements OnDestroy {
  // private listTitles: any;
  isEstimator = false;
  isLoading = false;
  ticketData: any;
  @Input() isExpanded: boolean = false;
  @Output() toggleSidebar: EventEmitter<boolean> = new EventEmitter<boolean>();
  @ViewChild(MatSidenav)
  sidenav!: MatSidenav;
  currentUser: any;
  roleList!: any;
  private ngUnsubscribe = new Subject<void>();
  isSubMenuVisible: { [key: string]: boolean } = {
    // Add more keys if you have more menus
    'menu1': false,
    'menu2': false,
    'menu3': false,
    'menu4': false,
    'menu5': false
  };

  constructor(private router: Router,
    private authenticationService: AuthenticationService, private observer: BreakpointObserver, public constantService: ConstantService) { }

  ngOnInit(): void {
    this.currentUser = this.authenticationService.currentUserValue;
    this.roleList = this.currentUser.role
      .split(',')
      .map((role: string) => role.trim().toLowerCase())
      .filter((role: string) => role !== ''); // Remove empty entries
  }

  ngAfterViewInit() {
    if (!this.sidenav) {
      console.error('Sidenav is undefined or not initialized.');
      return;
    }
  
    this.observer
      .observe(['(max-width: 800px)'])
      .pipe(delay(1), takeUntil(this.ngUnsubscribe))
      .subscribe((res) => {
        if (res.matches) {
          this.sidenav.mode = 'over';
          this.sidenav.close();
        } else {
          this.sidenav.mode = 'side';
          this.sidenav.open();
        }
      });
  
    this.router.events
      .pipe(
        filter((e): e is NavigationEnd => e instanceof NavigationEnd),
        takeUntil(this.ngUnsubscribe)
      )
      .subscribe(() => {
        if (this.sidenav && this.sidenav.mode === 'over') {
          this.sidenav.close();
        }
      });
  }

  logout() {
    this.authenticationService.logout();
    //this.router.navigate(['/login']);
    window.location.href = '/login';
  }

  toggleSubMenu(menu: string) {
    this.isSubMenuVisible[menu] = !this.isSubMenuVisible[menu];
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
    return this.router.url.includes(routePath);
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

}




