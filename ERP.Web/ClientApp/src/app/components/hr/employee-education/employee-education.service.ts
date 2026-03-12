import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { EmployeeEducationEndPoints } from './employee-education.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class EmployeeEducationService extends BaseService<any> {

    endPointControllerName = "EmployeeEducation";
    constructor(httpClient: HttpClient, private http: HttpClient, private employeeEducationEndPoints: EmployeeEducationEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllEmployeeEducations(employeeEducationsFilterForm: any) {
        return this.post(employeeEducationsFilterForm, this.endPointControllerName + this.employeeEducationEndPoints.getAllEmployeeEducations)
            .pipe(map((data: any) => data));
    }

    saveEmployeeEducation(saveEmployeeEducationCommand: any) {
        return this.post(saveEmployeeEducationCommand, this.endPointControllerName + this.employeeEducationEndPoints.saveEmployeeEducation)
            .pipe(map((data: any) => data));
    }

    deleteEmployeeEducation(id: number) {
        return this.delete(id, this.endPointControllerName + this.employeeEducationEndPoints.deleteEmployeeEducation)
            .pipe();
    }

    getEmployeeEducationById(id: number) {
        return this.get(id, this.endPointControllerName + this.employeeEducationEndPoints.getEmployeeEducationById)
            .pipe(map((data: any) => data));
    }

    getEmployeeEducationByName(name: string) {
        return this.get(name, this.endPointControllerName + this.employeeEducationEndPoints.getEmployeeEducationByName)
            .pipe(map((data: any) => data));
    }
}
