import {AfterViewInit, Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {BreakpointObserver, Breakpoints} from '@angular/cdk/layout';
import {Observable, of, Subscription} from 'rxjs';
import {map, shareReplay, takeWhile} from 'rxjs/operators';
import {CategoryBasic, CategoryTree} from '../model/category';
import {CategoriesSearchHttpService} from '../categories-search-http.service';
import {select, Store} from '@ngrx/store';
import * as fromCategorySearch from '../state';
import {MatSidenav} from '@angular/material/sidenav';
import * as searchActions from '../state/categories-search.actions';

@Component({
  selector: 'app-categories-search-container',
  templateUrl: './categories-search-container.component.html',
  styleUrls: ['./categories-search-container.component.scss']
})
export class CategoriesSearchContainerComponent implements OnInit, OnDestroy {
  @ViewChild('drawer') drawer: MatSidenav;
  showDetailView = false;
  showButtonShowSideNav: boolean;
  componentActive = true;
  newCardDetailsResultSubscription: Subscription;

  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      map(result => result.matches),
      shareReplay()
    );
  cardsSearchResult: CategoryBasic[];
  cardDetails: CategoryTree;
  currentParentId: number;

  constructor(private breakpointObserver: BreakpointObserver,
              private categoriesSearchService: CategoriesSearchHttpService,
              private store: Store<fromCategorySearch.State>) {
    this.currentParentId = 0;
  }

  ngOnInit(): void {

    // contains the results from a search
    this.store.pipe(select(fromCategorySearch.getFilteredCategories),
      takeWhile(() => this.componentActive))
      .subscribe((result: CategoryBasic[]) => this.cardsSearchResult = result);

    // contains the results of clicking into a card.details-> shows treeView
    this.newCardDetailsResultSubscription = this.categoriesSearchService.newCardDetailsResult.subscribe({
      next: (v) => {
        if (v === true) {
          console.log('------------------->newCardDetailsResultSubscription  TRUE');
          this.cardDetails = this.categoriesSearchService.currentCategoryDetail;
          this.SetSideNavShow(false);
        } else {
          console.log('------------------->newCardDetailsResultSubscription  FALSE');
          this.cardDetails = null;
        }
      }
    });

    this.store.pipe(select(fromCategorySearch.getShowButtonShowSideNav), takeWhile(() => this.componentActive)).subscribe(
      showButtonShowSideNav => {
        this.showButtonShowSideNav = showButtonShowSideNav;
        console.log('------------------->getShowButtonShowSideNav');
        if (this.showButtonShowSideNav) {
          this.showDetailView = true;
        } else {
          this.showDetailView = false;
        }
      });
  }

  SetSideNavShow(setSideNavShow: boolean) {
    if (setSideNavShow === true) {
      this.drawer?.open();
      this.store.dispatch(new searchActions.ShowButtonShowSideNav(false));
    } else {
      this.drawer?.close();
      this.store.dispatch(new searchActions.ShowButtonShowSideNav(true));
    }
  }

  ngOnDestroy(): void {
    this.componentActive = false;
    if (this.newCardDetailsResultSubscription) {
      this.newCardDetailsResultSubscription.unsubscribe();
    }
  }

}
