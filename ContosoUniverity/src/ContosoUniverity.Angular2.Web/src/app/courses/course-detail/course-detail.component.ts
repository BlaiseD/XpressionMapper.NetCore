import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { Subscription } from 'rxjs/Subscription';

import { ICourse } from '../icourse';
import { CourseService } from '../course.service';

@Component({
  selector: 'app-course-detail',
  templateUrl: './course-detail.component.html',
  styleUrls: ['./course-detail.component.css']
})
export class CourseDetailComponent implements OnInit {
  public title: string = 'Courses';
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
      dept => this.course = dept,
      error => this.errorMessage = <any>error);
  }

  onBack(): void {
    this._router.navigate(['/courses']);
  }

}
