import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { EmployeeDesignationEndPoints } from './employee-designation.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class EmployeeDesignationService extends BaseService<any> {

    endPointControllerName = "EmployeeDesignation";
    constructor(httpClient: HttpClient, private http: HttpClient, private employeeDesignationEndPoints: EmployeeDesignationEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllEmployeeDesignations(employeeDesignationsFilterForm: any) {
        return this.post(employeeDesignationsFilterForm, this.endPointControllerName + this.employeeDesignationEndPoints.getAllEmployeeDesignations)
            .pipe(map((data: any) => data));
    }

    saveEmployeeDesignation(saveEmployeeDesignationCommand: any) {
        return this.post(saveEmployeeDesignationCommand, this.endPointControllerName + this.employeeDesignationEndPoints.saveEmployeeDesignation)
            .pipe(map((data: any) => data));
    }

    deleteEmployeeDesignation(id: number) {
        return this.delete(id, this.endPointControllerName + this.employeeDesignationEndPoints.deleteEmployeeDesignation)
            .pipe();
    }

    getEmployeeDesignationById(id: number) {
        return this.get(id, this.endPointControllerName + this.employeeDesignationEndPoints.getEmployeeDesignationById)
            .pipe(map((data: any) => data));
    }

    getEmployeeDesignationByName(name: string) {
        return this.get(name, this.endPointControllerName + this.employeeDesignationEndPoints.getEmployeeDesignationByName)
            .pipe(map((data: any) => data));
    }
}
