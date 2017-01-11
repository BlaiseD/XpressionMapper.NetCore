import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { Subscription } from 'rxjs/Subscription';

import { ICourse } from '../icourse';
import { CourseService } from '../course.service';

@Component({
  selector: 'app-course-delete',
  templateUrl: './course-delete.component.html',
  styleUrls: ['./course-delete.component.css']
})
export class CourseDeleteComponent implements OnInit {
  public pageTitle: string = 'Courses';
  public course: ICourse;
  errorMessage: string;
  private sub: Subscription;

  constructor(private _route: ActivatedRoute,
    private _router: Router,
    private _courseService: CourseService) { }

  ngOnInit() {
    this.sub = this._route.params.subscribe(
      params => {
        let id = +params['id'];
        this.getCourse(id);
      });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  getCourse(id: number) {
    this._courseService.getCourse(id).subscribe(
      course => this.course = course,
      error => this.errorMessage = <any>error);
  }

  onBack(): void {
    this._router.navigate(['/courses']);
  }

  submitClick(): void {
    this._courseService.deleteCourse(this.course.courseID)
      .subscribe(course => {
        this._router.navigate(['/courses']);
      },
      error => this.errorMessage = <any>error);
  }

}
