import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { EmployeeDeviceEndPoints } from './employee-device.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class EmployeeDeviceService extends BaseService<any> {

    endPointControllerName = "EmployeeDevice";
    constructor(httpClient: HttpClient, private http: HttpClient, private employeeDeviceEndPoints: EmployeeDeviceEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    saveEmployeeDevice(saveEmployeeDeviceCommand: any) {
        return this.post(saveEmployeeDeviceCommand, this.endPointControllerName + this.employeeDeviceEndPoints.saveEmployeeDevice)
            .pipe(map((data: any) => data));
    }

    getEmployeeDevice(getDevicesByEmployee: any) {
        return this.post(getDevicesByEmployee, this.endPointControllerName + this.employeeDeviceEndPoints.getEmployeeDevice)
            .pipe(map((data: any) => data));
    }
}
