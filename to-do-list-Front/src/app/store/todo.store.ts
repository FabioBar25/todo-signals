import { inject } from '@angular/core';
import { signalStore, withHooks, withMethods, withState, patchState } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { catchError, concatMap, EMPTY, finalize, pipe, switchMap, tap } from 'rxjs';

import { TaskApiService } from '../api/task.api';
import { Task } from '../models/task';

type TodoState = {
  tasks: Task[];
  isLoading: boolean;
  isSaving: boolean;
  error: string | null;
};

const initialState: TodoState = {
  tasks: [],
  isLoading: false,
  isSaving: false,
  error: null
};

export const TodoStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withMethods((store, taskApi = inject(TaskApiService)) => {
    const loadTasks = rxMethod<void>(
      pipe(
        tap(() => patchState(store, { isLoading: true, error: null })),
        switchMap(() =>
          taskApi.getTasks().pipe(
            tap((tasks) => patchState(store, { tasks })),
            catchError(() => {
              patchState(store, { error: 'Unable to load tasks.' });
              return EMPTY;
            }),
            finalize(() => patchState(store, { isLoading: false }))
          )
        )
      )
    );

    const addTask = rxMethod<string>(
      pipe(
        tap(() => patchState(store, { isSaving: true, error: null })),
        concatMap((title) =>
          taskApi.createTask(title).pipe(
            tap((task) => patchState(store, { tasks: [...store.tasks(), task] })),
            catchError(() => {
              patchState(store, { error: 'Unable to save the new task.' });
              return EMPTY;
            }),
            finalize(() => patchState(store, { isSaving: false }))
          )
        )
      )
    );

    const updateTask = rxMethod<{ id: number; title: string }>(
      pipe(
        tap(() => patchState(store, { isSaving: true, error: null })),
        concatMap(({ id, title }) =>
          taskApi.updateTask(id, title).pipe(
            tap((updatedTask) =>
              patchState(store, {
                tasks: store.tasks().map((task) => (task.id === updatedTask.id ? updatedTask : task))
              })
            ),
            catchError(() => {
              patchState(store, { error: 'Unable to update the task.' });
              return EMPTY;
            }),
            finalize(() => patchState(store, { isSaving: false }))
          )
        )
      )
    );

    const deleteTask = rxMethod<number>(
      pipe(
        tap(() => patchState(store, { isSaving: true, error: null })),
        concatMap((id) =>
          taskApi.deleteTask(id).pipe(
            tap(() =>
              patchState(store, {
                tasks: store.tasks().filter((task) => task.id !== id)
              })
            ),
            catchError(() => {
              patchState(store, { error: 'Unable to delete the task.' });
              return EMPTY;
            }),
            finalize(() => patchState(store, { isSaving: false }))
          )
        )
      )
    );

    return {
      loadTasks,
      addTask,
      updateTask,
      deleteTask
    };
  }),
  withHooks({
    onInit(store) {
      store.loadTasks();
    }
  })
);
