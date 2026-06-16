import { Component, Input } from '@angular/core';

@Component({
    selector: 'app-sidebar-tooltip-panel',
    standalone: false,
    template: `<div class="sidebar-nav-tooltip">{{ message }}</div>`,
    styles: [`
    .sidebar-nav-tooltip {
      background: #424242;
      color: #ffffff;
      font-size: 12px;
      font-weight: 600;
      line-height: 1.3;
      padding: 6px 10px;
      border-radius: 4px;
      white-space: nowrap;
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.18);
    }
  `],
})
export class SidebarTooltipPanelComponent {
  @Input() message = '';
}
