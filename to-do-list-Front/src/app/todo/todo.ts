import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Task } from '../models/task';
import { AuthSessionService } from '../core/auth/auth-session.service';
import { TodoStore } from '../store/todo.store';

@Component({
  selector: 'app-todo',
  standalone: true,
  templateUrl: './todo.html',
  styleUrls: ['./todo.scss']
})
export class TodoComponent {
  readonly store = inject(TodoStore);
  readonly authSession = inject(AuthSessionService);
  private readonly router = inject(Router);
  readonly newTask = signal('');
  readonly editingTaskId = signal<number | null>(null);
  readonly editedTitle = signal('');

  constructor() {
    this.store.retryLoad();
  }

  addTask() {
    const title = this.newTask().trim();
    if (!title) {
      return;
    }

    this.store.addTask(title);
    this.newTask.set('');
  }

  deleteTask(id: number) {
    this.store.deleteTask(id);
  }

  retryLoad() {
    this.store.retryLoad();
  }

  startEdit(task: Task) {
    this.editingTaskId.set(task.id);
    this.editedTitle.set(task.title);
  }

  saveEdit(id: number) {
    const title = this.editedTitle().trim();
    if (!title) {
      return;
    }

    this.store.updateTask({ id, title });
    this.cancelEdit();
  }

  cancelEdit() {
    this.editingTaskId.set(null);
    this.editedTitle.set('');
  }

  async logout() {
    await this.authSession.logout();
    this.store.clearTasks();
    await this.router.navigateByUrl('/login');
  }
}
