import { Directive, Input, TemplateRef, ViewContainerRef, OnInit } from '@angular/core';

@Directive({
    selector: '[appHasRole]',
    standalone: true
})

export class HasRoleDirective implements OnInit {
    @Input('appHasRole') requiredRoles: string | string[] = [];
    private roleList: string[] = [];

    constructor(
        private templateRef: TemplateRef<any>,
        private viewContainer: ViewContainerRef
    ) { }

    ngOnInit(): void {
        // Get user from localStorage and extract roles
        const user = JSON.parse(localStorage.getItem('currentUser') || '{}');
        const roles = user.role || '';

        this.roleList = roles
            .split(',')
            .map((role: string) => role.trim().toLowerCase())
            .filter((role: string) => role !== '');

        this.updateView();
    }

    private updateView(): void {

        const rolesToCheck = Array.isArray(this.requiredRoles)
            ? this.requiredRoles
            : [this.requiredRoles];

        const hasAccess = rolesToCheck.some(role =>
            this.roleList.includes(role.toLowerCase())
        );

        if (hasAccess) {
            this.viewContainer.createEmbeddedView(this.templateRef);
        } else {
            this.viewContainer.clear();
        }
    }
}