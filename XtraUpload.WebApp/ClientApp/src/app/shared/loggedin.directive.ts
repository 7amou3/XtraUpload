import { Directive, Output, EventEmitter, OnInit } from '@angular/core';
import { UserStorageService } from '../services/user.storage.service';

@Directive({
  selector: '[appIsLoggedIn]'
})
export class IsLoggedInDirective implements OnInit {
  @Output() LoggedIn = new EventEmitter<ILoggedin>();
  constructor(private userStorage: UserStorageService) { }

  ngOnInit() {
    const token = this.userStorage.getToken();
    if (token) {
        this.LoggedIn.next({isLoggedIn: true, role: this.userStorage.getProfile().role});
    }
    else {
        this.LoggedIn.next({isLoggedIn: false});
    }
  }
}

export interface ILoggedin {
  isLoggedIn: boolean;
  role?: string;
}
