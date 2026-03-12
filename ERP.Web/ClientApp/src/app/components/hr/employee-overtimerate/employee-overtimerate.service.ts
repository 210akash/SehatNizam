import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { EmployeeOvertimeRateEndPoints } from './employee-overtimerate.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class EmployeeOvertimeRateService extends BaseService<any> {

    endPointControllerName = "EmployeeOvertimeRate";
    constructor(httpClient: HttpClient, private http: HttpClient, private employeeOvertimeRateEndPoints: EmployeeOvertimeRateEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllEmployeeOvertimeRates(employeeOvertimeRatesFilterForm: any) {
        return this.post(employeeOvertimeRatesFilterForm, this.endPointControllerName + this.employeeOvertimeRateEndPoints.getAllEmployeeOvertimeRates)
            .pipe(map((data: any) => data));
    }

    saveEmployeeOvertimeRate(saveEmployeeOvertimeRateCommand: any) {
        return this.post(saveEmployeeOvertimeRateCommand, this.endPointControllerName + this.employeeOvertimeRateEndPoints.saveEmployeeOvertimeRate)
            .pipe(map((data: any) => data));
    }

    deleteEmployeeOvertimeRate(id: number) {
        return this.delete(id, this.endPointControllerName + this.employeeOvertimeRateEndPoints.deleteEmployeeOvertimeRate)
            .pipe();
    }
}
