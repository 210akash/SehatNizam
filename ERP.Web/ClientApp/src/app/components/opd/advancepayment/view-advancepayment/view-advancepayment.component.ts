import { Component, Inject, Optional, ViewChild } from '@angular/core';
import {  FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AdvancePaymentListComponent } from '../advancepayment-list/advancepayment-list.component';

@Component({
  selector: 'app-view-advancepayment',
  templateUrl: './view-advancepayment.component.html',
  styleUrl: './view-advancepayment.component.css',
    standalone: false
})
export class ViewAdvancePaymentComponent {
  serviceForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  constructor(@Optional() @Inject(MAT_DIALOG_DATA) public data: { element: any } | null){}
  @ViewChild(AdvancePaymentListComponent) advancepaymentListComponent!: AdvancePaymentListComponent;

  ngOnInit(): void {
  }
} 