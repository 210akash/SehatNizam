import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { HolidayService } from '../holiday.service';

@Component({
    selector: 'app-delete-holiday',
    templateUrl: './delete-holiday.component.html',
    styleUrl: './delete-holiday.component.css',
    standalone: false
})

export class DeleteHolidayComponent {
  holidayForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog, private formBuilder: FormBuilder, private notificationsService: NotificationsService, private holidayService: HolidayService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

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
    if (this.data.element.id != null) {
      this.isEditMode = true;
    }
    this.constantService.LoadData(element, this.holidayForm);
  }

  async delete() {
    (await this.holidayService.deleteHoliday(this.data.element.id)).subscribe({
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
