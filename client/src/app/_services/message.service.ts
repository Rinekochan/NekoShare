import {inject, Injectable, signal} from '@angular/core';
import {environment} from '../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {PaginatedResult} from '../_models/pagination';
import {Message} from '../_models/message';
import {setPaginatedResponse, setPaginationHeaders} from './paginationHelper';
import {HubConnection, HubConnectionBuilder} from '@microsoft/signalr';
import {User} from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubsUrl;
  private http = inject(HttpClient);
  private hubConnection?: HubConnection;

  paginatedResult = signal<PaginatedResult<Message[]> | null>(null);
  messageThread = signal<Message[]>([]);

  createHubConnection(user: User, otherUsername: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${this.hubUrl}/messages?user=${otherUsername}`, {
        accessTokenFactory: () => user.token
      })
      .withAutomaticReconnect()
      .build()

    this.hubConnection.start().catch(error => console.log(error));

    this.hubConnection.on('ReceiveMessageThread', messages => {
      this.messageThread.set(messages)
    })
  }

  getMessages(pageNumber: number, pageSize: number, container: string) {
    let params = setPaginationHeaders(pageNumber, pageSize);

    params = params.append("Container", container);

    return this.http.get<Message[]>(`${this.baseUrl}/message`, {observe: 'response', params})
      .subscribe({
        next: response => setPaginatedResponse(response, this.paginatedResult)
      })
  }

  getMessageThread(partner: string) {
    return this.http.get<Message[]>(`${this.baseUrl}/message/thread/${partner}`);
  }

  sendMessage(username: string, content: string) {
    return this.http.post<Message>(`${this.baseUrl}/message`, {recipientUsername: username, content: content})
  }

  deleteMessage(id: number) {
    return this.http.delete(`${this.baseUrl}/message/${id}`)
  }
}
