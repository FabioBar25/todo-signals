import { signalStore, withState, withMethods, patchState } from '@ngrx/signals';
import { Task } from '../models/task';

type TodoState = {
  tasks: Task[];
};

const initialState: TodoState = {
  tasks: []
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

      patchState(store, {
        tasks: [...store.tasks(), newTask]
      });
    },

    deleteTask(id: number) {

      patchState(store, {
        tasks: store.tasks().filter(t => t.id !== id)
      });

    },

    loadTasks(tasks: Task[]) {
      patchState(store, { tasks });
    }

  }))
);
