import {Component, inject, input, OnInit, output, ViewChild} from '@angular/core';
import {Message} from '../../_models/message';
import {MessageService} from '../../_services/message.service';
import {TimeagoModule} from 'ngx-timeago';
import {FormsModule, NgForm} from '@angular/forms';

@Component({
  selector: 'app-member-messages',
  standalone: true,
  imports: [
    TimeagoModule, FormsModule
  ],
  templateUrl: './member-messages.component.html',
  styleUrl: './member-messages.component.css'
})
export class MemberMessagesComponent {
  @ViewChild('messageForm') messageForm?: NgForm;
  private messageService = inject(MessageService);
  username = input.required<string>();
  messages = input.required<Message[]>();
  updatedMessages = output<Message>();
  messageContent = '';

  sendMessage() {
    this.messageService.sendMessage(this.username(), this.messageContent).subscribe({
      next: message => {
        this.updatedMessages.emit(message);
        this.messageForm?.reset();
      }
    })
  }
}
