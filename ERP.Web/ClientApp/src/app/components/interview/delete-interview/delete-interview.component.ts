import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { InterviewService } from '../interview.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-delete-interview',
  templateUrl: './delete-interview.component.html',
  styleUrl: './delete-interview.component.css',
  standalone: false
})

export class DeleteInterviewComponent {
  isLoading = false;
  isEditMode: boolean = true;

  currentPage = 0;
  pageSize = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['interviewDate', 'attendees', 'joinAfterDays', 'comments', 'status', 'createdDate', 'createdBy'];
  dataSource: any;
  take = 50;
  totalRows = 0;
  documents: any[] = [];

  urlSafe: SafeResourceUrl | undefined;
  dialogRef: any;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService,
    private interviewService: InterviewService, private sanitizer: DomSanitizer, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.dataSource = this.data.element?.interviewHistory;
    this.documents = this.data.element?.attachments;
  }

  pageChanged(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
  }

  getAttendeesString(element: any) {
    return element.interviewAttendees
      .map((attendee: any) => {
        const user = attendee.aspNetUsers;
        return user ? `${user.firstName} ${user.lastName}` : null;
      })
      .filter((name: string | null) => !!name)
      .join(', ');

    return '';
  }

  GetDocument(event: any, index: number, template: any) {
    this.urlSafe = this.sanitizer.bypassSecurityTrustResourceUrl(this.documents[index].fileSource + '#toolbar=0');
    this.dialogRef = this.dialog.open(template, {
      width: '50%',
      height: '70%',
      disableClose: true,
    });
  }

  isImage(filePath: string): boolean {
    if (!filePath) return false;
    const ext = filePath.split('.').pop()?.toLowerCase();
    return ['jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp', 'svg'].includes(ext || '');
  }

  isPdf(filePath: string): boolean {
    return filePath?.toLowerCase().endsWith('.pdf');
  }

  async delete() {
    (await this.interviewService.deleteInterview(this.data.element.id)).subscribe({
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