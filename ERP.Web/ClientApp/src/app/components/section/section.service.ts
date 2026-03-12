import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BaseService } from '../../Service/base.service';
import { SectionEndPoints } from './section.endpoints';

@Injectable({
    providedIn: 'root'
})

export class SectionService extends BaseService<any> {

    endPointControllerName = 'Section';

    constructor(private http: HttpClient, httpClient: HttpClient, private sectionEndPoints: SectionEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getSectionByName(name: string) {
        return await this.get('?name=' + name, this.endPointControllerName + this.sectionEndPoints.getSectionByName)
            .pipe(map((data: any) => data));
    }

    async saveSection(createSectionForm: any) {
        return await this.post(createSectionForm, this.endPointControllerName + this.sectionEndPoints.saveSection)
            .pipe(map((data: any) => data));
    }

    async getAllSection(SectionFilterForm: any) {
        return await this.post(SectionFilterForm, this.endPointControllerName + this.sectionEndPoints.getAllSection)
            .pipe(map((data: any) => data));
    }

    async getSectionByRowId(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.sectionEndPoints.getSectionByRowId)
            .pipe(map((data: any) => data));
    }

    async deleteSection(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.sectionEndPoints.deleteSection)
            .pipe(map((data: any) => data));
    }
}
