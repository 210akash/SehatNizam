import { Component, Inject } from '@angular/core';
import { FormGroup,  } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-currency',
    templateUrl: './view-currency.component.html',
    styleUrl: './view-currency.component.css',
    standalone: false
})

export class ViewCurrencyComponent {
  currencyForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;

  constructor( @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }
}
