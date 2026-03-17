import { Component, inject, signal } from '@angular/core';
import { TodoStore } from '../store/todo.store';

@Component({
  selector: 'app-todo',
  standalone: true,
  templateUrl: './todo.html',
  styleUrls: ['./todo.scss']
})
export class TodoComponent {

  store = inject(TodoStore);

  newTask = signal('');

  editingTaskId = signal<number | null>(null);
  editedTitle = signal('');

  addTask() {
    const title = this.newTask().trim();
    if (!title) return;

    this.store.addTask(title);
    this.newTask.set('');
  }

  deleteTask(id: number) {
    this.store.deleteTask(id);
  }

  startEdit(task: any) {
    this.editingTaskId.set(task.id);
    this.editedTitle.set(task.title);
  }

  saveEdit(id: number) {
    const title = this.editedTitle().trim();
    if (!title) return;

    this.store.updateTask(id, title);
    this.cancelEdit();
  }

  cancelEdit() {
    this.editingTaskId.set(null);
    this.editedTitle.set('');
  }

}
