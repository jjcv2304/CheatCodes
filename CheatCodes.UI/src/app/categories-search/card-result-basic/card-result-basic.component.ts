import {Component, EventEmitter, Input, Output} from '@angular/core';
import {CategoryBasic} from '../model/category';
import {CategoriesSearchHttpService} from '../categories-search-http.service';

@Component({
  selector: 'app-card-result-basic',
  templateUrl: './card-result-basic.component.html',
  styleUrls: ['./card-result-basic.component.scss']
})
export class CardResultBasicComponent {
  @Input() card: CategoryBasic;
  @Output() showCardDetailsClicked = new EventEmitter<void>();

  constructor(private categoriesSearchHttpService: CategoriesSearchHttpService) {
  }

  showCardDetails() {
    this.categoriesSearchHttpService.GetCategoryDetails(this.card.id);
    this.showCardDetailsClicked.emit();
  }
}
