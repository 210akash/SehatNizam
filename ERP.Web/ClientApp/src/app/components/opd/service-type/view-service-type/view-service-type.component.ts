import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-view-service-type',
  templateUrl: './view-service-type.component.html',
  styleUrls: ['./view-service-type.component.css'],
  standalone: false
})
export class ViewServiceTypeComponent implements OnInit {
  form!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private constantServiceType: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      name: [''],
      isActive: [false]
    });
    this.constantServiceType.LoadData(this.data.element, this.form);
  }
}
