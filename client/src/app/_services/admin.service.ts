import {inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {environment} from '../../environments/environment';
import { User } from '../_models/user';
import {Photo} from '../_models/photo';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getUsersWithRoles() {
    return this.http.get<User[]>(`${this.baseUrl}/admin/users-with-roles`);
  }

  updateUserRoles(username: string, roles: string[]) {
    return this.http.post<string[]>(`${this.baseUrl}/admin/edit-roles/${username}?roles=${roles}`, {});
  }

  getPhotosForApproval() {
    return this.http.get<Photo[]>(`${this.baseUrl}/admin/photos-for-approval`);
  }

  approvePhoto(photoId: number) {
    return this.http.put(`${this.baseUrl}/admin/approve/photo?id=${photoId}`, {});
  }

  rejectPhoto(photoId: number) {
    return this.http.put(`${this.baseUrl}/admin/reject/photo?id=${photoId}`, {});
  }
}
