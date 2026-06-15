import { Component, Inject, Optional, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { AdvancePaymentListComponent } from '../advancepayment-list/advancepayment-list.component';
import { ConstantService } from '../../../../Service/constant.service';
import { AdvancePaymentService } from '../advancepayment.service';

@Component({
  selector: 'app-confirm-advancepayment',
  templateUrl: './confirm-advancepayment.component.html',
  styleUrl: './confirm-advancepayment.component.css',
    standalone: false
})
export class ConfirmAdvancePaymentComponent {
  serviceForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog,private formBuilder: FormBuilder,private advancePaymentService: AdvancePaymentService, private constantService: ConstantService, @Optional() @Inject(MAT_DIALOG_DATA) public data: { element: any } | null){}
  @ViewChild(AdvancePaymentListComponent) advancepaymentListComponent!: AdvancePaymentListComponent;

  ngOnInit(): void {
  }

  LoadData(element: any) {
    debugger
    if (element != null) {
      this.isEditMode = true;
    }
    this.constantService.LoadData(element, this.serviceForm);
  }

  async confirm(){
    const elementId = this.data?.element?.id;
    if (!elementId) return;
    
    (await this.advancePaymentService.confirmAdvancePayment(elementId)).subscribe({
      next: (data) => {
        if(data == true){
          this.isLoading = false;
          this.dialog.closeAll();
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }
}