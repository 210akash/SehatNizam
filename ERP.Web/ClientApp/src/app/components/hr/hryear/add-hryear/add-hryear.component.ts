import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { HRYearService } from '../hryear.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
    selector: 'app-add-hryear',
    templateUrl: './add-hryear.component.html',
    styleUrl: './add-hryear.component.css',
    standalone: false
})

export class AddHRYearComponent {
  hryearForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  companyList: any;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private hryearService: HRYearService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.hryearForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
      startDate: [new Date(), [Validators.required, Validators.min(0)]],
      endDate: [new Date(), [Validators.required, Validators.min(0)]],
    });
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.hryearForm);
    }
    else{
       this.dateChange();
    }
  }

  SaveData() {
    if (this.hryearForm.invalid) {
      this.constantService.markFormGroupTouched(this.hryearForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.hryearForm.value);


        let startDate = new Date(this.hryearForm.get('startDate')?.value);
    _clienttemperatureForm['startDate'] = startDate.toLocaleString();

            let endDate = new Date(this.hryearForm.get('endDate')?.value);
    _clienttemperatureForm['endDate'] = endDate.toLocaleString();

    this.hryearService.saveHRYear(_clienttemperatureForm).subscribe({
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

  dateChange() {
    const startDate = this.hryearForm.get('startDate')?.value;
    const endDate = this.hryearForm.get('endDate')?.value;

    if (startDate && endDate) {
      const formattedStartDate = this.constantService.formatDate(startDate);
      const formattedEndDate = this.constantService.formatDate(endDate);

      // Set the 'name' as "YYYY-MM-DD - YYYY-MM-DD"
      this.hryearForm.get('name')?.setValue(`${formattedStartDate} - ${formattedEndDate}`);
    }
  }
}
