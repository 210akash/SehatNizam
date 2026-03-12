import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { EmployeeTypeService } from '../employee-type.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { CompanyService } from '../../../company/company.service';

@Component({
  selector: 'app-add-employee-type',
  templateUrl: './add-employee-type.component.html',
  styleUrl: './add-employee-type.component.css',
  standalone: false
})

export class AddEmployeeTypeComponent {
  employeeTypeForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  companyList: any;

  constructor(private companyService: CompanyService, private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private employeeTypeService: EmployeeTypeService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.employeeTypeForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
      noOfLeavesPerMonth: [0, Validators.required],
    });

    this.LoadData(this.data.element);
    this.getCompanyList();
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.employeeTypeForm);
    }
  }

  SaveData() {
    if (this.employeeTypeForm.invalid) {
      this.constantService.markFormGroupTouched(this.employeeTypeForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.employeeTypeForm.value);

    this.employeeTypeService.saveEmployeeType(_clienttemperatureForm).subscribe({
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
