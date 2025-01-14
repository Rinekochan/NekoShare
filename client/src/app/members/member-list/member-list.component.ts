import {Component, inject, OnInit} from '@angular/core';
import {MembersService} from '../../_services/members.service';
import {MemberCardComponent} from '../member-card/member-card.component';
import {PaginationModule} from 'ngx-bootstrap/pagination';
import {FormsModule} from '@angular/forms';
import {ButtonsModule} from 'ngx-bootstrap/buttons';

@Component({
  imports: [
    MemberCardComponent,
    PaginationModule,
    FormsModule,
    ButtonsModule
  ],
  selector: 'app-member-list',
  standalone: true,
  styleUrl: './member-list.component.css',
  templateUrl: './member-list.component.html'
})
export class MemberListComponent implements OnInit {
  memberService = inject(MembersService);
  genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Females'}];

  ngOnInit() {
    if (!this.memberService.paginatedResult()) this.loadMembers();
  }

  loadMembers() {
    this.memberService.getMembers();
  }

  resetFilters() {
    this.memberService.resetUserParams();
    this.loadMembers();
  }

  pageChanged(event: any) {
    if (this.memberService.userParams().pageNumber !== event.page) {
      this.memberService.userParams().pageNumber = event.page;
      this.loadMembers();
    }
  }
}
