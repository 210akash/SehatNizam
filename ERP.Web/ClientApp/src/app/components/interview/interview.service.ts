import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { InterviewEndPoints } from './interview.endpoints';

@Injectable({
    providedIn: 'root'
})

export class InterviewService extends BaseService<any> {

    endPointControllerName = "Interview";
    constructor(httpClient: HttpClient, private http: HttpClient, private interviewEndPoints: InterviewEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllInterviews(interviewsFilterForm: any) {
        return this.post(interviewsFilterForm, this.endPointControllerName + this.interviewEndPoints.getAllInterviews)
            .pipe(map((data: any) => data));
    }

    saveInterview(saveInterviewCommand: any) {
        return this.post(saveInterviewCommand, this.endPointControllerName + this.interviewEndPoints.saveInterview)
            .pipe(map((data: any) => data));
    }

    deleteInterview(id: number) {
        return this.delete(id, this.endPointControllerName + this.interviewEndPoints.deleteInterview)
            .pipe();
    }

    getInterviewById(id: number) {
        return this.get(id, this.endPointControllerName + this.interviewEndPoints.getInterviewById)
            .pipe(map((data: any) => data));
    }

    getInterviewByName(name: string) {
        return this.get(name, this.endPointControllerName + this.interviewEndPoints.getInterviewByName)
            .pipe(map((data: any) => data));
    }

    getInterviewAttendees() {
        return this.get(this.endPointControllerName + this.interviewEndPoints.getInterviewAttendees)
            .pipe(map((data: any) => data));
    }

    addComments(_command: any) {
        return this.post(_command, this.endPointControllerName + this.interviewEndPoints.addComments)
            .pipe(map((data: any) => data));
    }

    getCode() {
        return this.get(this.endPointControllerName + this.interviewEndPoints.getCode)
            .pipe(map((data: any) => data));
    }


}