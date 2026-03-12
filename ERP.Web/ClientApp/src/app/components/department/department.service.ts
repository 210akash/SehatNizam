import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { DepartmentEndPoints } from './department.endpoints';

@Injectable({
    providedIn: 'root'
})

export class DepartmentService extends BaseService<any> {

    endPointControllerName = "Department";
    constructor(httpClient: HttpClient, private http: HttpClient, private departmentEndPoints: DepartmentEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllDepartments(departmentsFilterForm: any) {
        return this.post(departmentsFilterForm, this.endPointControllerName + this.departmentEndPoints.getAllDepartments)
            .pipe(map((data: any) => data));
    }

    saveDepartment(saveDepartmentCommand: any) {
        return this.post(saveDepartmentCommand, this.endPointControllerName + this.departmentEndPoints.saveDepartment)
            .pipe(map((data: any) => data));
    }

    deleteDepartment(id: number) {
        return this.delete(id, this.endPointControllerName + this.departmentEndPoints.deleteDepartment)
            .pipe();
    }

    getDepartmentById(id: number) {
        return this.get(id, this.endPointControllerName + this.departmentEndPoints.getDepartmentById)
            .pipe(map((data: any) => data));
    }

    getDepartmentByName(name: string) {
        return this.get(name, this.endPointControllerName + this.departmentEndPoints.getDepartmentByName)
            .pipe(map((data: any) => data));
    }

    getDepartmentByCompany(companyId: string) {
        return this.get('?CompanyId=' + companyId, this.endPointControllerName + this.departmentEndPoints.getDepartmentByCompany)
            .pipe(map((data: any) => data));
    }
}
