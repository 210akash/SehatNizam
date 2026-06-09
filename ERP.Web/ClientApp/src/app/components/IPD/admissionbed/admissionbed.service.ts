import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';
import { AdmissionBedEndPoints } from './admissionbed.endpoints';


@Injectable({
    providedIn: 'root'
})
export class AdmissionBedService extends BaseService<any>{

    endPointControllerName = "AdmissionBed";
    constructor(httpClient: HttpClient, private http: HttpClient, private admissionBedEndPoints: AdmissionBedEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllAdmissionBeds(AdmissionBedFilterForm: any) {
        return this.post(AdmissionBedFilterForm, this.endPointControllerName + this.admissionBedEndPoints.getAllAdmissionBeds)
            .pipe(map((data: any) => data));
    }

    saveAdmissionBed(AdmissionBedFilterForm: any) {
        return this.post(AdmissionBedFilterForm, this.endPointControllerName + this.admissionBedEndPoints.saveAdmissionBed)
            .pipe(map((data: any) => data));
    }

    deleteAdmissionBed(id: number) {
      return this.delete(id, this.endPointControllerName + this.admissionBedEndPoints.deleteAdmissionBed)
          .pipe();
    }

    getAdmissionBedById(id: number) {
      return this.get(id, this.endPointControllerName + this.admissionBedEndPoints.getAdmissionBedById)
          .pipe(map((data: any) => data));
    }

    getAdmissionBedByName(name: string) {
      return this.get(name, this.endPointControllerName + this.admissionBedEndPoints.getAdmissionBedByName)
          .pipe(map((data: any) => data));
    }

}
