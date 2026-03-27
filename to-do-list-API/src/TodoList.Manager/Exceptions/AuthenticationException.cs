namespace TodoList.Manager.Exceptions;

public sealed class AuthenticationException(string message) : Exception(message);
