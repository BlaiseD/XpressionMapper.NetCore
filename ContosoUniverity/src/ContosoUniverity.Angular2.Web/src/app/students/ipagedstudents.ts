import { IStudent } from './istudent';

export interface IPagedStudents {
        students: IStudent[];
        studentCount: number;
}