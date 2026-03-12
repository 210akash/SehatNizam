import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { HolidayService } from '../holiday.service';

@Component({
    selector: 'app-add-holiday',
    templateUrl: './add-holiday.component.html',
    styleUrl: './add-holiday.component.css',
    standalone: false
})

export class AddHolidayComponent {
  holidayForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  companyList: any;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private holidayService: HolidayService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.holidayForm = this.formBuilder.group({
      id: [0],
      title: ['', Validators.required],
      date: [new Date(), [Validators.required, Validators.min(0)]],
      description: ['', Validators.required],
    });
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.holidayForm);
    }
  }

  SaveData() {
    if (this.holidayForm.invalid) {
      this.constantService.markFormGroupTouched(this.holidayForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.holidayForm.value);


        let date = new Date(this.holidayForm.get('date')?.value);
    _clienttemperatureForm['date'] = date.toLocaleDateString();

    this.holidayService.saveHoliday(_clienttemperatureForm).subscribe({
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
