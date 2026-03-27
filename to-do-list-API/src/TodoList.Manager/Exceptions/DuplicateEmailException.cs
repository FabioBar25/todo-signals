namespace TodoList.Manager.Exceptions;

public sealed class DuplicateEmailException(string message) : Exception(message);
