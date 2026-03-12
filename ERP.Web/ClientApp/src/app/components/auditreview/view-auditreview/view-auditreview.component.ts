import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-auditreview',
    templateUrl: './view-auditreview.component.html',
    styleUrl: './view-auditreview.component.css',
    standalone: false
})

export class ViewAuditReviewComponent {
  isLoading = false;
  isEditMode: boolean = true;

  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }
  ngOnInit(): void {
    console.log(this.data.element);
  }
}
