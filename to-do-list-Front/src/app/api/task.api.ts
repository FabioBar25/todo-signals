import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Task } from '../models/task';

type SaveTaskRequest = {
  title: string;
};

@Injectable({ providedIn: 'root' })
export class TaskApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/tasks';

  getTasks(): Observable<Task[]> {
    return this.http.get<Task[]>(this.baseUrl);
  }

  createTask(title: string): Observable<Task> {
    const payload: SaveTaskRequest = { title };
    return this.http.post<Task>(this.baseUrl, payload);
  }

  updateTask(id: number, title: string): Observable<Task> {
    const payload: SaveTaskRequest = { title };
    return this.http.put<Task>(`${this.baseUrl}/${id}`, payload);
  }

  deleteTask(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
