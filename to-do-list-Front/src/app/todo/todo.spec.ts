import { signal } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TodoStore } from '../store/todo.store';
import { TodoComponent } from './todo';

describe('TodoComponent', () => {
  let fixture: ComponentFixture<TodoComponent>;

  const tasks = signal([{ id: 1, title: 'Wash dishes' }]);
  const isLoading = signal(false);
  const isSaving = signal(false);
  const hasLoadTimedOut = signal(false);
  const error = signal<string | null>(null);

  const storeMock = {
    tasks,
    isLoading,
    isSaving,
    hasLoadTimedOut,
    error,
    addTask: vi.fn(),
    updateTask: vi.fn(),
    deleteTask: vi.fn(),
    retryLoad: vi.fn()
  };

  beforeEach(async () => {
    tasks.set([{ id: 1, title: 'Wash dishes' }]);
    isLoading.set(false);
    isSaving.set(false);
    hasLoadTimedOut.set(false);
    error.set(null);
    vi.clearAllMocks();

    await TestBed.configureTestingModule({
      imports: [TodoComponent],
      providers: [{ provide: TodoStore, useValue: storeMock }]
    }).compileComponents();

    fixture = TestBed.createComponent(TodoComponent);
    fixture.detectChanges();
  });

  function textContent() {
    return fixture.nativeElement.textContent as string;
  }

  it('renders tasks from the signal store', () => {
    expect(textContent()).toContain('Wash dishes');
  });

  it('reacts to task signal changes by re-rendering the list', () => {
    tasks.set([
      { id: 1, title: 'Wash dishes' },
      { id: 2, title: 'Buy groceries' }
    ]);
    fixture.detectChanges();

    expect(textContent()).toContain('Buy groceries');

    tasks.set([{ id: 2, title: 'Buy groceries' }]);
    fixture.detectChanges();

    expect(textContent()).not.toContain('Wash dishes');
    expect(textContent()).toContain('Buy groceries');
  });

  it('shows and hides the loading message when the loading signal changes', () => {
    isLoading.set(true);
    fixture.detectChanges();

    expect(textContent()).toContain('Loading tasks...');

    isLoading.set(false);
    fixture.detectChanges();

    expect(textContent()).not.toContain('Loading tasks...');
  });

  it('calls deleteTask when the delete button is clicked', () => {
    const deleteButton = Array.from(fixture.nativeElement.querySelectorAll('button')).find(
      (button) => button.textContent?.includes('Delete')
    ) as HTMLButtonElement;

    deleteButton.click();

    expect(storeMock.deleteTask).toHaveBeenCalledWith(1);
  });

  it('shows the timeout panel and retries when the store times out', () => {
    hasLoadTimedOut.set(true);
    error.set('The backend is taking too long to respond.');
    fixture.detectChanges();

    const retryButton = Array.from(fixture.nativeElement.querySelectorAll('button')).find(
      (button) => button.textContent?.includes('Retry')
    ) as HTMLButtonElement;

    expect(textContent()).toContain('The backend did not respond in time.');

    retryButton.click();

    expect(storeMock.retryLoad).toHaveBeenCalled();
  });

  it('disables action buttons while the store is saving', () => {
    isSaving.set(true);
    fixture.detectChanges();

    const buttons = Array.from(fixture.nativeElement.querySelectorAll('button')) as HTMLButtonElement[];
    const editButton = buttons.find((button) => button.textContent?.includes('Edit'));
    const deleteButton = buttons.find((button) => button.textContent?.includes('Delete'));

    expect(editButton?.disabled).toBe(true);
    expect(deleteButton?.disabled).toBe(true);
  });

  it('shows and clears the error message when the error signal changes', () => {
    error.set('Unable to load tasks.');
    fixture.detectChanges();

    expect(textContent()).toContain('Unable to load tasks.');

    error.set(null);
    fixture.detectChanges();

    expect(textContent()).not.toContain('Unable to load tasks.');
  });
});
