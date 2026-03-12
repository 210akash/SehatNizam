import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { EmployeeOvertimeRateService } from '../employee-overtimerate.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { CompanyService } from '../../../company/company.service';

@Component({
    selector: 'app-add-employee-overtimerate',
    templateUrl: './add-employee-overtimerate.component.html',
    styleUrl: './add-employee-overtimerate.component.css',
    standalone: false
})

export class AddEmployeeOvertimeRateComponent {
  employeeOvertimeRateForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  companyList: any;

  constructor( private companyService: CompanyService,private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private employeeOvertimeRateService: EmployeeOvertimeRateService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.employeeOvertimeRateForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
      rate: [0, Validators.required],
    });
    
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.employeeOvertimeRateForm);
    }
  }

  SaveData() {
    if (this.employeeOvertimeRateForm.invalid) {
      this.constantService.markFormGroupTouched(this.employeeOvertimeRateForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.employeeOvertimeRateForm.value);

    this.employeeOvertimeRateService.saveEmployeeOvertimeRate(_clienttemperatureForm).subscribe({
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
}
