import { Injectable } from '@angular/core';
import { HttpBackend , HttpClient , HttpHeaders} from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { UserAttendanceEndPoints } from './user-attendance.endpoints';
import { BaseService } from '../../../Service/base.service';
import { environment } from '../../../../environments/environment';

@Injectable({
    providedIn: 'root'
})

export class UserAttendanceService extends BaseService<any> {

    endPointControllerName = 'UserAttendance';
    reportEndPointControllerName = 'Reports';

    constructor(private httpBackend: HttpBackend,private http: HttpClient, httpClient: HttpClient, private userAttendanceEndPoints: UserAttendanceEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async saveUserAttendance(createDeviceRegistratonForm: any) {
        return await this.post(createDeviceRegistratonForm, this.endPointControllerName + this.userAttendanceEndPoints.saveUserAttendance)
            .pipe(map((data: any) => data));
    }

    async getAllUserAttendance(deviceRegistratonFilterForm: any) {
        return await this.post(deviceRegistratonFilterForm, this.endPointControllerName + this.userAttendanceEndPoints.getAllUserAttendance)
            .pipe(map((data: any) => data));
    }

    async getUserAttendanceById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.userAttendanceEndPoints.getUserAttendanceById)
            .pipe(map((data: any) => data));
    }

    async getUserAttendanceByUser(getUserAttendanceByUserFilterForm: any) {
        return await this.post(getUserAttendanceByUserFilterForm, this.endPointControllerName + this.userAttendanceEndPoints.getUserAttendanceByUser)
            .pipe(map((data: any) => data));
    }

    async deleteUserAttendance(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.userAttendanceEndPoints.deleteUserAttendance)
            .pipe(map((data: any) => data));
    }

    async getZonesByUserInAttendance(userId: any, roleId: any) {
        return this.get('?userId=' + userId + '&roleId=' + roleId, this.endPointControllerName + this.userAttendanceEndPoints.getZonesByUserInAttendance)
            .pipe(map((data: any) => data));
    }

    getReportDemo(fDate: string, tDate: string): Observable<Blob> {
        const headers = new HttpHeaders()
            .append('Content-Type', 'application/json')
            .append('Access-Control-Allow-Headers', 'Content-Type')
            .append('Access-Control-Allow-Methods', 'GET')
            .append('Access-Control-Allow-Origin', '*');
            

        const baseUrl = environment.dev_uri; 
    
        return this.http.get(`${baseUrl}/Reports/DemoReportGet`, {
            observe: 'body',
            responseType: 'blob', // Specify response type as 'blob'
            headers: headers,
            params: {
                fDate: fDate,
                tDate: tDate
            },
        });
    }
    
}