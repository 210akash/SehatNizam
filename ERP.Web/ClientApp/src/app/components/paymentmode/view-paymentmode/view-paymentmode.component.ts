import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-paymentmode',
    templateUrl: './view-paymentmode.component.html',
    styleUrl: './view-paymentmode.component.css',
    standalone: false
})

export class ViewPaymentModeComponent {
  isLoading = false;
  isEditMode: boolean = true;

  constructor( @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }
}
