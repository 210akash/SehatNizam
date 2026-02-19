import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-accounttype',
    templateUrl: './view-accounttype.component.html',
    styleUrl: './view-accounttype.component.css',
    standalone: false
})

export class ViewAccountTypeComponent {
  isLoading = false;
  isEditMode: boolean = true;

  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }
}
