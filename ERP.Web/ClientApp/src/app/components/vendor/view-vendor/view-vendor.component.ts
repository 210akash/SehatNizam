import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-vendor',
    templateUrl: './view-vendor.component.html',
    styleUrl: './view-vendor.component.css',
    standalone: false
})

export class ViewVendorComponent {
  isLoading = false;
  isEditMode: boolean = true;

  constructor( @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }
}
