import { Component } from '@angular/core';
import { SidemenuModule } from '../sidemenu/sidemenu.module';

@Component({
    selector: 'app-home-layout',
    template: `
    <app-sidemenu></app-sidemenu>
  `,
    styles: [],
    standalone: true,
    imports: [SidemenuModule],
})
export class HomeLayoutComponent { }
