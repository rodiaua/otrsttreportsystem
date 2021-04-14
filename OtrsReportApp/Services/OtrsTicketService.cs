using System;
using System.Collections.Generic;
using System.Text;
using OtrsReportApp.Models;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using OtrsReportApp.Models.DTO;
using OtrsReportApp.Extensions;
using ClosedXML.Excel;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper.Internal;
using OtrsReportApp.Data;
using System.Runtime.CompilerServices;
using OtrsReportApp.Models.OtrsTicket;
using OtrsReportApp.Constants;
using static OtrsReportApp.Constants.ArticleConstants;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using LinqKit;
using OtrsReportApp.Models.Logging;
using Microsoft.AspNetCore.Http.Connections;

namespace OtrsReportApp.Services
{
  public class OtrsTicketService
  {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public OtrsTicketService(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
      _scopeFactory = scopeFactory;
      _mapper = mapper;
    }


    private Dictionary<long, Dictionary<string, string>> GetTTDynamicFieldsBulk(IEnumerable<long> ids)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var dynamicFieldsValues = db.DynamicFieldValue.Where(fields => ids.Contains(fields.ObjectId)).Include(field => field.Field).Where(field => field.Field.Name.Contains("Key") ||
        field.Field.Name.Equals("TicketFreeTime1") || field.Field.Name.Contains("TicketSide")).ToList();

        Dictionary<long, Dictionary<string, string>> ttsDynamicFields = new Dictionary<long, Dictionary<string, string>>();
        foreach (var id in ids)
        {
          var ttDynamicFields = dynamicFieldsValues.FindAll(field => field.ObjectId == id);
          Dictionary<string, string> configs = new Dictionary<string, string>();
          foreach (var dfv in ttDynamicFields)
          {
            var config = Encoding.UTF8.GetString(dfv.Field.Config, 0, dfv.Field.Config.Length).Split("\n").ToList().FindAll(value => value.Contains("Key")).ToList();
            foreach (var item in config)
            {
              var res = item.Split(":");
              if (res.Length > 1 && res[0].Contains("Key"))
                configs.TryAdd(res[0].Trim(), res[1].TrimStart());
            }
          }
          Dictionary<string, string> result = new Dictionary<string, string>();
          if (configs.Count > 0)
          {
            foreach (var dfValue in ttDynamicFields)
            {
              if (dfValue.ValueText != null)
              {
                if (dfValue.Field.Name.Equals("TicketSide")) result.TryAdd("ProblemSide", dfValue.ValueText);
                else if (configs.ContainsKey(dfValue.ValueText))
                  result.TryAdd(dfValue.Field.Name.Substring(0, dfValue.Field.Name.Length - 3), configs.First(p => p.Key.Equals(dfValue.ValueText)).Value);
                else result.TryAdd(dfValue.Field.Name.Substring(0, dfValue.Field.Name.Length - 3), dfValue.ValueText);
              }
              else if (dfValue.ValueDate != null)
              {
                result.TryAdd("CloseTime", dfValue.ValueDate.ToString());
              }
            }
          }
          ttsDynamicFields.Add(id, result);
        }
        return ttsDynamicFields;
      }
    }


    public Report GetFilteredTicketsReportBulk(Filters filters)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        List<TicketReportDTO> ticketReportDTOs = new List<TicketReportDTO>();
        var tickets = (from ticket in db.Ticket
                       where ticket.CreateTime.CompareTo(filters.Period.startTime) > 0
                       && ticket.CreateTime.CompareTo(filters.Period.endTime) < 0
                       select ticket)
                      .Include(ticket => ticket.TicketState)
                      .Include(ticket => ticket.Queue)
                      .Include(ticket => ticket.TicketPriority)
                      .ToList().Where(t => !t.Queue.Name.Equals("Trash"));

        var ttsDynamicFields = GetTTDynamicFieldsBulk((from ticket in tickets select ticket.Id).ToList());

        FilteringItems filteringItems = new FilteringItems();

        var zones = ttsDynamicFields.Select(p => p.Value).ToList().SelectValues("Zone").Distinct().ToList();
        var types = ttsDynamicFields.Select(p => p.Value).ToList().SelectValues("Type").Distinct().ToList();
        var direction = ttsDynamicFields.Select(p => p.Value).ToList().SelectValues("Direction").Distinct().ToList();
        var natInt = ttsDynamicFields.Select(p => p.Value).ToList().SelectValues("NatInt").Distinct().ToList();
        var initiator = ttsDynamicFields.Select(p => p.Value).ToList().SelectValues("Initiator").Distinct().ToList();
        var category = ttsDynamicFields.Select(p => p.Value).ToList().SelectValues("Category").Distinct().ToList();
        var problemSide = ttsDynamicFields.Select(p => p.Value).ToList().SelectValues("ProblemSide").Distinct().ToList();



        filteringItems.States = tickets.Select(ticket => ticket.TicketState.Name).Distinct().OrderByDescending(state => state).toSelectItems();
        filteringItems.TicketPriorities = tickets.Select(ticket => ticket.TicketPriority.Name).Distinct().OrderByDescending(ticketPriority => ticketPriority).toSelectItems();
        filteringItems.Zones = zones.Count > 0 ? zones.toSelectItems() : null;
        filteringItems.Types = types.Count > 0 ? types.toSelectItems() : null;
        filteringItems.Directions = direction.Count > 0 ? direction.toSelectItems() : null;
        filteringItems.NatInts = natInt.Count > 0 ? natInt.toSelectItems() : null;
        filteringItems.Initiators = initiator.Count > 0 ? initiator.toSelectItems() : null;
        filteringItems.Categories = category.Count > 0 ? category.toSelectItems() : null;
        filteringItems.ProblemSides = problemSide.Count > 0 ? problemSide.toSelectItems() : null;

        foreach (var ticket in tickets)
        {
          TicketReportDTO ticketReportDTO = new TicketReportDTO();
          ticketReportDTO.TN = ticket.Tn;
          ticketReportDTO.State = ticket.TicketState.Name;
          ticketReportDTO.CreateTime = ticket.CreateTime;
          ticketReportDTO.Client = ticket.CustomerId;
          ticketReportDTO.TicketPriority = ticket.TicketPriority.Name;
          var dFields = ttsDynamicFields[ticket.Id];
          if (dFields.Count > 0)
          {
            foreach (var field in dFields)
            {
              typeof(TicketReportDTO).GetProperty(field.Key)?.SetValue(ticketReportDTO, !field.Key.Equals("CloseTime") ? field.Value : DateTime.Parse(field.Value));
            }
          }
          if (IsTicketDTOValid(ticketReportDTO, filters)) ticketReportDTOs.Add(ticketReportDTO);
        }

        return new Report()
        {
          ticketReportDTOs = ticketReportDTOs.OrderByDescending(ticket => ticket.CreateTime),
          filteringItems = filteringItems
        };

      }
    }

    private Dictionary<string, string> GetTTDynamicFields(long ttId)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var dynamicFieldsValues = (from fields in db.DynamicFieldValue
                                   where EF.Functions.Like(fields.ObjectId.ToString(), ttId.ToString())
                                   select fields).Include(field => field.Field).Where(field => field.Field.Name.Contains("Key")).ToList();
        Dictionary<string, string> configs = new Dictionary<string, string>();
        foreach (var dfv in dynamicFieldsValues)
        {
          var config = Encoding.UTF8.GetString(dfv.Field.Config, 0, dfv.Field.Config.Length).Split("\n");
          foreach (var item in config)
          {
            var res = item.Split(":");
            if (res.Length > 1 && res[0].Contains("Key"))
              configs.TryAdd(res[0].Trim(), res[1].TrimStart());
          }
        }
        Dictionary<string, string> result = new Dictionary<string, string>();
        if (configs.Count > 0)
        {
          foreach (var dfValue in dynamicFieldsValues)
          {
            if (dfValue.ValueText != null)
            {
              if (configs.ContainsKey(dfValue.ValueText))
                result.Add(dfValue.Field.Name.Substring(0, dfValue.Field.Name.Length - 3), configs.First(p => p.Key.Equals(dfValue.ValueText)).Value);
              else result.Add(dfValue.Field.Name.Substring(0, dfValue.Field.Name.Length - 3), dfValue.ValueText);
            }
          }
        }
        return result;
      }
    }

    public IEnumerable<TicketReportDTO> GetFilteredTicketsReport(Filters filters)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        List<TicketReportDTO> ticketReportDTOs = new List<TicketReportDTO>();
        var tickets = (from ticket in db.Ticket
                       where ticket.CreateTime.CompareTo(filters.Period.startTime.ToLocalTime()) > 0
                       && ticket.CreateTime.CompareTo(filters.Period.endTime.ToLocalTime()) < 0
                       select ticket)
                      .Include(ticket => ticket.TicketState)
                      .Include(ticket => ticket.Queue)
                      .Include(ticket => ticket.TicketPriority)
                      .ToList();
        foreach (var ticket in tickets)
        {
          TicketReportDTO ticketReportDTO = new TicketReportDTO();
          ticketReportDTO.TN = ticket.Tn;
          ticketReportDTO.State = ticket.TicketState.Name;
          ticketReportDTO.CreateTime = ticket.CreateTime;
          ticketReportDTO.Client = ticket.CustomerId;
          ticketReportDTO.TicketPriority = ticket.TicketPriority.Name;
          var dFields = GetTTDynamicFields(ticket.Id);
          if (dFields.Count > 0)
          {
            foreach (var field in dFields)
            {
              typeof(TicketReportDTO).GetProperty(field.Key)?.SetValue(ticketReportDTO, field.Value);
            }
          }
          if (IsTicketDTOValid(ticketReportDTO, filters)) ticketReportDTOs.Add(ticketReportDTO);
        }
        return ticketReportDTOs.OrderByDescending(ticket => ticket.CreateTime);
      }
    }

    //Category
    private bool IsTicketDTOValid(TicketReportDTO ticket, Filters filters)
    {
      var zones = filters.Zones;
      var types = filters.Types;
      var states = filters.States;
      var initiators = filters.Initiators;
      var natInts = filters.NatInts;
      var priorities = filters.Priorities;
      var directions = filters.Directions;
      var catogories = filters.Categories;
      var problemSides = filters.ProblemSides;

      if (zones != null && zones.Length > 0 && zones.Contains(ticket.Zone) == false) return false;
      if (types != null && types.Length > 0 && types.Contains(ticket.Type) == false) return false;
      if (states != null && states.Length > 0 && states.Contains(ticket.State) == false) return false;
      if (initiators != null && initiators.Length > 0 && initiators.Contains(ticket.Initiator) == false) return false;
      if (natInts != null && natInts.Length > 0 && natInts.Contains(ticket.NatInt) == false) return false;
      if (priorities != null && priorities.Length > 0 && priorities.Contains(ticket.TicketPriority) == false) return false;
      if (directions != null && directions.Length > 0 && directions.Contains(ticket.Direction) == false) return false;
      if (catogories != null && catogories.Length > 0 && catogories.Contains(ticket.Category) == false) return false;
      if (problemSides != null && problemSides.Length > 0 && problemSides.Contains(ticket.ProblemSide) == false) return false;
      return true;
    }

    //Category
    public FilteringItems GetFilteringItems(Period period)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        List<TicketReportDTO> ticketReportDTOs = new List<TicketReportDTO>();
        var tickets = (from ticket in db.Ticket
                       where ticket.CreateTime.CompareTo(period.startTime.ToLocalTime()) > 0
                       && ticket.CreateTime.CompareTo(period.endTime.ToLocalTime()) < 0
                       select ticket)
                      .Include(ticket => ticket.TicketState)
                      .Include(ticket => ticket.Queue)
                      .Include(ticket => ticket.TicketPriority)
                      .ToList();
        var states = tickets.Select(ticket => ticket.TicketState.Name).Distinct().OrderByDescending(state => state).toSelectItems();
        var priorities = tickets.Select(ticket => ticket.TicketPriority.Name).Distinct().OrderByDescending(ticketPriority => ticketPriority).toSelectItems();
        var zones = new List<string>();
        var types = new List<string>();
        var directions = new List<string>();
        var natInts = new List<string>();
        var initiators = new List<string>();
        var categories = new List<string>();
        var problemSides = new List<string>();
        foreach (var ticket in tickets)
        {
          var dfields = GetTTDynamicFields(ticket.Id);
          if (dfields.Count > 0)
          {
            if (dfields.ContainsKey("Zone") && !String.IsNullOrEmpty(dfields["Zone"])) zones.Add(dfields["Zone"]);
            if (dfields.ContainsKey("Type") && !String.IsNullOrEmpty(dfields["Type"])) types.Add(dfields["Type"]);
            if (dfields.ContainsKey("Direction") && !String.IsNullOrEmpty(dfields["Direction"])) directions.Add(dfields["Direction"]);
            if (dfields.ContainsKey("NatInt") && !String.IsNullOrEmpty(dfields["NatInt"])) natInts.Add(dfields["NatInt"]);
            if (dfields.ContainsKey("Initiator") && !String.IsNullOrEmpty(dfields["Initiator"])) initiators.Add(dfields["Initiator"]);
            if (dfields.ContainsKey("Category") && !String.IsNullOrEmpty(dfields["Category"])) initiators.Add(dfields["Category"]);
            if (dfields.ContainsKey("ProblemSide") && !String.IsNullOrEmpty(dfields["ProblemSide"])) initiators.Add(dfields["ProblemSide"]);
          }
        }
        var filteringItems = new FilteringItems()
        {
          Directions = directions.Distinct().OrderByDescending(name => name).toSelectItems(),
          Initiators = initiators.Distinct().OrderByDescending(name => name).toSelectItems(),
          Types = types.Distinct().OrderByDescending(name => name).toSelectItems(),
          Zones = zones.Distinct().OrderByDescending(name => name).toSelectItems(),
          NatInts = natInts.Distinct().OrderByDescending(name => name).toSelectItems(),
          States = states,
          TicketPriorities = priorities,
          Categories = categories.Distinct().OrderByDescending(name => name).toSelectItems(),
          ProblemSides = problemSides.Distinct().OrderByDescending(name => name).toSelectItems()
        };
        return filteringItems;
      }
    }


    public IEnumerable<TicketReportDTO> GetTicketsReport(Period period)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        List<TicketReportDTO> ticketReportDTOs = new List<TicketReportDTO>();
        var tickets = (from ticket in db.Ticket
                       where ticket.CreateTime.CompareTo(period.startTime.ToLocalTime()) > 0
                       && ticket.CreateTime.CompareTo(period.endTime.ToLocalTime()) < 0
                       select ticket)
                      .Include(ticket => ticket.TicketState)
                      .Include(ticket => ticket.Queue)
                      .Include(ticket => ticket.TicketPriority)
                      .ToList();
        foreach (var ticket in tickets)
        {
          TicketReportDTO ticketReportDTO = new TicketReportDTO();
          ticketReportDTO.TN = ticket.Tn;
          ticketReportDTO.State = ticket.TicketState.Name;
          ticketReportDTO.CreateTime = ticket.CreateTime;
          ticketReportDTO.Client = ticket.CustomerId;
          ticketReportDTO.TicketPriority = ticket.TicketPriority.Name;
          var dFields = GetTTDynamicFields(ticket.Id);
          foreach (var field in dFields)
          {
            typeof(TicketReportDTO).GetProperty(field.Key)?.SetValue(ticketReportDTO, field.Value);
          }
          ticketReportDTOs.Add(ticketReportDTO);
        }
        return ticketReportDTOs;
      }
    }

    public long GetTicketTotalNumber(Period period)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var result = (from ticket in db.Ticket
                      where ticket.CreateTime.CompareTo(period.startTime.ToLocalTime()) > 0
                              && ticket.CreateTime.CompareTo(period.endTime.ToLocalTime()) < 0
                      select ticket).ToList().Count();

        return result;
      }
    }

    public FileStreamResult DownloadReport(Filters filters)
    {
      var report = GetFilteredTicketsReportBulk(filters).ticketReportDTOs;
      using (var workbook = new XLWorkbook())
      {
        var worksheet = workbook.AddWorksheet("TTReport");
        var currentRow = 1;
        worksheet.Cell(currentRow, 1).Value = "TN";
        worksheet.Cell(currentRow, 2).Value = "Create Time";
        worksheet.Cell(currentRow, 3).Value = "Client";
        worksheet.Cell(currentRow, 4).Value = "Problem Side";
        worksheet.Cell(currentRow, 5).Value = "Zone";
        worksheet.Cell(currentRow, 6).Value = "Type";
        worksheet.Cell(currentRow, 7).Value = "Description";
        worksheet.Cell(currentRow, 8).Value = "Initiator";
        worksheet.Cell(currentRow, 9).Value = "Direction";
        worksheet.Cell(currentRow, 10).Value = "National/International";
        worksheet.Cell(currentRow, 11).Value = "Category";
        worksheet.Cell(currentRow, 12).Value = "Priority";
        worksheet.Cell(currentRow, 13).Value = "State";
        worksheet.Cell(currentRow, 14).Value = "Close Time";


        foreach (var item in report)
        {
          currentRow++;
          worksheet.Cell(currentRow, 1).DataType = XLDataType.Number;
          worksheet.Cell(currentRow, 1).Value = item.TN;
          worksheet.Cell(currentRow, 2).Value = item.CreateTime;
          worksheet.Cell(currentRow, 3).Value = item.Client;
          worksheet.Cell(currentRow, 4).Value = item.ProblemSide;
          worksheet.Cell(currentRow, 5).Value = item.Zone;
          worksheet.Cell(currentRow, 6).Value = item.Type;
          worksheet.Cell(currentRow, 7).Value = item.Description;
          worksheet.Cell(currentRow, 8).Value = item.Initiator;
          worksheet.Cell(currentRow, 9).Value = item.Direction;
          worksheet.Cell(currentRow, 10).Value = item.NatInt;
          worksheet.Cell(currentRow, 11).Value = item.Category;
          worksheet.Cell(currentRow, 12).Value = item.TicketPriority;
          worksheet.Cell(currentRow, 13).Value = item.State;
          worksheet.Cell(currentRow, 14).Value = item.CloseTime;
        }


        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Seek(0, SeekOrigin.Begin);
        return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
      }
    }

    private bool TicketIsPending(ICollection<Article> articles)
    {
      var externalEMailes = articles
        .Where(a => a.ArticleTypeId == (short)ArticleType.ExternalEmail && a.ArticleSenderTypeId == (short)ArticalSenderType.Customer)
        .OrderByDescending(a => a.CreateTime);
      var agentAnswers = articles
        .Where(a => a.ArticleTypeId == (short)ArticleType.ExternalEmail && a.ArticleSenderTypeId == (short)ArticalSenderType.Agent)
        .OrderByDescending(a => a.CreateTime);

      if (externalEMailes.Count() > 0)
      {
        if (agentAnswers?.Count() < 2) return true;
        else if (externalEMailes?.First().CreateTime > agentAnswers?.First().CreateTime)
        {
          return true;
        }
      }
      return false;
    }

    public async Task<IEnumerable<OtrsTicketDTO>> GetPendingTickets()
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        using (var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
        {
          var ackTicketsIds = (await GetAcknowledgedTickets()).Select(x => x.Id);

          var result = await context.Ticket.AsSplitQuery().Where(tt => tt.TicketStateId == (short)State.Open && tt.QueueId != (int)TicketQueue.Trash &&
          context.DynamicFieldValue.AsSplitQuery().
                        Where(dfv => dfv.ValueText.Equals("NationalKey")).Select(dfv => dfv.ObjectId).Contains(tt.Id)
                        && !ackTicketsIds.Contains(tt.Id)).Where(tt => context.DynamicFieldValue.AsSplitQuery().
                        Where(dfv => dfv.ValueText.Equals("ClientKey")).Select(dfv => dfv.ObjectId).Contains(tt.Id))
                        .Include(tt => tt.Article).AsSplitQuery().ToListAsync();

          var otrsTicketDTOs = result.Where(tt => TicketIsPending(tt.Article)).Select(t =>
          {
            return new OtrsTicketDTO()
            {
              Id = t.Id,
              Tn = t.Tn,
              Title = t.Title,
              CreateTime = t.Article.
              Where(a => a.ArticleTypeId == (short)ArticleType.ExternalEmail && a.ArticleSenderTypeId == (short)ArticalSenderType.Customer)
              .Select(a => a.CreateTime)
              .OrderByDescending(a => a)
              .First()
              .ToUniversalTime()
            };
          }).OrderByDescending(x => x.CreateTime);

          await SavePendedTickets(otrsTicketDTOs.Select(t =>
          {
            return new PendedTicket()
            {
              TicketId = t.Id,
              Overdue = (int)((((TimeSpan)(DateTime.UtcNow - t.CreateTime)).TotalSeconds) / 60)
            };
          }).ToList());

          return otrsTicketDTOs;
        }
      }
    }

    public async Task<List<OtrsTicketDTO>> GetTickets(List<long> ids)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        using (var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
        {
          var result = await context.Ticket.AsSplitQuery().Where(t => ids.Contains(t.Id)).Include(t => t.Article).AsSplitQuery().ToListAsync();

          return result.Select(t =>
          {
            return new OtrsTicketDTO()
            {
              Id = t.Id,
              Tn = t.Tn,
              Title = t.Title,
              CreateTime = t.Article.
              Where(a => a.ArticleTypeId == (short)ArticleType.ExternalEmail && a.ArticleSenderTypeId == (short)ArticalSenderType.Customer)
              .Select(a => a.CreateTime)
              .OrderByDescending(a => a)
              .First()
              .ToUniversalTime()
            };
          }).OrderByDescending(x => x.CreateTime).ToList();
        }
      }
    }

    public async Task<IEnumerable<long>> GetClosedTicketIds(IEnumerable<long> ids)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        using (var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
        {
          return await context.Ticket.Where(t => t.TicketStateId == (int)State.Closed && ids.Contains(t.Id)).Select(t => t.Id).ToListAsync();
        }
      }
    }

    public async Task<IEnumerable<OtrsTicketDTO>> GetAcknowledgedTickets()
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        using (var context = scope.ServiceProvider.GetRequiredService<TicketDbContext>())
        {

          var acknowledgedTickets = await (from t in context.AcknowledgedTicket
                                           select t).ToListAsync();
          using (var otrsContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
          {
            var ackPendingTtIds = (await otrsContext.Ticket.Where(t => acknowledgedTickets.Select(t => t.TicketId).Contains(t.Id)).Include(t => t.Article).AsSplitQuery().ToListAsync())
              .Where(tt => IsAcknowledgedTicketInPendingState(tt.Article.ToList(), acknowledgedTickets.Single(t => t.TicketId == tt.Id))).Select(tt => tt.Id);

            var closedTtIds = await GetClosedTicketIds(acknowledgedTickets.Select(s => s.TicketId));

            context.AcknowledgedTicket.RemoveRange(acknowledgedTickets.Where(t => closedTtIds.Contains(t.TicketId) || ackPendingTtIds.Contains(t.TicketId)));
            await context.SaveChangesAsync();
            return await GetTickets(await context.AcknowledgedTicket.Select(t => t.TicketId).ToListAsync());
          }
        }
      }
    }

    public bool IsAcknowledgedTicketInPendingState(List<Article> articles, AcknowledgedTicket acknowledgedTicket)
    {
      var externalEMailes = articles
        .Where(a => a.ArticleTypeId == (short)ArticleType.ExternalEmail && a.ArticleSenderTypeId == (short)ArticalSenderType.Customer)
        .OrderByDescending(a => a.CreateTime);
      var agentAnswers = articles
        .Where(a => a.ArticleTypeId == (short)ArticleType.ExternalEmail && a.ArticleSenderTypeId == (short)ArticalSenderType.Agent)
        .OrderByDescending(a => a.CreateTime);

      var agentAnswered = agentAnswers.Count() > 0;

      if (acknowledgedTicket.CreateTime < externalEMailes.First().CreateTime
         && (agentAnswered ? externalEMailes.First().CreateTime > agentAnswers.First().CreateTime : true))
      {
        return true;
      }
      return false;
    }

    public AcknowledgedTicket GetAcknowledgedTickets(long id)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        using (var context = scope.ServiceProvider.GetRequiredService<TicketDbContext>())
        {
          var acknowledgedTicket = from t in context.AcknowledgedTicket
                                   where t.TicketId == id
                                   select t;
          return acknowledgedTicket.FirstOrDefault();
        }
      }
    }

    public IEnumerable<AcknowledgedTicket> GetAcknowledgedTickets(IEnumerable<long> ids)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        using (var context = scope.ServiceProvider.GetRequiredService<TicketDbContext>())
        {
          var acknowledgedTicket = from t in context.AcknowledgedTicket
                                   where ids.Contains(t.TicketId)
                                   select t;
          return acknowledgedTicket.ToList();
        }
      }
    }

    public async Task<List<OtrsTicketDTO>> SaveAcknowledgedTickets(IEnumerable<AcknowledgedTicket> acknowledgedTickets)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        using (var context = scope.ServiceProvider.GetRequiredService<TicketDbContext>())
        {
          var ackTts = await context.AcknowledgedTicket.ToListAsync();
          var ackTtsWithNoDuplicates = acknowledgedTickets.Where(t => !ackTts.Select(s => s.TicketId).Contains(t.TicketId));
          if (acknowledgedTickets.Count() > 0)
          {
            context.AcknowledgedTicket.AddRange(ackTtsWithNoDuplicates);
            await context.SaveChangesAsync();
          }
          return await GetTickets(acknowledgedTickets.Select(t => t.TicketId).ToList());
        }
      }
    }

    public async Task<OtrsTicketDTO> RemoveAcknowledgedTickets(long id)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        using (var context = scope.ServiceProvider.GetRequiredService<TicketDbContext>())
        {
          var ackTt = GetAcknowledgedTickets(id);
          if (ackTt != null)
          {
            context.AcknowledgedTicket.Remove(ackTt);
            await context.SaveChangesAsync();
          }
          return (await GetTickets(new long[] { id }.ToList())).FirstOrDefault();
        }
      }
    }

    public async Task<List<OtrsTicketDTO>> RemoveAcknowledgedTickets(IEnumerable<long> ids)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        using (var context = scope.ServiceProvider.GetRequiredService<TicketDbContext>())
        {
          context.AcknowledgedTicket.RemoveRange(GetAcknowledgedTickets(ids));
          await context.SaveChangesAsync();
          return await GetTickets(ids.ToList());
        }
      }
    }

    public List<LogItem> GetPendingTicketLogs()
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        using (var context = scope.ServiceProvider.GetRequiredService<LoggingDbContext>())
        {
          return context.LogItem.OrderByDescending(log => log.Time).ToList();
        }
      }
    }

    public async Task SavePendedTickets(List<PendedTicket> pendedTickets)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        using (var context = scope.ServiceProvider.GetRequiredService<TicketDbContext>())
        {
          pendedTickets.ForEach(pendedTicket =>
          {
            var ticketExists = context.PendedTicket.FirstOrDefault(t => t.TicketId == pendedTicket.TicketId);
            if (ticketExists != null && ticketExists.Overdue < 180 && pendedTicket.Overdue > 180)
            {
              ticketExists.Overdue = pendedTicket.Overdue;
              context.Update(ticketExists);
            }
            else if (ticketExists == null && pendedTicket.Overdue > 60)
            {
              context.Add(pendedTicket);
            }
          });
          await context.SaveChangesAsync();
        }
      }
    }

    public async Task<List<PendedTicketDTO>> GetPendedTickets(Period period)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        using (var context = scope.ServiceProvider.GetRequiredService<TicketDbContext>())
        {
          using (var otrsContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
          {
            var otrsTTs = otrsContext.Ticket.Where(t => t.CreateTime >= period.startTime && t.CreateTime <= period.endTime);
            var ttIds = await otrsTTs.Select(t => t.Id).ToListAsync();

            var pendedTickets = await context.PendedTicket.Where(p => ttIds.Contains(p.TicketId)).ToListAsync();
            var pendedTicketId = pendedTickets.Select(pt => pt.TicketId);

            var dynamicFields = otrsContext.DynamicFieldValue.Where(dfv => pendedTicketId.Contains(dfv.ObjectId))
              .Include(dfv => dfv.Field).AsSplitQuery();

            var result = otrsTTs.Where(t => pendedTicketId.Contains(t.Id))
              .Include(t => t.TicketState).AsSplitQuery().Include(t => t.TicketPriority).AsSplitQuery().AsEnumerable().Select(t => {
                return new PendedTicketDTO()
                {
                  TicketId = t.Id,
                  Tn = t.Tn,
                  CreateTime = t.CreateTime,
                  Client = t.CustomerId,
                  ProblemSide = dynamicFields.FirstOrDefault(d => d.ObjectId == t.Id && d.Field.Name.Equals("TicketSide"))?.ValueText,
                  Zone = dynamicFields.FirstOrDefault(d => d.ObjectId == t.Id && d.Field.Name.Equals("ZoneKey"))?.ValueText.Replace("Key",""),
                  Type = dynamicFields.FirstOrDefault(d => d.ObjectId == t.Id && d.Field.Name.Equals("TypeKey"))?.ValueText.Replace("Key", ""),
                  Description = dynamicFields.FirstOrDefault(d => d.ObjectId == t.Id && d.Field.Name.Equals("DescriptionKey"))?.ValueText.Replace("Key", ""),
                  Direction = dynamicFields.FirstOrDefault(d => d.ObjectId == t.Id && d.Field.Name.Equals("DirectionKey"))?.ValueText.Replace("Key", ""),
                  NatInt = dynamicFields.FirstOrDefault(d => d.ObjectId == t.Id && d.Field.Name.Equals("NatIntKey"))?.ValueText.Replace("Key", ""),
                  Category = dynamicFields.FirstOrDefault(d => d.ObjectId == t.Id && d.Field.Name.Equals("CategoryKey"))?.ValueText.Replace("Key", ""),
                  Initiator = dynamicFields.FirstOrDefault(d => d.ObjectId == t.Id && d.Field.Name.Equals("InitiatorKey"))?.ValueText.Replace("Key", ""),
                  TicketPriority = t.TicketPriority.Name,
                  State = t.TicketState.Name,
                  CloseTime = dynamicFields.FirstOrDefault(d => d.ObjectId == t.Id && d.ValueDate != null)?.ValueDate,
                  Overdue = pendedTickets.FirstOrDefault(pt => pt.TicketId == t.Id).Overdue
                };
            });

            return result.OrderByDescending(t => t.CreateTime).ToList();
          }
        }
      }
    }

    public int TotalPendedTickets(Period period)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        using (var context = scope.ServiceProvider.GetRequiredService<TicketDbContext>())
        {
          using (var otrsContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
          {
            var otrsIds = otrsContext.Ticket.Where(t => t.CreateTime >= period.startTime && t.CreateTime <= period.endTime).Select(t => t.Id).ToList(); 
            return context.PendedTicket.Where(p => otrsIds.Contains(p.TicketId)).Count();
          }
        }
      }
    }
  }
}
