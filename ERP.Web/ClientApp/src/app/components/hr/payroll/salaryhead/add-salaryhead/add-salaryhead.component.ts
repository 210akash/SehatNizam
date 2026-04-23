import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../../Service/notification.service';
import { ConstantService, SalaryHeadTypeEnum } from '../../../../../Service/constant.service';
import { SalaryHeadService } from '../salaryhead.service';

@Component({
  selector: 'app-add-salaryhead',
  templateUrl: './add-salaryhead.component.html',
  styleUrl: './add-salaryhead.component.css',
  standalone: false
})

export class AddSalaryHeadComponent {
  salaryheadForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  companyList: any;
  salaryHeadTypes: { key: string; value: number }[] = [];

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private salaryheadService: SalaryHeadService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.salaryheadForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
      type: [1, Validators.required],
      isTaxable: [false],
    });

    this.loadSalaryHeadTypes();
    this.LoadData(this.data.element);
  }

  loadSalaryHeadTypes(): void {
    this.salaryHeadTypes = Object.keys(SalaryHeadTypeEnum)
      .filter(key => isNaN(Number(key))) // Filter out numeric keys
      .map(key => ({
        key: key,
        value: SalaryHeadTypeEnum[key as keyof typeof SalaryHeadTypeEnum]
      }));
    console.log(this.salaryHeadTypes);
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.salaryheadForm);
    }
  }

  SaveData() {
    if (this.salaryheadForm.invalid) {
      this.constantService.markFormGroupTouched(this.salaryheadForm);
      return;
    }

    let _salaryheadForm: any = {};
    _salaryheadForm = Object.assign(_salaryheadForm, this.salaryheadForm.value);

    this.isLoading = true;

    this.salaryheadService.saveSalaryHead(_salaryheadForm).subscribe({
      next: (data) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        } else {
          this.notificationsService.showNotification(data.Data || data.Error || 'Error saving notification', 'snack-bar-danger');
        }
        this.isLoading = false;
      },
      error: (error) => {
        this.notificationsService.showNotification(error?.error?.Error || 'Error saving notification', 'snack-bar-danger');
        console.error(error);
        this.isLoading = false;
      }
    });
  }
}
