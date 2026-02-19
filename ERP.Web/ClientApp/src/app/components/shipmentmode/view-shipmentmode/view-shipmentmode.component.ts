import { Component, Inject } from '@angular/core';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-shipmentmode',
    templateUrl: './view-shipmentmode.component.html',
    styleUrl: './view-shipmentmode.component.css',
    standalone: false
})

export class ViewShipmentModeComponent {
  isLoading = false;
  isEditMode: boolean = true;

  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }
}
