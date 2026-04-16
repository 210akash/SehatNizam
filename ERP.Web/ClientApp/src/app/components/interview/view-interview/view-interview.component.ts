import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-view-interview',
  templateUrl: './view-interview.component.html',
  styleUrl: './view-interview.component.css',
  standalone: false
})

export class ViewInterviewComponent {
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

  constructor(private sanitizer: DomSanitizer, private dialog: MatDialog, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

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

getScorePercentage(scaleId: number): number {
  switch (scaleId) {
    case 1: return 20;  // Poor
    case 2: return 40;  // Below Avg
    case 3: return 60;  // Average
    case 4: return 80;  // Good
    case 5: return 100; // Excellent
    default: return 0;
  }
}

}