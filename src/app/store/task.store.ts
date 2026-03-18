import { signalStore, withMethods, patchState, withHooks } from '@ngrx/signals';
import { withEntities, addEntity, removeEntity, setAllEntities } from '@ngrx/signals/entities';
import { Task } from '../models/task';
import { effect } from '@angular/core';

const STORAGE_KEY = 'tasks';

export const TaskStore = signalStore(
  { providedIn: 'root' },

  withEntities<Task>(),

  withMethods((store) => ({
    
    addTask(title: string) {
      const newTask: Task = {
        id: Date.now(),
        title
      };

      patchState(store, addEntity(newTask));
      localStorage.setItem('tasks', JSON.stringify(store.entities()));
    },

    deleteTask(id: number) {
      patchState(store, removeEntity(id));
    },

    loadTasks(tasks: Task[]) {
      patchState(store, setAllEntities(tasks));
    }

  })),

  withHooks({
    onInit(store) {
      // Load persisted tasks
      const stored = localStorage.getItem(STORAGE_KEY);

      if (stored) {
        const tasks: Task[] = JSON.parse(stored);
        patchState(store, setAllEntities(tasks));
      }

      // Persist whenever tasks change
      effect(() => {
        const tasks = store.entities();
        localStorage.setItem(STORAGE_KEY, JSON.stringify(tasks));
      });
    }
  })
);