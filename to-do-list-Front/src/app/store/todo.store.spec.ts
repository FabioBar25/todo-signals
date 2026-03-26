import { TestBed } from '@angular/core/testing';
import { Observable, of, Subject, throwError } from 'rxjs';

import { TaskApiService } from '../api/task.api';
import { Task } from '../models/task';
import { TodoStore } from './todo.store';

describe('TodoStore', () => {
  afterEach(() => {
    TestBed.resetTestingModule();
  });

  function setup(taskApiOverrides: Partial<TaskApiService> = {}) {
    
    const taskApi = {
      getTasks: vi.fn(() => of([])),
      createTask: vi.fn((title: string) => of({ id: 1, title })),
      updateTask: vi.fn((id: number, title: string) => of({ id, title })),
      deleteTask: vi.fn(() => of(void 0)),
      ...taskApiOverrides
    };

    TestBed.configureTestingModule({
      providers: [{ provide: TaskApiService, useValue: taskApi }]
    });

    return {
      store: TestBed.inject(TodoStore),
      taskApi
    };
  }

  it('transitions loading from true to false after a successful load', () => {
    const tasks$ = new Subject<Task[]>();
    const { store } = setup({
      getTasks: vi.fn(() => tasks$.asObservable())
    });

    expect(store.isLoading()).toBe(true);
    expect(store.error()).toBeNull();
    expect(store.hasLoadTimedOut()).toBe(false);

    tasks$.next([{ id: 1, title: 'Loaded from API' }]);
    expect(store.tasks()).toEqual([{ id: 1, title: 'Loaded from API' }]);
    expect(store.isLoading()).toBe(true);

    tasks$.complete();

    expect(store.isLoading()).toBe(false);
    expect(store.error()).toBeNull();
    expect(store.tasks()).toEqual([{ id: 1, title: 'Loaded from API' }]);
  });

  it('transitions saving from true to false and appends a created task', () => {
    const createTask$ = new Subject<Task>();
    const { store, taskApi } = setup({
      createTask: vi.fn(() => createTask$.asObservable())
    });

    store.addTask('New task');

    expect(store.isSaving()).toBe(true);
    expect(taskApi.createTask).toHaveBeenCalledWith('New task');

    createTask$.next({ id: 5, title: 'New task' });
    expect(store.tasks()).toEqual([{ id: 5, title: 'New task' }]);
    expect(store.isSaving()).toBe(true);

    createTask$.complete();

    expect(store.isSaving()).toBe(false);
    expect(store.tasks()).toEqual([{ id: 5, title: 'New task' }]);
  });

  it('surfaces a load error and clears the loading flag after a failed load', () => {
    const { store } = setup({
      getTasks: vi.fn(() => throwError(() => new Error('boom')))
    });

    expect(store.isLoading()).toBe(false);
    expect(store.error()).toBe('Unable to load tasks.');
    expect(store.hasLoadTimedOut()).toBe(false);
  });

  it('retries loading and replaces the error once the follow-up request succeeds', () => {
    const responses: Observable<Task[]>[] = [
      throwError(() => new Error('boom')),
      of([{ id: 7, title: 'Recovered task' }])
    ];

    const { store, taskApi } = setup({
      getTasks: vi.fn(() => responses.shift() ?? of([]))
    });

    expect(store.error()).toBe('Unable to load tasks.');

    store.retryLoad();

    expect(taskApi.getTasks).toHaveBeenCalledTimes(2);
    expect(store.error()).toBeNull();
    expect(store.isLoading()).toBe(false);
    expect(store.tasks()).toEqual([{ id: 7, title: 'Recovered task' }]);
  });
});
