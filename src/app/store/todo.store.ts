import { signalStore, withState, withMethods, patchState, withHooks } from '@ngrx/signals';
import { effect } from '@angular/core'; // Import effect
import { Task } from '../models/task';

type TodoState = {
  tasks: Task[];
};

const LOCAL_STORAGE_KEY = 'todo-tasks';

const initialState: TodoState = {
  tasks: JSON.parse(localStorage.getItem(LOCAL_STORAGE_KEY) || '[]')
};

export const TodoStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withMethods((store) => ({
    addTask(title: string) {
      const newTask: Task = { id: Date.now(), title };
      patchState(store, { tasks: [...store.tasks(), newTask] });
    },
    deleteTask(id: number) {
      patchState(store, { tasks: store.tasks().filter(t => t.id !== id) });
    },
    updateTask(id: number, newTitle: string) {
      patchState(store, {
        tasks: store.tasks().map(task =>
          task.id === id ? { ...task, title: newTitle } : task
        )
      });
    }
  })),
  // --- ADD THIS SECTION ---
  withHooks({
    onInit(store) {
      effect(() => {
        const tasks = store.tasks();
        localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(tasks));
      });
    },
  })
);