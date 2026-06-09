import { Component, Inject, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { AdmissionServiceListComponent } from '../admissionservice-list/admissionservice-list.component';
import { ConstantService } from '../../../../Service/constant.service';
import { ServiceService } from '../../../opd/service/service.service';

@Component({
  selector: 'app-delete-admissionservice',
  templateUrl: './delete-admissionservice.component.html',
  styleUrl: './delete-admissionservice.component.css',
    standalone: false
})
export class DeleteAdmissionServiceComponent {
  serviceForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog,private formBuilder: FormBuilder,private serviceService: ServiceService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }){}
  @ViewChild(AdmissionServiceListComponent) admissionserviceListComponent!: AdmissionServiceListComponent;

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
    (await this.serviceService.deleteService(this.data.element.id)).subscribe({
      next: (data) => {
        if(data == true){
          this.isLoading = false;
          this.dialog.getDialogById("message-delete-tracker")?.close({ data: data });
          this.admissionserviceListComponent.bindData();
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }
}
