import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { SalaryTaxSlabEndPoints } from './salarytaxslab.endpoints';
import { BaseService } from '../../../../Service/base.service';
import { environment } from '../../../../../environments/environment';

@Injectable({
    providedIn: 'root'
})

export class SalaryTaxSlabService extends BaseService<any> {

    endPointControllerName = "SalaryTaxSlab";
    constructor(httpClient: HttpClient, private http: HttpClient, private salarytaxslabEndPoints: SalaryTaxSlabEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllSalaryTaxSlab(citiesFilterForm: any) {
        return this.post(citiesFilterForm, this.endPointControllerName + this.salarytaxslabEndPoints.getAllSalaryTaxSlab)
            .pipe(map((data: any) => data));
    }

    saveSalaryTaxSlab(saveSalaryTaxSlabCommand: any) {
        return this.post(saveSalaryTaxSlabCommand, this.endPointControllerName + this.salarytaxslabEndPoints.saveSalaryTaxSlab)
            .pipe(map((data: any) => data));
    }

    deleteSalaryTaxSlab(id: number) {
        return this.delete(id, this.endPointControllerName + this.salarytaxslabEndPoints.deleteSalaryTaxSlab)
            .pipe();
    }

    getSalaryTaxSlabById(id: number) {
        return this.get(id, this.endPointControllerName + this.salarytaxslabEndPoints.getSalaryTaxSlabById)
            .pipe(map((data: any) => data));
    }
}
