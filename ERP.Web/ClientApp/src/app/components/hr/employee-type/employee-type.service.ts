import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { EmployeeTypeEndPoints } from './employee-type.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class EmployeeTypeService extends BaseService<any> {

    endPointControllerName = "EmployeeType";
    constructor(httpClient: HttpClient, private http: HttpClient, private employeeTypeEndPoints: EmployeeTypeEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllEmployeeTypes(employeeTypesFilterForm: any) {
        return this.post(employeeTypesFilterForm, this.endPointControllerName + this.employeeTypeEndPoints.getAllEmployeeTypes)
            .pipe(map((data: any) => data));
    }

    saveEmployeeType(saveEmployeeTypeCommand: any) {
        return this.post(saveEmployeeTypeCommand, this.endPointControllerName + this.employeeTypeEndPoints.saveEmployeeType)
            .pipe(map((data: any) => data));
    }

    deleteEmployeeType(id: number) {
        return this.delete(id, this.endPointControllerName + this.employeeTypeEndPoints.deleteEmployeeType)
            .pipe();
    }

    getEmployeeTypeById(id: number) {
        return this.get(id, this.endPointControllerName + this.employeeTypeEndPoints.getEmployeeTypeById)
            .pipe(map((data: any) => data));
    }

    getEmployeeTypeByName(name: string) {
        return this.get(name, this.endPointControllerName + this.employeeTypeEndPoints.getEmployeeTypeByName)
            .pipe(map((data: any) => data));
    }
}
