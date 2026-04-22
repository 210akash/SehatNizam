import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { SalaryHeadEndPoints } from './salaryhead.endpoints';
import { BaseService } from '../../../../Service/base.service';
import { environment } from '../../../../../environments/environment';

@Injectable({
    providedIn: 'root'
})

export class SalaryHeadService extends BaseService<any> {

    endPointControllerName = "SalaryHead";
    constructor(httpClient: HttpClient, private http: HttpClient, private salaryheadEndPoints: SalaryHeadEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllSalaryHeads(salaryHeadsFilterForm: any) {
        return this.post(salaryHeadsFilterForm, this.endPointControllerName + this.salaryheadEndPoints.getAllSalaryHeads)
            .pipe(map((data: any) => data));
    }

    saveSalaryHead(saveSalaryHeadCommand: any) {
        return this.post(saveSalaryHeadCommand, this.endPointControllerName + this.salaryheadEndPoints.saveSalaryHead)
            .pipe(map((data: any) => data));
    }

    deleteSalaryHead(id: number) {
        return this.delete(id, this.endPointControllerName + this.salaryheadEndPoints.deleteSalaryHead)
            .pipe();
    }

    getSalaryHeadById(id: number) {
        return this.get(id, this.endPointControllerName + this.salaryheadEndPoints.getSalaryHeadById)
            .pipe(map((data: any) => data));
    }

    getSalaryHeadByName(name: string) {
        return this.get(name, this.endPointControllerName + this.salaryheadEndPoints.getSalaryHeadByName)
            .pipe(map((data: any) => data));
    }
}
