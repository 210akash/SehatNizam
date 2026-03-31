import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { AppointmentTypeEndPoints } from './appointment-type.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class AppointmentTypeService extends BaseService<any> {

    endPointControllerName = 'AppointmentType';

    constructor(private http: HttpClient, httpClient: HttpClient, private appointmentTypeEndPoints: AppointmentTypeEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async saveAppointmentType(createAppointmentTypeForm: any) {
        return await this.post(createAppointmentTypeForm, this.endPointControllerName + this.appointmentTypeEndPoints.saveAppointmentType)
            .pipe(map((data: any) => data));
    }

    async getAllAppointmentType(appointmentTypeFilterForm: any) {
        return await this.post(appointmentTypeFilterForm, this.endPointControllerName + this.appointmentTypeEndPoints.getAllAppointmentType)
            .pipe(map((data: any) => data));
    }

    async deleteAppointmentType(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.appointmentTypeEndPoints.deleteAppointmentType)
            .pipe(map((data: any) => data));
    }
}