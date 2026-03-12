import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-accountflow',
    templateUrl: './view-accountflow.component.html',
    styleUrl: './view-accountflow.component.css',
    standalone: false
})

export class ViewAccountFlowComponent {
  isLoading = false;
  isEditMode: boolean = true;

  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }
}
