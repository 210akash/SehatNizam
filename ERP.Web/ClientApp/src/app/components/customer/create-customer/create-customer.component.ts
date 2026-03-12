import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { createMask } from '@ngneat/input-mask';
import { DealershipService } from '../../order/dealership/dealership.service';
import { ConstantService } from '../../../Service/constant.service';
import { NotificationsService } from '../../../Service/notification.service';

@Component({
  selector: 'app-create-customer',
  templateUrl: './create-customer.component.html',
  styleUrls: ['./create-customer.component.css'],
  standalone: false,
})

export class CreateCustomerComponent implements OnInit {
  createCustomerForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  roleList: any;
  isEditMode: boolean = false;

  phoneNoInputMask = createMask('0399-9999999');
  cnicInputMask = createMask('99999-9999999-9');

  dialogRef: any;

  constructor(
    private notificationsService: NotificationsService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private constantService: ConstantService,
    private customerService: DealershipService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) {}

  ngOnInit(): void {
    this.createCustomerForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
      phoneNo: ['', Validators.required],
      secondaryPhoneNo: [''],
      address: ['', Validators.required],
      cnic: [''],
      ntn: [''],
      isActive: [true, Validators.required],
      dealershipTypeId: [2],
      territoryId: [null],
    });

    this.LoadData(this.data.element);
  }

  get f() {
    return this.createCustomerForm.controls;
  }

  async saveCustomer() {
    this.isLoading = true;
    if (this.createCustomerForm.invalid) {
      this.constantService.markFormGroupTouched(this.createCustomerForm);
      return;
    }
    let _createCustomerForm: any = {};
    _createCustomerForm = Object.assign(
      _createCustomerForm,
      this.createCustomerForm.value
    );

    (await this.customerService.saveDealership(_createCustomerForm)).subscribe({
      next: (data) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(
            'Customer Saved Successfully!',
            'snack-bar-success'
          );
          this.dialog.closeAll();
          this.isLoading = false;
        } else if (data.Status == 409) {
          this.notificationsService.showNotification(
            'Name already exist!',
            'snack-bar-danger'
          );
          this.isLoading = false;
        }
      },
      error: (error) => {
        this.notificationsService.showNotification(
          'Please Fill the required fields!',
          'snack-bar-danger'
        );
        console.log(error);
        this.isLoading = false;
      },
    });
  }

  LoadData(element: any) {
    if (this.data.element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.createCustomerForm);
    }
    console.log(this.createCustomerForm);
  }
}
