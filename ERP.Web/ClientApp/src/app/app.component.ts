import { Component, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, NavigationError, NavigationStart, Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  standalone: false
})
export class AppComponent implements OnInit, OnDestroy {
  showLoader = true;

  constructor(private router: Router) {}

  ngOnInit() {
    this.router.events.subscribe((event: any) => {
      if (event instanceof NavigationStart) {
        this.showLoader = true;
      }

      if (event instanceof NavigationEnd || event instanceof NavigationError) {
        if (event.url === '/hrdashboard') {
          this.showLoader = false;
        } else {
          this.showLoader = true;
        }
      }
    });
  }

  ngOnDestroy(): void {
    // Implement if you add subscriptions later
  }

  title(title: any) {
    throw new Error('Method not implemented.');
  }
}
