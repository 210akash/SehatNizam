import { Component, Inject } from '@angular/core';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-accountcategory',
    templateUrl: './view-accountcategory.component.html',
    styleUrl: './view-accountcategory.component.css',
    standalone: false
})

export class ViewAccountCategoryComponent {
  isLoading = false;
  isEditMode: boolean = true;

  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }
}
