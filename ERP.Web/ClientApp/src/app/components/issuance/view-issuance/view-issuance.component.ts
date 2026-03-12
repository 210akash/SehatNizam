import { Component, Inject } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-issuance',
    templateUrl: './view-issuance.component.html',
    styleUrl: './view-issuance.component.css',
    standalone: false
})

export class ViewIssuanceComponent {
  IssuanceForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;

  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }
}
