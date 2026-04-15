import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { CandidateEvaluationCategoryEndPoints } from './candidateevaluationcategory.endpoints';
import { BaseService } from '../../../Service/base.service';
import { environment } from '../../../../environments/environment';

@Injectable({
    providedIn: 'root'
})

export class CandidateEvaluationCategoryService extends BaseService<any> {

    endPointControllerName = "CandidateEvaluationCategory";
    constructor(httpClient: HttpClient, private http: HttpClient, private candidateevaluationcategoryEndPoints: CandidateEvaluationCategoryEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllCandidateEvaluationCategorys(candidateevaluationcategorysFilterForm: any) {
        return this.post(candidateevaluationcategorysFilterForm, this.endPointControllerName + this.candidateevaluationcategoryEndPoints.getAllCandidateEvaluationCategorys)
            .pipe(map((data: any) => data));
    }

    saveCandidateEvaluationCategory(saveCandidateEvaluationCategoryCommand: any) {
        return this.post(saveCandidateEvaluationCategoryCommand, this.endPointControllerName + this.candidateevaluationcategoryEndPoints.saveCandidateEvaluationCategory)
            .pipe(map((data: any) => data));
    }

    deleteCandidateEvaluationCategory(id: number) {
        return this.delete(id, this.endPointControllerName + this.candidateevaluationcategoryEndPoints.deleteCandidateEvaluationCategory)
            .pipe();
    }
}
