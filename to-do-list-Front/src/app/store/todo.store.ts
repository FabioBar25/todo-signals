import { inject } from '@angular/core';
import { signalStore, withHooks, withMethods, withState, patchState } from '@ngrx/signals';
import { rxMethod } from '@ngrx/signals/rxjs-interop';
import { catchError, concatMap, EMPTY, finalize, pipe, switchMap, tap, timeout, TimeoutError } from 'rxjs';

import { TaskApiService } from '../api/task.api';
import { Task } from '../models/task';

type TodoState = {
  tasks: Task[];
  isLoading: boolean;
  isSaving: boolean;
  hasLoadTimedOut: boolean;
  error: string | null;
};

const LOAD_TIMEOUT_MS = 10000;
const SAVE_TIMEOUT_MS = 10000;

const initialState: TodoState = {
  tasks: [],
  isLoading: false,
  isSaving: false,
  hasLoadTimedOut: false,
  error: null
};

export const TodoStore = signalStore(
  { providedIn: 'root' },
  withState(initialState),
  withMethods((store, taskApi = inject(TaskApiService)) => {
    const loadTasks = rxMethod<void>(
      pipe(
        tap(() => patchState(store, { isLoading: true, hasLoadTimedOut: false, error: null })),
        switchMap(() =>
          taskApi.getTasks().pipe(
            timeout(LOAD_TIMEOUT_MS),
            tap((tasks) => patchState(store, { tasks })),
            catchError((error) => {
              patchState(store, {
                error:
                  error instanceof TimeoutError
                    ? 'The backend is taking too long to respond.'
                    : 'Unable to load tasks.',
                hasLoadTimedOut: error instanceof TimeoutError
              });
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
            timeout(SAVE_TIMEOUT_MS),
            tap((task) => patchState(store, { tasks: [...store.tasks(), task] })),
            catchError((error) => {
              patchState(store, {
                error:
                  error instanceof TimeoutError
                    ? 'Saving the task timed out. Check that the backend is running.'
                    : 'Unable to save the new task.'
              });
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
            timeout(SAVE_TIMEOUT_MS),
            tap((updatedTask) =>
              patchState(store, {
                tasks: store.tasks().map((task) => (task.id === updatedTask.id ? updatedTask : task))
              })
            ),
            catchError((error) => {
              patchState(store, {
                error:
                  error instanceof TimeoutError
                    ? 'Updating the task timed out. Check that the backend is running.'
                    : 'Unable to update the task.'
              });
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
            timeout(SAVE_TIMEOUT_MS),
            tap(() =>
              patchState(store, {
                tasks: store.tasks().filter((task) => task.id !== id)
              })
            ),
            catchError((error) => {
              patchState(store, {
                error:
                  error instanceof TimeoutError
                    ? 'Deleting the task timed out. Check that the backend is running.'
                    : 'Unable to delete the task.'
              });
              return EMPTY;
            }),
            finalize(() => patchState(store, { isSaving: false }))
          )
        )
      )
    );

    return {
      loadTasks,
      retryLoad: () => loadTasks(),
      addTask,
      updateTask,
      deleteTask,
      clearTasks: () =>
        patchState(store, {
          tasks: [],
          isLoading: false,
          isSaving: false,
          hasLoadTimedOut: false,
          error: null
        })
    };
  }),
  withHooks({
    onInit(store) {
      store.loadTasks();
    }
  })
);
