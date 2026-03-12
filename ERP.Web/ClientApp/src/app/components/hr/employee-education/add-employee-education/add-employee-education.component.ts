import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { EmployeeEducationService } from '../employee-education.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { CompanyService } from '../../../company/company.service';

@Component({
    selector: 'app-add-employee-education',
    templateUrl: './add-employee-education.component.html',
    styleUrl: './add-employee-education.component.css',
    standalone: false
})

export class AddEmployeeEducationComponent {
  employeeEducationForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  companyList: any;

  constructor( private companyService: CompanyService,private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private employeeEducationService: EmployeeEducationService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.employeeEducationForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
    });
    
    this.LoadData(this.data.element);
    this.getCompanyList();
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.employeeEducationForm);
    }
  }

  SaveData() {
    if (this.employeeEducationForm.invalid) {
      this.constantService.markFormGroupTouched(this.employeeEducationForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.employeeEducationForm.value);

    this.employeeEducationService.saveEmployeeEducation(_clienttemperatureForm).subscribe({
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
