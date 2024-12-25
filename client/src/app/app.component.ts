import {Component, inject, OnInit} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {HttpClient} from '@angular/common/http';
import {NgFor} from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NgFor],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  http = inject(HttpClient);
  title = 'Neko Share';
  users: any;

  ngOnInit(): void {
    this.http.get('https://localhost:5001/api/users').subscribe({
      next: data => this.users = data,
      error: err => console.log(err),
      complete: () => console.log("Request has completed")

    })
  }
}
