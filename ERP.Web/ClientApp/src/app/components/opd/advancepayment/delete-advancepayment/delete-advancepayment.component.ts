import { Component, Inject, Optional, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { AdvancePaymentListComponent } from '../advancepayment-list/advancepayment-list.component';
import { ConstantService } from '../../../../Service/constant.service';
import { AdvancePaymentService } from '../advancepayment.service';

@Component({
  selector: 'app-delete-advancepayment',
  templateUrl: './delete-advancepayment.component.html',
  styleUrl: './delete-advancepayment.component.css',
    standalone: false
})
export class DeleteAdvancePaymentComponent {
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

  async delete(){
    const elementId = this.data?.element?.id;
    if (!elementId) return;
    
    (await this.advancePaymentService.deleteAdvancePayment(elementId)).subscribe({
      next: (data) => {
        if(data == true){
          this.isLoading = false;
          this.dialog.getDialogById("message-delete-tracker")?.close({ data: data });
          this.advancepaymentListComponent.bindData();
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }
}