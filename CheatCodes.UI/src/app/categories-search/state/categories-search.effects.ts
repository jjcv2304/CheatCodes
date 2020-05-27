import {Injectable} from '@angular/core';
import {CategoriesSearchHttpService} from '../categories-search-http.service';
import {Actions, Effect, ofType} from '@ngrx/effects';
import * as searchActions from './categories-search.actions';
import {catchError, map, switchMap} from 'rxjs/operators';
import {ICategoryBasic} from '../model/category';
import {Envelope} from '../../utils/envelope';
import {Observable, of} from 'rxjs';
import {Action} from '@ngrx/store';
import {CategoriesFilter} from './categories-search.actions';


@Injectable()
export class CategoriesSearchEffects {
  constructor(private actions$: Actions, private categoriesSearchHttpService: CategoriesSearchHttpService) {
  }

  // GetCategoriesByFiltersAsync returns an envelope....
  @Effect()
  searchCategories$: Observable<Action> = this.actions$.pipe(
    ofType(searchActions.CategoriesSearchActionTypes.CategoriesFilter),
    switchMap((action: CategoriesFilter) =>
      this.categoriesSearchHttpService.GetCategoriesByFiltersAsync(action.payload).pipe(
        map((data: Envelope<Array<ICategoryBasic>>) => (new searchActions.CategoriesFilterSuccess(data.result))
        ),
        catchError(err => of(new searchActions.CategoriesFilterFail(err)))
      ))
  );
}
