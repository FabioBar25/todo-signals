import { Component, inject, signal } from '@angular/core';
import { TodoStore } from '../store/todo.store';

@Component({
  selector: 'app-todo',
  standalone: true,
  templateUrl: './todo.html',
  styleUrl: './todo.scss'
})
export class TodoComponent {

  store = inject(TodoStore);

  newTask = signal('');

  addTask() {
    const title = this.newTask();

    if (!title.trim()) return;

    this.store.addTask(title);
    this.newTask.set('');
  }

  deleteTask(id: number) {
    this.store.deleteTask(id);
  }

}
