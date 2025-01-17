import {Component, inject, OnInit, signal} from '@angular/core';
import {AdminService} from '../../_services/admin.service';
import {Photo} from '../../_models/photo';
import {ToastrService} from 'ngx-toastr';

@Component({
  selector: 'app-photo-management',
  standalone: true,
  imports: [],
  templateUrl: './photo-management.component.html',
  styleUrl: './photo-management.component.css'
})
export class PhotoManagementComponent implements OnInit {
  private adminService = inject(AdminService);
  private toastrService = inject(ToastrService);
  photos = signal<Photo[]>([]);

  ngOnInit(): void {
    this.getPhotosForApproval();
  }

  getPhotosForApproval() {
    this.adminService.getPhotosForApproval().subscribe({
      next: photos => this.photos.set(photos)
    })
  }

  approvePhoto(photoId: number) {
    this.adminService.approvePhoto(photoId).subscribe({
      next: _ => {
        this.toastrService.info("Successfully approve photo");
        this.photos.update(p => p.filter(p => p.id != photoId));
      },
      error: _ => this.toastrService.error("Error while approving photo"),
    });
  }

  rejectPhoto(photoId: number) {
    this.adminService.rejectPhoto(photoId).subscribe({
      next: _ => {
        this.toastrService.info("Successfully reject photo");
        this.photos.update(p => p.filter(p => p.id != photoId));
      },
      error: _ => this.toastrService.error("Error while rejecting photo"),
    });
  }
}
