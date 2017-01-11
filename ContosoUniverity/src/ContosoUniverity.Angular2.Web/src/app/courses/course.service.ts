import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/map';

import { ICourse } from './icourse';

@Injectable()
export class CourseService {
    private _coursesUrl = 'http://localhost:60360/api/Courses';

    constructor(private _http: Http) { }

    getCourses(): Observable<ICourse[]> {
        return this._http.get(this._coursesUrl)
            .map((response: Response) => <ICourse[]>response.json())
            .do(data => console.log('All: ' + JSON.stringify(data)))
            .catch(this.handleError);
    }

    getCourse(id: number): Observable<ICourse> {
        const url = `${this._coursesUrl}/${id}`;
        return this._http.get(url)
            .map((response: Response) => <ICourse>response.json())
            .do(data => console.log('All: ' + JSON.stringify(data)))
            .catch(this.handleError);
    }

    deleteCourse(id: number): Observable<ICourse> {
        const url = `${this._coursesUrl}/${id}`;
        return this._http.delete(url)
            .map((response: Response) => response.json())
            .do(data => console.log('All: ' + JSON.stringify(data)))
            .catch(this.handleError);
    }

    updateCourse(dept: ICourse): Observable<ICourse> {
        const url = `${this._coursesUrl}/${dept.courseID}`;
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        return this._http.put(url, JSON.stringify(dept), options)
            .map((response: Response) => response.json())
            .do(data => console.log('All: ' + JSON.stringify(data)))
            .catch(this.handleError);
    }

    insertCourse(dept: ICourse): Observable<ICourse> {
        const url = this._coursesUrl;
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        return this._http.post(url, JSON.stringify(dept), options)
            .map((response: Response) => response.json())
            .do(data => console.log('All: ' + JSON.stringify(data)))
            .catch(this.handleError);
    }

    private handleError(error: Response) {
        console.error(error);
        return Observable.throw(error.json().error || 'Server error');
    }

}