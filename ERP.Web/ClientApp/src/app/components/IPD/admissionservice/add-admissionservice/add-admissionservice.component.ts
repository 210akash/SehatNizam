import { Component, Inject, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { AdmissionServiceListComponent } from '../admissionservice-list/admissionservice-list.component';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AdmissionServiceService } from '../admissionservice.service';
import { PaymentModeService } from '../../../paymentmode/paymentmode.service';
import { ServiceService } from '../../../opd/service/service.service';

@Component({
  selector: 'app-add-admissionservice',
  templateUrl: './add-admissionservice.component.html',
  styleUrl: './add-admissionservice.component.css',
  standalone: false
})
export class AddAdmissionServiceComponent {
  admissionServiceForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  serviceList :any;
  paymentModesList: any;

  constructor( private admissionServiceService: AdmissionServiceService, 
    private formBuilder: FormBuilder, 
    private dialog: MatDialog, 
    private notificationsService: NotificationsService, 
     private paymentModeService: PaymentModeService,
     private serviceService: ServiceService,
    private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }
  @ViewChild(AdmissionServiceListComponent) admissionserviceListComponent!: AdmissionServiceListComponent;

  ngOnInit(): void {

    this.admissionServiceForm = this.formBuilder.group({
      id: [this.data.element.id],
      admissionId : [this.data.element.id],
      serviceId : [0, Validators.required],
      basePrice: [0, Validators.required],
      paymentModeId: [5, Validators.required],
      discount: [0, Validators.required],
      payable: [0, Validators.required],
      paymentStatusId : [1]
    });
  
    this.getAllPaymentModes();
    this.getserviceList();
  }

  getAllPaymentModes() {
    this.paymentModeService.getAllPaymentModes({})
      .subscribe((d: any) => this.paymentModesList = d?.item1 ?? []);
  }

  SaveData() {
    debugger
    if (this.admissionServiceForm.invalid) {
      this.constantService.markFormGroupTouched(this.admissionServiceForm);
      return;
    }

    this.isLoading = true;
    let _admissionServiceForm: any = {};
    _admissionServiceForm = Object.assign(_admissionServiceForm, this.admissionServiceForm.value);

    this.admissionServiceService.saveAdmissionService(_admissionServiceForm).subscribe({
      next: (data) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          // this.admissionserviceListComponent.bindData();
          this.admissionServiceForm.reset();
          this.dialog.closeAll();
        }
        else
          this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }

  getserviceList() {
    let _WardFilter: any = {};
    this.serviceService.getAllServices(_WardFilter).subscribe((data: any) => {
     this.serviceList = data.item1;
    });
  }

selectService(event: any) {
  // Get the selected service id
  const selectedServiceId = event.value;

  // Find the corresponding service object from serviceList
  const selectedService = this.serviceList.find((s: { id: any; }) => s.id === selectedServiceId);

  if (selectedService) {
    // Update the form's basePrice and serviceId
    this.admissionServiceForm.patchValue({
      serviceId: selectedService.id,
      basePrice: selectedService.basePrice,
      payable: selectedService.basePrice // if payable should initially equal basePrice
    });
  }
}

calculateTotalPayable() {
    const fee = Number(this.admissionServiceForm.get('basePrice')?.value) || 0;
    const discount = Number(this.admissionServiceForm.get('discount')?.value) || 0;
    if (discount > fee) {
      this.admissionServiceForm.get('discount')?.setValue(0, { emitEvent: false });
      this.admissionServiceForm.get('payable')?.setValue(fee, { emitEvent: false });
      this.notificationsService.showNotification('Discount can be greater than rate.', 'snack-bar-danger');
    }
    else {
      const total = fee - discount;
      var payable = total < 0 ? 0 : Number(total.toFixed(2));
      this.admissionServiceForm.get('payable')?.setValue(payable, { emitEvent: false });
    }
  }

 reset(){
  this.admissionServiceForm.get('roomId')?.patchValue('');
  this.admissionServiceForm.get('bedId')?.patchValue('');
 }

}
