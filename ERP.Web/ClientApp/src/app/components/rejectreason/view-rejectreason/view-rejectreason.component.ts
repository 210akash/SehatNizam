import { Component, Inject } from '@angular/core';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-rejectreason',
    templateUrl: './view-rejectreason.component.html',
    styleUrl: './view-rejectreason.component.css',
    standalone: false
})

export class ViewRejectReasonComponent {
  isLoading = false;
  isEditMode: boolean = true;

  constructor(private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }
}
