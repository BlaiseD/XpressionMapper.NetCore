import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CustomFormsModule } from 'ng2-validation'

import { Subscription } from 'rxjs/Subscription';

import { ICourse } from '../icourse';
import { IDepartment } from '../../departments/idepartment';
import { CourseService } from '../course.service';
import { DepartmentService } from '../../departments/department.service';

@Component({
  selector: 'app-course-edit',
  templateUrl: './course-edit.component.html',
  styleUrls: ['./course-edit.component.css']
})
export class CourseEditComponent implements OnInit {
  public pageTitle: string = 'Course';
  public course: ICourse;

  public departments: IDepartment[];
  errorMessage: string;
  private sub: Subscription;

  constructor(private _route: ActivatedRoute,
    private _router: Router,
    private _courseService: CourseService,
    private _departmentService: DepartmentService) { }

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

    this._departmentService.getDepartments()
      .subscribe(departments => this.departments = departments,
      error => this.errorMessage = <any>error);
  }

  onBack(): void {
    this._router.navigate(['/courses']);
  }

  submitClick(): void {
    this._courseService.updateCourse(this.course)
      .subscribe(dept => {
        this._router.navigate(['/courses']);
      },
      error => this.errorMessage = <any>error);

  }
}
