import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { EmployeeShiftEndPoints } from './employee-shift.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class EmployeeShiftService extends BaseService<any> {

    endPointControllerName = "EmployeeShift";
    constructor(httpClient: HttpClient, private http: HttpClient, private employeeShiftEndPoints: EmployeeShiftEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllEmployeeShifts(employeeShiftsFilterForm: any) {
        return this.post(employeeShiftsFilterForm, this.endPointControllerName + this.employeeShiftEndPoints.getAllEmployeeShifts)
            .pipe(map((data: any) => data));
    }

    saveEmployeeShift(saveEmployeeShiftCommand: any) {
        return this.post(saveEmployeeShiftCommand, this.endPointControllerName + this.employeeShiftEndPoints.saveEmployeeShift)
            .pipe(map((data: any) => data));
    }

    deleteEmployeeShift(id: number) {
        return this.delete(id, this.endPointControllerName + this.employeeShiftEndPoints.deleteEmployeeShift)
            .pipe();
    }

    getEmployeeShiftById(id: number) {
        return this.get(id, this.endPointControllerName + this.employeeShiftEndPoints.getEmployeeShiftById)
            .pipe(map((data: any) => data));
    }

    getEmployeeShiftByName(name: string) {
        return this.get(name, this.endPointControllerName + this.employeeShiftEndPoints.getEmployeeShiftByName)
            .pipe(map((data: any) => data));
    }
}
