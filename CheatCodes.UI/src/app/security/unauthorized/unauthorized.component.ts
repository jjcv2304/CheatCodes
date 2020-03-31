import { Component, OnInit } from '@angular/core';
import {AuthService} from '../auth-service.component';

@Component({
  selector: 'app-unauthorized',
  templateUrl: './unauthorized.component.html',
  styles: [
  ],
})
export class UnauthorizedComponent implements OnInit {
  constructor(private _authService: AuthService) { }

  ngOnInit() { }

  logout() {
    this._authService.logout();
  }
}
