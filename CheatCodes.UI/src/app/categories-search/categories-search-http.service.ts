import {Injectable} from '@angular/core';
import {Envelope} from '../utils/envelope';
import {HttpClient, HttpParams} from '@angular/common/http';
import {CategoryBasic, CategoryTree, ICategoryBasic, ICategoryTree} from './model/category';
import {BehaviorSubject} from 'rxjs';
import {CategorySearchFilters} from './model/categorySearchFilters';

@Injectable()
export class CategoriesSearchHttpService {

  newCardSearchResult = new BehaviorSubject(false);
  newCardDetailsResult = new BehaviorSubject(false);
  private cardDetailResult: CategoryTree;

  get currentCategoryDetail(): CategoryTree {
    return this.cardDetailResult;
  }

  private cardsSearchResult: CategoryBasic[];

  // todo remove all cardsSearchResult
  get currentCategories(): CategoryBasic[] {
    return this.cardsSearchResult;
  }

  private readonly categorySearchUrl = 'https://localhost:57246/api/CategoriesSearch';

  constructor(private http: HttpClient) {
  }

  GetCategoriesByPartialNameAsync(categoryName: string) {
    let params = new HttpParams();
    params = params.append('textSearch', categoryName);
    return this.http.get<Envelope<Array<ICategoryBasic>>>(
      this.categorySearchUrl + '/GetCategoriesByPartialName',
      {params: params})
      .subscribe(
        (data: Envelope<Array<ICategoryBasic>>) => {
          this.cardsSearchResult = data.result;
          this.newCardSearchResult.next(true);
        },
        () => {
          console.log('Error GetCategoriesByPartialNameAsync');
        }
      );
  }

  private toHttpParams(params) {
    return Object.getOwnPropertyNames(params)
      .reduce((p, key) => p.set(key, params[key]), new HttpParams());
  }

  // GetCategoriesByFiltersAsync(filters: CategorySearchFilters) {
  //   const params = this.toHttpParams(filters);
  //
  //   return this.http.get<Envelope<Array<ICategoryBasic>>>(
  //     this.categorySearchUrl + '/GetCategoriesByFilters',
  //     {params: params})
  //     .subscribe(
  //       (data: Envelope<Array<ICategoryBasic>>) => {
  //         this.cardsSearchResult = data.result;
  //         this.newCardSearchResult.next(true);
  //       },
  //       () => {
  //         console.log('Error GetCategoriesByFiltersAsync');
  //       }
  //     );
  // }

  GetCategoriesByFiltersAsync(filters: CategorySearchFilters) {
    const params = this.toHttpParams(filters);

    return this.http.get<Envelope<Array<ICategoryBasic>>>(
      this.categorySearchUrl + '/GetCategoriesByFilters',
      {params: params});
  }

  GetCategoryDetails(categoryRootId: number) {
    return this.http.get<Envelope<ICategoryBasic>>(
      this.categorySearchUrl + '/GetCategoriesSubTreeByRootId/' + categoryRootId)
      .subscribe(
        (data: Envelope<ICategoryTree>) => {
          this.cardDetailResult = data.result;
          this.newCardDetailsResult.next(true);
        },
        () => {
          console.log('Error GetCategoriesByPartialNameAsync');
        }
      );
  }

}
