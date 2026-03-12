import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { EmployeeLeaveTypeService } from '../employee-leave-type.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { CompanyService } from '../../../company/company.service';

@Component({
    selector: 'app-add-employee-leave-type',
    templateUrl: './add-employee-leave-type.component.html',
    styleUrl: './add-employee-leave-type.component.css',
    standalone: false
})

export class AddEmployeeLeaveTypeComponent {
  employeeLeaveTypeForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  companyList: any;

  constructor( private companyService: CompanyService,private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private employeeLeaveTypeService: EmployeeLeaveTypeService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.employeeLeaveTypeForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      name: ['', Validators.required],
    });
    
    this.LoadData(this.data.element);
    this.getCompanyList();
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.employeeLeaveTypeForm);
    }
  }

  SaveData() {
    if (this.employeeLeaveTypeForm.invalid) {
      this.constantService.markFormGroupTouched(this.employeeLeaveTypeForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.employeeLeaveTypeForm.value);

    this.employeeLeaveTypeService.saveEmployeeLeaveType(_clienttemperatureForm).subscribe({
      next: (data) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else
          this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        console.error(error);
        this.isLoading = false;
      }
    });
  }

  getCompanyList(): void {
    let _companyForm: any = {};
    this.companyService.getAllCompanys(_companyForm).subscribe(data => {
      this.companyList = data.item1;
    });
  }
}
