import { Component, Inject } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-comparativestatement',
    templateUrl: './view-comparativestatement.component.html',
    styleUrl: './view-comparativestatement.component.css',
    standalone: false
})

export class ViewComparativeStatementComponent {
  ComparativeStatementForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;

  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }
}
