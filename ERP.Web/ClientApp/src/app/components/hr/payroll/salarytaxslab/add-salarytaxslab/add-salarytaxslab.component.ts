import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../../Service/notification.service';
import { ConstantService } from '../../../../../Service/constant.service';
import { SalaryTaxSlabService } from '../salarytaxslab.service';

@Component({
    selector: 'app-add-salarytaxslab',
    templateUrl: './add-salarytaxslab.component.html',
    styleUrl: './add-salarytaxslab.component.css',
    standalone: false
})

export class AddSalaryTaxSlabComponent {
  salarytaxslabForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  companyList: any;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private salarytaxslabService: SalaryTaxSlabService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

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
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.salarytaxslabForm);
    }
  }

  SaveData() {
    if (this.salarytaxslabForm.invalid) {
      this.constantService.markFormGroupTouched(this.salarytaxslabForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.salarytaxslabForm.value);
    this.salarytaxslabService.saveSalaryTaxSlab(_clienttemperatureForm).subscribe({
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
