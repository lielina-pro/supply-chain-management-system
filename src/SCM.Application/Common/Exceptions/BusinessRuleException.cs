namespace SCM.Application.Common.Exceptions;
public class NotFoundException      : Exception { public NotFoundException(string msg)      : base(msg) { } }
public class BusinessRuleException  : Exception { public BusinessRuleException(string msg)  : base(msg) { } }
public class ValidationException    : Exception { public ValidationException(string msg)    : base(msg) { } }
