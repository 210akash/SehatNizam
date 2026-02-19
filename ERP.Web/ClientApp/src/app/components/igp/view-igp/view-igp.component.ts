import { Component, Inject } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-iGP',
    templateUrl: './view-iGP.component.html',
    styleUrl: './view-iGP.component.css',
    standalone: false
})

export class ViewIGPComponent {
  IGPForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;

  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }
}
