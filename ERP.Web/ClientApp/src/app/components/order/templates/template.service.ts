import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { TemplateEndPoints } from './template.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class TemplateService extends BaseService<any> {

    endPointControllerName = 'Templates';

    constructor(private http: HttpClient, httpClient: HttpClient, private templateEndPoints: TemplateEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async save(templateForm: string) {
        return await this.post(templateForm, this.endPointControllerName + this.templateEndPoints.save)
            .pipe(map((data: any) => data));
    }

    async getAllTemplates(tempalteFilterForm: any) {
        return this.post(tempalteFilterForm, this.endPointControllerName + this.templateEndPoints.getAll)
            .pipe(map((data: any) => data));
    }

    async getTemplateById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.templateEndPoints.getById)
            .pipe(map((data: any) => data));
    }

    async getPrintTemplate(orderId: number, templateId: any) {
        return this.get('?orderId=' + orderId + '&templateId=' + templateId, this.endPointControllerName + this.templateEndPoints.getPrintTemplate)
            .pipe(map((data: any) => data));
    }
    async getPrintTemplateByShopId(shopId: number, templateId: any) {
        return this.get('?shopId=' + shopId + '&templateId=' + templateId, this.endPointControllerName + this.templateEndPoints.getPrintTemplateByShopId)
            .pipe(map((data: any) => data));
    }

    
}