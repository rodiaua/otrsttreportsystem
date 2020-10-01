using AutoMapper;
using OtrsReportApp.Models;
using OtrsReportApp.Models.Account;
using OtrsReportApp.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OtrsReportApp
{
  public class MappingProfile: Profile
  {
    public MappingProfile()
    {
      CreateMap<Ticket, TicketDTO>();
      CreateMap<Queue, QueueDTO>();
      CreateMap<TicketState, TicketStateDTO>();
      CreateMap<RegistrationUserModel, AccountUser>()
        .ForMember("UserName", opt => opt.MapFrom(src => src.Email));
      CreateMap<AccountUser, UserTableModel>();
      
    }
  }
}
