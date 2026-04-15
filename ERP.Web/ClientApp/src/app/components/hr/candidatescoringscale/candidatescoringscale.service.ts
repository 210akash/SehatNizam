import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';
import { CandidateScoringScaleEndPoints } from './candidatescoringscale.endpoints';

@Injectable({
    providedIn: 'root'
})

export class CandidateScoringScaleService extends BaseService<any> {

    endPointControllerName = 'CandidateScoringScale';

    constructor(private http: HttpClient, httpClient: HttpClient, private candidateScoringScaleEndPoints: CandidateScoringScaleEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllCandidateScoringScales() {
        return this.get(this.endPointControllerName + this.candidateScoringScaleEndPoints.getAllCandidateScoringScales)
            .pipe(map((data: any) => data));
    }
}