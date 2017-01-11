import { ISortDescription } from './isortdescription';

export interface ISortCollection {
        sortDescriptions: ISortDescription[];
        skip: number;
	    take: number;
}