import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { SalaryTaxSlabService } from '../salarytaxslab.service';
import { NotificationsService } from '../../../../../Service/notification.service';
import { ConstantService } from '../../../../../Service/constant.service';

@Component({
    selector: 'app-delete-salarytaxslab',
    templateUrl: './delete-salarytaxslab.component.html',
    styleUrl: './delete-salarytaxslab.component.css',
    standalone: false
})

export class DeleteSalaryTaxSlabComponent {
  salarytaxslabForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog, private formBuilder: FormBuilder, private notificationsService: NotificationsService, private salarytaxslabService: SalaryTaxSlabService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.salarytaxslabForm = this.formBuilder.group({
  id: [0],
      fromAmount: [0, [Validators.required, Validators.min(0)]],
      toAmount: [0, [Validators.required, Validators.min(0)]],
      percentage: [0, [Validators.required, Validators.min(0.001)]],
    });
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (this.data.element.id != null) {
      this.isEditMode = true;
    }
    this.constantService.LoadData(element, this.salarytaxslabForm);
  }

  async delete() {
    (await this.salarytaxslabService.deleteSalaryTaxSlab(this.data.element.id)).subscribe({
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
