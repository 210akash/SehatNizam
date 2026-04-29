import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { EmployeeShiftService } from '../employee-shift.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { CompanyService } from '../../../company/company.service';

@Component({
    selector: 'app-add-employee-shift',
    templateUrl: './add-employee-shift.component.html',
    styleUrl: './add-employee-shift.component.css',
    standalone: false
})

export class AddEmployeeShiftComponent {
  employeeShiftForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  companyList: any;

  constructor( private companyService: CompanyService,private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private employeeShiftService: EmployeeShiftService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.employeeShiftForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      name: ['', Validators.required],
      fromTime: [null, Validators.required],
      toTime: [null, Validators.required],
      isDualDate: [false],
    });
    
    this.LoadData(this.data.element);
    this.getCompanyList();
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.employeeShiftForm);
    }
  }

  SaveData() {
    if (this.employeeShiftForm.invalid) {
      this.constantService.markFormGroupTouched(this.employeeShiftForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.employeeShiftForm.value);

    this.employeeShiftService.saveEmployeeShift(_clienttemperatureForm).subscribe({
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
