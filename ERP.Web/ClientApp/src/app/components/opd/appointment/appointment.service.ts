import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../../Service/base.service';
import { environment } from '../../../../environments/environment';
import { AppointmentEndPoints } from './appointment.endpoints';

@Injectable({
    providedIn: 'root'
})

export class AppointmentService extends BaseService<any> {

    endPointControllerName = "Appointment";
    constructor(httpClient: HttpClient, private http: HttpClient, private AppointmentEndPoints: AppointmentEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllAppointments(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.AppointmentEndPoints.getAllAppointments)
            .pipe(map((data: any) => data));
    }

    getAllAppointmentByDoctor(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.AppointmentEndPoints.getAllAppointmentByDoctor)
            .pipe(map((data: any) => data));
    }

    saveAppointment(saveAppointmentCommand: any) {
        return this.post(saveAppointmentCommand, this.endPointControllerName + this.AppointmentEndPoints.saveAppointment)
            .pipe(map((data: any) => data));
    }

    deleteAppointment(id: number) {
        return this.delete(id, this.endPointControllerName + this.AppointmentEndPoints.deleteAppointment)
            .pipe();
    }

    getAppointmentById(id: number) {
        return this.get(id, this.endPointControllerName + this.AppointmentEndPoints.getAppointmentById)
            .pipe(map((data: any) => data));
    }

    getAppointmentByName(name: string) {
        return this.get(name, this.endPointControllerName + this.AppointmentEndPoints.getAppointmentByName)
            .pipe(map((data: any) => data));
    }

     getAppointmentByToken(token: string) {
        return this.get('?token=' +  token, this.endPointControllerName + this.AppointmentEndPoints.getAppointmentByToken)
            .pipe(map((data: any) => data));
    }


        getAllAppointmentStatus() {
        return this.get(this.endPointControllerName + this.AppointmentEndPoints.getAllAppointmentStatus)
            .pipe(map((data: any) => data));
    }
}
