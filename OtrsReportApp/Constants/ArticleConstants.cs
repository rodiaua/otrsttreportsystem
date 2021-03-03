using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp.Constants
{
  public class ArticleConstants
  {
    public enum ArticalSenderType
    {
      Agent = 1,
      System =2,
      Customer = 3
    }

    public enum ArticleType
    {
      ExternalEmail =1,
      InternalEmail = 2
    }
  }
}
