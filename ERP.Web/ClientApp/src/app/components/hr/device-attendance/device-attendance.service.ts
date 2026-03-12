import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';
import { DeviceAttendanceEndPoints } from './device-attendance.endpoints';

@Injectable({
    providedIn: 'root'
})

export class DeviceAttendanceService extends BaseService<any> {

    endPointControllerName = "DeviceAttendance";
    constructor(httpClient: HttpClient, private http: HttpClient, private deviceAttendanceEndPoints: DeviceAttendanceEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    syncAttendanceByDate(fromDate: any, toDate: any) {
        return this.get('?fromDate=' + fromDate + '&toDate=' + toDate, this.endPointControllerName + this.deviceAttendanceEndPoints.syncAttendanceByDate)
            .pipe(map((data: any) => data));
    }

    syncAttendanceByEmployee(employeeId: any, fromDate: any, toDate: any) {
        return this.get('?employeeId=' + employeeId + '&fromDate=' + fromDate + '&toDate=' + toDate, this.endPointControllerName + this.deviceAttendanceEndPoints.syncAttendanceByEmployee)
            .pipe(map((data: any) => data));
    }
}
