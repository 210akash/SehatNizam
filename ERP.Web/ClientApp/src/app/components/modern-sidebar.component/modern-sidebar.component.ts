import { Component, OnInit, HostListener } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-modern-sidebar',
  templateUrl: './modern-sidebar.component.html',
  styleUrls: ['./modern-sidebar.component.css'],
  standalone: false
})
export class ModernSidebarComponent implements OnInit {
  isSidebarOpen = true;
  openSubMenus: Set<string> = new Set();
  currentUser: any = {
    firstName: 'Jane',
    lastName: 'Doe',
    department: { name: 'Administration', company: { name: 'BGC - IGC' } },
    role: 'admin'
  };
  roleList: string[] = ['admin'];

  constructor(private router: Router) {}

  ngOnInit(): void {
    // Initialize roles from your auth service logic
    // this.roleList = this.currentUser.role.split(',').map(r => r.trim().toLowerCase());
  }

  toggleSidebar() {
    this.isSidebarOpen = !this.isSidebarOpen;
  }

  toggleSubMenu(menuKey: string): void {
    if (this.openSubMenus.has(menuKey)) {
      this.openSubMenus.delete(menuKey);
    } else {
      this.openSubMenus.clear();
      this.openSubMenus.add(menuKey);
    }
  }

  isSubMenuVisible(menuKey: string): boolean {
    return this.openSubMenus.has(menuKey);
  }

  isActive(routePath: string): boolean {
    return this.router.url === routePath;
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
    console.log('Logging out...');
  }
}
