import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-view-sugar-type',
  templateUrl: './view-sugar-type.component.html',
  styleUrls: ['./view-sugar-type.component.css'],standalone: false
})

export class ViewSugarTypeComponent implements OnInit {
  viewSugarTypeForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  roleList: any;
  constructor(private dialog: MatDialog, private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.viewSugarTypeForm = this.formBuilder.group({
      id: [0],
      name: [''],
    });

    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.viewSugarTypeForm);
  }


}
