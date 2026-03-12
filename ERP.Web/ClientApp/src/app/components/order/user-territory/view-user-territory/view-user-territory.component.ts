import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';


@Component({
  selector: 'app-view-user-territory',
  templateUrl: './view-user-territory.component.html',
  styleUrls: ['./view-user-territory.component.css'],standalone: false
})

export class ViewUserTerritoryComponent implements OnInit {
  viewUserTerritory!: FormGroup;
  isLoading = false;
  territory: any = null;

  constructor(private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.viewUserTerritory = this.formBuilder.group({
      id: [0],
      region: [''],
      zone: [''],
      area: [''],
      territory: [''],
      user: [''],
      role: ['']
    });

    this.LoadData(this.data.element);
  }

  get f() {
    return this.viewUserTerritory.controls;
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.viewUserTerritory);
    var FullName = element.user?.firstName + ' ' + element.user?.lastName;
    this.viewUserTerritory.get('role')?.patchValue(element.user?.aspNetUserRoles[0]?.role?.name);
    this.viewUserTerritory.get('user')?.patchValue(FullName);
    this.viewUserTerritory.get('region')?.patchValue(element.region?.name);
    this.viewUserTerritory.get('zone')?.patchValue(element.zone?.name);
    this.viewUserTerritory.get('area')?.patchValue(element.area?.name);
    this.territory = element.isAllTerritoryCheck === true ? 'All Territories' : element.territory?.name;
    this.viewUserTerritory.get('territory')?.patchValue(this.territory);
  }
}
