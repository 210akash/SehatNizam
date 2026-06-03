import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-view-referrer',
  templateUrl: './view-referrer.component.html',
  styleUrls: ['./view-referrer.component.css'],
  standalone: false
})
export class ViewReferrerComponent implements OnInit {
  form!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private constantReferrer: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      name: [''],
      isActive: [false]
    });
    this.constantReferrer.LoadData(this.data.element, this.form);
  }
}
