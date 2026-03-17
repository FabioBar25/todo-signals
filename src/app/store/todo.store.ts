import { signalStore, withState, withMethods, patchState } from '@ngrx/signals';
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
      const newTask: Task = {
        id: Date.now(),
        title
      };

      const updatedTasks = [...store.tasks(), newTask];
      patchState(store, { tasks: updatedTasks });

      // persist
      localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(updatedTasks));
    },

    deleteTask(id: number) {
      const updatedTasks = store.tasks().filter(t => t.id !== id);
      patchState(store, { tasks: updatedTasks });

      // persist
      localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(updatedTasks));
    },

    loadTasks(tasks: Task[]) {
      patchState(store, { tasks });
      localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(tasks));
    }

  }))
);