import { Component, Inject, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { AdmissionBedListComponent } from '../admissionbed-list/admissionbed-list.component';
import { BedService } from '../../bed/bed.service';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-delete-admissionbed',
  templateUrl: './delete-admissionbed.component.html',
  styleUrl: './delete-admissionbed.component.css',
    standalone: false
})
export class DeleteAdmissionBedComponent {
  bedForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog,private formBuilder: FormBuilder,private bedService: BedService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }){}
  @ViewChild(AdmissionBedListComponent) admissionbedListComponent!: AdmissionBedListComponent;

  ngOnInit(): void {
  }

  LoadData(element: any) {
    debugger
    if (element != null) {
      this.isEditMode = true;
    }
    this.constantService.LoadData(element, this.bedForm);
  }

  async delete(){
    (await this.bedService.deleteBed(this.data.element.id)).subscribe({
      next: (data) => {
        if(data == true){
          this.isLoading = false;
          this.dialog.getDialogById("message-delete-tracker")?.close({ data: data });
          this.admissionbedListComponent.bindData();
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }
}
