import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { EmployeeEndPoints } from './employee.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class EmployeeService extends BaseService<any> {

    endPointControllerName = "Employee";
    constructor(httpClient: HttpClient, private http: HttpClient, private employeeEndPoints: EmployeeEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getEmployeeByName(employeeFilterForm: any) {
        return this.post(employeeFilterForm, this.endPointControllerName + this.employeeEndPoints.getEmployeeByName)
            .pipe(map((data: any) => data));
    }

      getEmployeeByDepartment(departmentId:number) {
        return this.get('?departmentId=' + departmentId, this.endPointControllerName + this.employeeEndPoints.getEmployeeByDepartment)
            .pipe(map((data: any) => data));
    }

      getEmployeeByDepartmentManager() {
        return this.get(this.endPointControllerName + this.employeeEndPoints.getEmployeeByDepartmentManager)
            .pipe(map((data: any) => data));
    }
}
