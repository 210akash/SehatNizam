import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { TriageService } from '../triage.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-delete-triage',
  templateUrl: './delete-triage.component.html',
  styleUrls: ['./delete-triage.component.css'],standalone: false
})
export class DeleteTriageComponent implements OnInit {
  deleteTriageForm!: FormGroup;
  isLoading = false;

  constructor(
    private notificationsService: NotificationsService,
    private dialog: MatDialog,
    private triageService: TriageService,
    private formBuilder: FormBuilder,
    private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.deleteTriageForm = this.formBuilder.group({
      id: [0],
      appointmentId: ['']
    });

    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.deleteTriageForm);
  }

  async delete() {
    this.isLoading = true;
    (await this.triageService.deleteTriage(this.data.element.id)).subscribe({
      next: (data: { Status: number; Message: string; }) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Message || 'Triage deleted successfully', 'snack-bar-success');
          this.dialog.closeAll();
        }
        else {
          this.notificationsService.showNotification(data.Message || 'Unable to delete triage', 'snack-bar-danger');
        }
        this.isLoading = false;
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }
}
