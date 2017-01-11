import { Component, OnInit } from '@angular/core';

import { ICourse } from '../icourse'
import { CourseService } from '../course.service';

@Component({
  selector: 'app-course-list',
  templateUrl: './course-list.component.html',
  styleUrls: ['./course-list.component.css']
})
export class CourseListComponent implements OnInit {

  constructor(private _courseService: CourseService) { }

  public title: string = 'Courses';

  errorMessage: string;
  courses: ICourse[];

  ngOnInit() {
    this._courseService.getCourses()
            .subscribe(courses => this.courses = courses,
                           error => this.errorMessage = <any>error);
  }

}
