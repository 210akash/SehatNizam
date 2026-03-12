import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-grn',
    templateUrl: './view-grn.component.html',
    styleUrl: './view-grn.component.css',
    standalone: false
})

export class ViewGRNComponent {
  isLoading = false;
  isEditMode: boolean = true;

  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }
  ngOnInit(): void {
    console.log(this.data.element);
  }
}
