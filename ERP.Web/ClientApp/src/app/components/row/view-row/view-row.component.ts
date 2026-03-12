import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-view-row',
  standalone: false,
  templateUrl: './view-row.component.html',
  styleUrl: './view-row.component.css'
})
export class ViewRowComponent {

  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }
}
