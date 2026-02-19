import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-deliveryterms',
    templateUrl: './view-deliveryterms.component.html',
    styleUrl: './view-deliveryterms.component.css',
    standalone: false
})

export class ViewDeliveryTermsComponent {
  isLoading = false;
  isEditMode: boolean = true;

  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }
}
