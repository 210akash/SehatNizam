import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { CompanyEndPoints } from './company.endpoints';

@Injectable({
    providedIn: 'root'
})

export class CompanyService extends BaseService<any> {

    endPointControllerName = "Company";
    constructor(httpClient: HttpClient, private http: HttpClient, private companyEndPoints: CompanyEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllCompanys(companysFilterForm: any) {
        return this.post(companysFilterForm, this.endPointControllerName + this.companyEndPoints.getAllCompanys)
            .pipe(map((data: any) => data));
    }

    saveCompany(saveCompanyCommand: any) {
        return this.post(saveCompanyCommand, this.endPointControllerName + this.companyEndPoints.saveCompany)
            .pipe(map((data: any) => data));
    }

    deleteCompany(id: number) {
        return this.delete(id, this.endPointControllerName + this.companyEndPoints.deleteCompany)
            .pipe();
    }

    getCompanyById(id: number) {
        return this.get(id, this.endPointControllerName + this.companyEndPoints.getCompanyById)
            .pipe(map((data: any) => data));
    }

    getCompanyByName(name: string) {
        return this.get(name, this.endPointControllerName + this.companyEndPoints.getCompanyByName)
            .pipe(map((data: any) => data));
    }

}
