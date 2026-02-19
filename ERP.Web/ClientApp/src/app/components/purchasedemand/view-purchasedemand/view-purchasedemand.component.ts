import { Component, Inject } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-purchasedemand',
    templateUrl: './view-purchasedemand.component.html',
    styleUrl: './view-purchasedemand.component.css',
    standalone: false
})

export class ViewPurchaseDemandComponent {
  PurchaseDemandForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;

  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }
}
