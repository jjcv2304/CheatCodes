import {Component, EventEmitter, Input, Output} from '@angular/core';
import {CardResultBasicComponent} from '../card-result-basic/card-result-basic.component';
import {CategoryBasic} from '../model/category';

@Component({
  selector: 'app-categories-search-list-result',
  templateUrl: './categories-search-list-result.component.html',
  styleUrls: ['./categories-search-list-result.component.scss']
})
export class CategoriesSearchListResultComponent {
  @Input() cards: CardResultBasicComponent[];
  @Output() showCardDetailsClicked = new EventEmitter<boolean>();

  constructor() {
  }

  showCardViewDetails($event: CategoryBasic) {

  }

  onShowCardDetailsClicked() {
    this.showCardDetailsClicked.emit();
  }
}
