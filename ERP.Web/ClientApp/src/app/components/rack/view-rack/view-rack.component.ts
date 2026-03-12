import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-view-rack',
  standalone: false,
  templateUrl: './view-rack.component.html',
  styleUrl: './view-rack.component.css'
})
export class ViewRackComponent {

  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }
}
