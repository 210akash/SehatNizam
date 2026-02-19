import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-accountsubcategory',
    templateUrl: './view-accountsubcategory.component.html',
    styleUrl: './view-accountsubcategory.component.css',
    standalone: false
})

export class ViewAccountSubcategoryComponent {
  isLoading = false;
  isEditMode: boolean = true;

  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }
}
