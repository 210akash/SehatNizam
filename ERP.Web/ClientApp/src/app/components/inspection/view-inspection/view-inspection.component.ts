import { Component, Inject } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-inspection',
    templateUrl: './view-inspection.component.html',
    styleUrl: './view-inspection.component.css',
    standalone: false
})

export class ViewInspectionComponent {
  InspectionForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;

  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }
}
