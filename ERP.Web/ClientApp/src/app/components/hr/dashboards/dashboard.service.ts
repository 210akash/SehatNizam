import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';
import { DashboardEndPoints } from './dashboard.endpoints';

@Injectable({
    providedIn: 'root'
})

export class DashboardService extends BaseService<any> {

    endPointControllerName = "Dashboard";
    constructor(httpClient: HttpClient, private http: HttpClient, private dashboardEndPoints: DashboardEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getHRDashboardData() {
        return this.get(this.endPointControllerName + this.dashboardEndPoints.getHRDashboardData)
            .pipe(map((data: any) => data));
    }

    getTodayAttendance() {
        return this.get(this.endPointControllerName + this.dashboardEndPoints.getTodayAttendance)
            .pipe(map((data: any) => data));
    }

    getTodayInterviews() {
        return this.get(this.endPointControllerName + this.dashboardEndPoints.getTodayInterviews)
            .pipe(map((data: any) => data));
    }


}