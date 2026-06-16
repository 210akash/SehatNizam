import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { OverlayModule } from '@angular/cdk/overlay';
import { SidemenuComponent } from './sidemenu.component';
import { SidebarNavTooltipsDirective } from './sidebar-nav-tooltip.directive';
import { SidebarTooltipPanelComponent } from './sidebar-tooltip-panel.component';

@NgModule({
    declarations: [
        SidemenuComponent,
        SidebarNavTooltipsDirective,
        SidebarTooltipPanelComponent,
    ],
    imports: [
        CommonModule,
        RouterModule,
        MatTooltipModule,
        MatIconModule,
        MatMenuModule,
        MatButtonModule,
        MatCardModule,
        MatDividerModule,
        OverlayModule,
    ],
    exports: [
        SidemenuComponent,
    ],
})
export class SidemenuModule { }
