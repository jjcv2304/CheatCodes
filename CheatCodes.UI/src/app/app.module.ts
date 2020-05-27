import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {HTTP_INTERCEPTORS, HttpClientModule} from '@angular/common/http';
import {FlipModule} from 'ngx-flip';

import {AppComponent} from './app.component';
import {CategoriesListComponent} from './categories-generation/categories-list/categories-list.component';
import {CategoriesService} from './categories-generation/categories/categories.service';
import {MainMenuComponent} from './main-menu/main-menu.component';
import {CategoryCardComponent} from './categories-generation/category-card/category-card.component';
import {CategoryEditComponent} from './categories-generation/category-edit/category-edit.component';
import {AppRoutingModule} from './app-routing.module';
import {FormsModule} from '@angular/forms';
import {FieldEditComponent} from './categories-generation/field-edit/field-edit.component';
import {CardMoveMenuComponent} from './categories-generation/category-card/card-move-menu/card-move-menu.component';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {MatSliderModule} from '@angular/material/slider';
import {CategoriesContainerComponent} from './categories-generation/categories-container/categories-container.component';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import {AuthService} from './security/auth-service.component';
import {SignoutRedirectCallbackComponent} from './security/signout-redirect-callback.component';
import {SigninRedirectCallbackComponent} from './security/signin-redirect-callback.component';
import {AuthInterceptorService} from './security/auth-interceptor.service';
import { UnauthorizedComponent } from './security/unauthorized/unauthorized.component';
import {StoreModule} from '@ngrx/store';
import {StoreDevtoolsModule} from '@ngrx/store-devtools';
import {environment} from '../environments/environment';
import {EffectsModule} from '@ngrx/effects';
import {CategoriesBreadCrumbsModule} from './categories-bread-crumbs/categories-bread-crumbs.module';


@NgModule({
    declarations: [
        AppComponent,
        CategoriesListComponent,
        MainMenuComponent,
        CategoryCardComponent,
        CategoryEditComponent,
        FieldEditComponent,
        CardMoveMenuComponent,
        CategoriesContainerComponent,
        SignoutRedirectCallbackComponent,
        SigninRedirectCallbackComponent,
        UnauthorizedComponent
    ],
    imports: [
        BrowserModule,
        HttpClientModule,
        FlipModule,
        AppRoutingModule,
        FormsModule,
        BrowserAnimationsModule,
        MatSliderModule,
        MatProgressSpinnerModule,
        StoreModule.forRoot({}),
        EffectsModule.forRoot([]),
        StoreDevtoolsModule.instrument({
            name: 'Dev tools ngrx for CheatCodes',
            maxAge: 25,
            logOnly: environment.production
        }),
      CategoriesBreadCrumbsModule
    ],
    providers: [
        CategoriesService,
        AuthService,
        {provide: HTTP_INTERCEPTORS, useClass: AuthInterceptorService, multi: true}
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}
