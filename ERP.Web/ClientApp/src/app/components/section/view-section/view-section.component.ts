import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-view-section',
  standalone: false,
  templateUrl: './view-section.component.html',
  styleUrl: './view-section.component.css'
})
export class ViewSectionComponent {

  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }
}
