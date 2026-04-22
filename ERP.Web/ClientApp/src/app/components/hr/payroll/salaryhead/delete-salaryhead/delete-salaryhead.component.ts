import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { SalaryHeadService } from '../salaryhead.service';
import { NotificationsService } from '../../../../../Service/notification.service';
import { ConstantService, SalaryHeadTypeEnum } from '../../../../../Service/constant.service';

@Component({
    selector: 'app-delete-salaryhead',
    templateUrl: './delete-salaryhead.component.html',
    styleUrl: './delete-salaryhead.component.css',
    standalone: false
})

export class DeleteSalaryHeadComponent {
  salaryheadForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  salaryHeadTypes: { [key: number]: string } = {};
  constructor(private dialog: MatDialog, private formBuilder: FormBuilder, private notificationsService: NotificationsService, private salaryheadService: SalaryHeadService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
      this.salaryHeadTypes = Object.keys(SalaryHeadTypeEnum)
          .filter(key => isNaN(Number(key))) // Filter out numeric keys
          .reduce((acc, key) => {
            const value = SalaryHeadTypeEnum[key as keyof typeof SalaryHeadTypeEnum];
            acc[value] = key; // Map numeric value to string name
            return acc;
          }, {} as { [key: number]: string });
    this.salaryheadForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
    });
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (this.data.element.id != null) {
      this.isEditMode = true;
    }
    this.constantService.LoadData(element, this.salaryheadForm);
  }

  async delete() {
    (await this.salaryheadService.deleteSalaryHead(this.data.element.id)).subscribe({
      next: (data) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        }
      },
      error: (error) => {
        console.log(error);
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }
}
