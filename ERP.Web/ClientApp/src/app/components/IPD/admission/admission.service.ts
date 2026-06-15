import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../../Service/base.service';
import { environment } from '../../../../environments/environment';
import { AdmissionEndPoints } from './admission.endpoints';

@Injectable({
    providedIn: 'root'
})

export class AdmissionService extends BaseService<any> {

    endPointControllerName = "Admission";
    constructor(httpClient: HttpClient, private http: HttpClient, private AdmissionEndPoints: AdmissionEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllAdmissions(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.AdmissionEndPoints.getAllAdmissions)
            .pipe(map((data: any) => data));
    }

    getAllAdmissionByDoctor(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.AdmissionEndPoints.getAllAdmissionByDoctor)
            .pipe(map((data: any) => data));
    }

    saveAdmission(saveAdmissionCommand: any) {
        return this.post(saveAdmissionCommand, this.endPointControllerName + this.AdmissionEndPoints.saveAdmission)
            .pipe(map((data: any) => data));
    }

    deleteAdmission(id: number) {
        return this.delete(id, this.endPointControllerName + this.AdmissionEndPoints.deleteAdmission)
            .pipe();
    }

    getAdmissionById(id: number) {
        return this.get(id, this.endPointControllerName + this.AdmissionEndPoints.getAdmissionById)
            .pipe(map((data: any) => data));
    }

    getAdmissionByName(name: string) {
        return this.get(name, this.endPointControllerName + this.AdmissionEndPoints.getAdmissionByName)
            .pipe(map((data: any) => data));
    }

    getAdmissionByToken(token: string, statusId: any) {
        return this.get('?token=' + token + '&statusId=' + statusId, this.endPointControllerName + this.AdmissionEndPoints.getAdmissionByToken)
            .pipe(map((data: any) => data));
    }


    getAllAdmissionStatus() {
        return this.get(this.endPointControllerName + this.AdmissionEndPoints.getAllAdmissionStatus)
            .pipe(map((data: any) => data));
    }

    saveConsultation(saveConsultationCommand: any) {
        return this.post(saveConsultationCommand, this.endPointControllerName + this.AdmissionEndPoints.saveConsultation)
            .pipe(map((data: any) => data));
    }

    confirmAdmission(saveConsultationCommand: any) {
        return this.post(saveConsultationCommand, this.endPointControllerName + this.AdmissionEndPoints.confirmAdmission)
            .pipe(map((data: any) => data));
    }

    cancelAppoinment(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.AdmissionEndPoints.cancelAppoinment)
            .pipe(map((data: any) => data));
    }

     getAdmissionsByBookingNo(token: string) {
        return this.get('?bookingNo=' + token, this.endPointControllerName + this.AdmissionEndPoints.getAdmissionsByBookingNo)
            .pipe(map((data: any) => data));
    }

      saveDischarge(saveAdmissionCommand: any) {
        return this.post(saveAdmissionCommand, this.endPointControllerName + this.AdmissionEndPoints.saveDischarge)
            .pipe(map((data: any) => data));
    }

}
