import {inject, Injectable, signal} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import { User } from '../_models/user';
import {map} from 'rxjs';
import {environment} from '../../environments/environment';
import {LikesService} from './likes.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private http = inject(HttpClient);
  private likeService = inject(LikesService);
  baseUrl = environment.apiUrl;
  currentUser = signal<User | null>(null);

  login(model: any) {
    return this.http.post<User>(`${this.baseUrl}/account/login`, model).pipe(
      map(user => {
        if(user) {
          this.setCurrentUser(user)
        }
        return user;
      })
    )
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }

  register(model: any) {
    return this.http.post<User>(`${this.baseUrl}/account/register`, model).pipe(
      map(user => {
        if(user) {
          this.setCurrentUser(user)
        }
        return user;
      })
    )
  }

  setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
    this.likeService.getLikeIds();
  }
}
