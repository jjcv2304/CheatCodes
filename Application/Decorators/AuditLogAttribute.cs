﻿using System;

namespace Application.Decorators
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
  public sealed class AuditLogAttribute : Attribute
  {
  }
}