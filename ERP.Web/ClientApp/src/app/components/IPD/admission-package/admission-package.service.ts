import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../../Service/base.service';
import { environment } from '../../../../environments/environment';
import { AdmissionPackageEndPoints } from './admission-package.endpoints';

@Injectable({
    providedIn: 'root'
})
export class AdmissionPackageService extends BaseService<any> {

    endPointControllerName = "AdmissionPackageMaster";

    constructor(httpClient: HttpClient, private admissionPackageEndPoints: AdmissionPackageEndPoints) {
        super(httpClient, environment.dev_uri);
    }

    getAllAdmissionPackages(filterForm: any) {
        return this.post(filterForm, this.endPointControllerName + this.admissionPackageEndPoints.getAllAdmissionPackages)
            .pipe(map((data: any) => data));
    }

    getAdmissionPackageById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.admissionPackageEndPoints.getAdmissionPackageById)
            .pipe(map((data: any) => data));
    }

    saveAdmissionPackage(command: any) {
        return this.post(command, this.endPointControllerName + this.admissionPackageEndPoints.saveAdmissionPackage)
            .pipe(map((data: any) => data));
    }

    deleteAdmissionPackage(id: number) {
        return this.delete(id, this.endPointControllerName + this.admissionPackageEndPoints.deleteAdmissionPackage)
            .pipe();
    }
}
