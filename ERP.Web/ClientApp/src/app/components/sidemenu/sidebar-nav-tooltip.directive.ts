import {
    AfterViewInit,
    Directive,
    ElementRef,
    Input,
    OnChanges,
    OnDestroy,
    SimpleChanges,
    inject,
} from '@angular/core';
import { ConnectedPosition, Overlay, OverlayRef } from '@angular/cdk/overlay';
import { ComponentPortal } from '@angular/cdk/portal';
import { SidebarTooltipPanelComponent } from './sidebar-tooltip-panel.component';

interface NavTooltipBinding {
    element: HTMLElement;
    label: string;
    onEnter: () => void;
    onLeave: () => void;
}

@Directive({
    selector: '[appSidebarNavTooltips]',
    standalone: false,
})
export class SidebarNavTooltipsDirective implements AfterViewInit, OnChanges, OnDestroy {
  @Input() sidebarCollapsed = false;

  private readonly host = inject(ElementRef<HTMLElement>);
  private readonly overlay = inject(Overlay);
  private bindings: NavTooltipBinding[] = [];
  private overlayRef: OverlayRef | null = null;
  private panelRef: ComponentPortal<SidebarTooltipPanelComponent> | null = null;
  private viewReady = false;

  private readonly positions: ConnectedPosition[] = [
    {
      originX: 'end',
      originY: 'center',
      overlayX: 'start',
      overlayY: 'center',
      offsetX: 10,
    },
  ];

  ngAfterViewInit(): void {
    this.viewReady = true;
    this.bindNavItems();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['sidebarCollapsed'] && !this.sidebarCollapsed) {
      this.hideTooltip();
    }
  }

  ngOnDestroy(): void {
    this.unbindNavItems();
    this.hideTooltip();
  }

  private bindNavItems(): void {
    this.unbindNavItems();

    const items = this.host.nativeElement.querySelectorAll(
      ':scope > .nav-group > .nav-link, :scope > .nav-group > .nav-button'
    );

    items.forEach((item: Element) => {
      const element = item as HTMLElement;
      const label = element.querySelector(':scope > span')?.textContent?.trim();
      if (!label) {
        return;
      }

      const onEnter = () => {
        if (this.sidebarCollapsed) {
          this.showTooltip(element, label);
        }
      };
      const onLeave = () => this.hideTooltip();

      element.addEventListener('mouseenter', onEnter);
      element.addEventListener('mouseleave', onLeave);
      element.addEventListener('focus', onEnter);
      element.addEventListener('blur', onLeave);

      this.bindings.push({ element, label, onEnter, onLeave });
    });
  }

  private unbindNavItems(): void {
    this.bindings.forEach(({ element, onEnter, onLeave }) => {
      element.removeEventListener('mouseenter', onEnter);
      element.removeEventListener('mouseleave', onLeave);
      element.removeEventListener('focus', onEnter);
      element.removeEventListener('blur', onLeave);
    });
    this.bindings = [];
  }

  private showTooltip(origin: HTMLElement, message: string): void {
    this.hideTooltip();

    const positionStrategy = this.overlay
      .position()
      .flexibleConnectedTo(origin)
      .withPositions(this.positions);

    this.overlayRef = this.overlay.create({
      positionStrategy,
      scrollStrategy: this.overlay.scrollStrategies.reposition(),
      panelClass: 'sidebar-nav-tooltip-panel',
    });

    const portal = new ComponentPortal(SidebarTooltipPanelComponent);
    const componentRef = this.overlayRef.attach(portal);
    componentRef.instance.message = message;
    this.panelRef = portal;
  }

  private hideTooltip(): void {
    if (this.overlayRef) {
      this.overlayRef.detach();
      this.overlayRef.dispose();
      this.overlayRef = null;
      this.panelRef = null;
    }
  }
}
