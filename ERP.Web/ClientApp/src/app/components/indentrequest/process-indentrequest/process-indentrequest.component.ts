import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { IndentrequestService } from '../indentrequest.service';

@Component({
    selector: 'app-process-indentrequest',
    templateUrl: './process-indentrequest.component.html',
    styleUrl: './process-indentrequest.component.css',
    standalone: false
})

export class ProcessIndentrequestComponent {
  indentrequestForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog, private formBuilder: FormBuilder, private notificationsService: NotificationsService, private indentrequestService: IndentrequestService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.indentrequestForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      requiredDate: ['', Validators.required],
      createdDate: [new Date()],
      departmentId: [0, Validators.required],
      status: [''], // Validation
      statusName: ['New'], // Validation
      statusId: [1], // Validation
      indentRequestDetail: this.formBuilder.array([]) // Initialize as a FormArray
    });
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (this.data.element.id != null) {
      this.isEditMode = true;
    }
    this.constantService.LoadData(element, this.indentrequestForm);
  }

  async process() {
    (await this.indentrequestService.processIndentrequest(this.data.element.id)).subscribe({
      next: (data) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification('Indent proceed successfully', 'snack-bar-success');
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
