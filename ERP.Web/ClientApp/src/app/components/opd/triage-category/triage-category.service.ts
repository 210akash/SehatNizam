import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { TriageCategoryEndPoints } from './triage-category.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class TriageCategoryService extends BaseService<any> {

    endPointControllerName = 'TriageCategory';

    constructor(private http: HttpClient, httpClient: HttpClient, private triagecategoryEndPoints: TriageCategoryEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async saveTriageCategory(createTriageCategoryForm: any) {
        return await this.post(createTriageCategoryForm, this.endPointControllerName + this.triagecategoryEndPoints.saveTriageCategory)
            .pipe(map((data: any) => data));
    }

    async getAllTriageCategory(triagecategoryFilterForm: any) {
        return await this.post(triagecategoryFilterForm, this.endPointControllerName + this.triagecategoryEndPoints.getAllTriageCategory)
            .pipe(map((data: any) => data));
    }

    async deleteTriageCategory(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.triagecategoryEndPoints.deleteTriageCategory)
            .pipe(map((data: any) => data));
    }
}