using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Constants
{
  public enum State
  {
    Open = 4,
    Closed = 2
  }

  public enum TicketQueue
  {
    Trash = 31,
    TestingVas = 38,
    TestingOperators = 39,
    TestingBut = 32,
    TestingSurvay = 42,
    TestingDaily = 52
  }
}
