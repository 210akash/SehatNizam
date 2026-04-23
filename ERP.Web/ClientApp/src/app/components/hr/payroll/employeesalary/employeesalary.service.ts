import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../../../Service/base.service';
import { environment } from '../../../../../environments/environment';
import { EmployeeSalaryEndPoints } from './employeesalary.endpoints';

@Injectable({
    providedIn: 'root'
})
export class EmployeeSalaryService extends BaseService<any> {

    endPointControllerName = 'EmployeeSalary';
    constructor(httpClient: HttpClient, private employeeSalaryEndPoints: EmployeeSalaryEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    saveEmployeeSalary(saveEmployeeSalaryCommand: any) {
        return this.post(
            saveEmployeeSalaryCommand,
            this.endPointControllerName + this.employeeSalaryEndPoints.saveEmployeeSalary
        ).pipe(map((data: any) => data));
    }
}
