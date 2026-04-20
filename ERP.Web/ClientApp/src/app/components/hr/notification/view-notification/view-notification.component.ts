import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';

@Component({
    selector: 'app-view-notification',
    templateUrl: './view-notification.component.html',
    styleUrls: ['./view-notification.component.css'],
    standalone: false
})

export class ViewNotificationComponent {
    constructor(
        private dialog: MatDialog,
        @Inject(MAT_DIALOG_DATA) public data: { element: any }
    ) { }

    close() {
        this.dialog.closeAll();
    }
}
