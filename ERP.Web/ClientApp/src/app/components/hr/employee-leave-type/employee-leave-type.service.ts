import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { EmployeeLeaveTypeEndPoints } from './employee-leave-type.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class EmployeeLeaveTypeService extends BaseService<any> {

    endPointControllerName = "EmployeeLeaveType";
    constructor(httpClient: HttpClient, private http: HttpClient, private employeeLeaveTypeEndPoints: EmployeeLeaveTypeEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllEmployeeLeaveTypes(employeeLeaveTypesFilterForm: any) {
        return this.post(employeeLeaveTypesFilterForm, this.endPointControllerName + this.employeeLeaveTypeEndPoints.getAllEmployeeLeaveTypes)
            .pipe(map((data: any) => data));
    }

    saveEmployeeLeaveType(saveEmployeeLeaveTypeCommand: any) {
        return this.post(saveEmployeeLeaveTypeCommand, this.endPointControllerName + this.employeeLeaveTypeEndPoints.saveEmployeeLeaveType)
            .pipe(map((data: any) => data));
    }

    deleteEmployeeLeaveType(id: number) {
        return this.delete(id, this.endPointControllerName + this.employeeLeaveTypeEndPoints.deleteEmployeeLeaveType)
            .pipe();
    }

    getEmployeeLeaveTypeById(id: number) {
        return this.get(id, this.endPointControllerName + this.employeeLeaveTypeEndPoints.getEmployeeLeaveTypeById)
            .pipe(map((data: any) => data));
    }

    getEmployeeLeaveTypeByName(name: string) {
        return this.get(name, this.endPointControllerName + this.employeeLeaveTypeEndPoints.getEmployeeLeaveTypeByName)
            .pipe(map((data: any) => data));
    }
}
