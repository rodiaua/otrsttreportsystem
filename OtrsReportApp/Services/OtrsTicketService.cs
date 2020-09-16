﻿using System;
using System.Collections.Generic;
using System.Text;
using OtrsReportApp.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using OtrsReportApp.Models.DTO;
using OtrsReportApp.Extensions;
using ClosedXML.Excel;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper.Internal;

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

    //public IEnumerable<Ticket> GetTicketsForPeriod(Period period)
    //{
    //  using (var scope = _scopeFactory.CreateScope())
    //  {
    //    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //    var result = (from ticket in db.Ticket
    //                  where ticket.CreateTime.CompareTo(period.startTime) > 0
    //                  && ticket.CreateTime.CompareTo(period.endTime) < 0
    //                  select ticket)
    //                  .Include(ticket => ticket.TicketState)
    //                  .Include(ticket => ticket.Queue)
    //                  .ToList();

    //    return result;
    //  }

    //}

    //private IEnumerable<string> RetreiveTTDynamicFields(long ttId)
    //{
    //  using(var scope = _scopeFactory.CreateScope())
    //  {
    //    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //    var dynamicFieldsValues = (from fields in db.DynamicFieldValue
    //                               where EF.Functions.Like(fields.ObjectId.ToString(), ttId.ToString())
    //                               select fields).Include(field => field.Field).ToList();

    //    Dictionary<string, string> configs = new Dictionary<string, string>();
    //    foreach (var dfv in dynamicFieldsValues)
    //    {
    //      var config = Encoding.UTF8.GetString(dfv.Field.Config, 0, dfv.Field.Config.Length).Split("\n");
    //      foreach (var item in config)
    //      {
    //        var res = item.Split(":");
    //        if (res.Length > 1)
    //          configs.TryAdd(res[0].Trim(), res[1].TrimStart());
    //      }
    //    }
    //    foreach (var dfValue in dynamicFieldsValues)
    //    {
    //      if (dfValue.ValueText != null)
    //      {
    //        if (configs.ContainsKey(dfValue.ValueText))
    //          yield return configs.First(p => p.Key.Equals(dfValue.ValueText)).Value;
    //        else yield return dfValue.ValueText;
    //      }
    //    }
    //  }
    //}

    private Dictionary<long, Dictionary<string,string>> GetTTDynamicFieldsBulk(IEnumerable<long> ids)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var dynamicFieldsValues = db.DynamicFieldValue.Where(fields => ids.Contains(fields.ObjectId)).Include(field => field.Field).Where(field => field.Field.Name.Contains("Key")).ToList();

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
                if (configs.ContainsKey(dfValue.ValueText))
                  result.TryAdd(dfValue.Field.Name.Substring(0, dfValue.Field.Name.Length - 3), configs.First(p => p.Key.Equals(dfValue.ValueText)).Value);
                else result.TryAdd(dfValue.Field.Name.Substring(0, dfValue.Field.Name.Length - 3), dfValue.ValueText);
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
                      .ToList();

        var ttsDynamicFields = GetTTDynamicFieldsBulk((from ticket in tickets select ticket.Id).ToList());

        FilteringItems filteringItems = new FilteringItems();

        var zones = ttsDynamicFields.Select(p => p.Value).ToList().SelectValues("Zone").Distinct().ToList();
        var types = ttsDynamicFields.Select(p => p.Value).ToList().SelectValues("Type").Distinct().ToList();
        var direction = ttsDynamicFields.Select(p => p.Value).ToList().SelectValues("Direction").Distinct().ToList();
        var natInt = ttsDynamicFields.Select(p => p.Value).ToList().SelectValues("NatInt").Distinct().ToList();
        var initiator = ttsDynamicFields.Select(p => p.Value).ToList().SelectValues("Initiator").Distinct().ToList();

        filteringItems.States = tickets.Select(ticket => ticket.TicketState.Name).Distinct().OrderByDescending(state => state).toSelectItems();
        filteringItems.TicketPriorities = tickets.Select(ticket => ticket.TicketPriority.Name).Distinct().OrderByDescending(ticketPriority => ticketPriority).toSelectItems();
        filteringItems.Zones = zones.Count > 0 ? zones.toSelectItems() : null;
        filteringItems.Types = types.Count > 0 ? types.toSelectItems() : null;
        filteringItems.Directions = direction.Count > 0 ? direction.toSelectItems() : null;
        filteringItems.NatInts = natInt.Count > 0 ? natInt.toSelectItems() : null;
        filteringItems.Initiators = initiator.Count > 0 ? initiator.toSelectItems() : null;

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
              typeof(TicketReportDTO).GetProperty(field.Key)?.SetValue(ticketReportDTO, field.Value);
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
        if(configs.Count > 0) {
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
          if(dFields.Count > 0)
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

    private bool IsTicketDTOValid(TicketReportDTO ticket, Filters filters)
    {
      var zones = filters.Zones;
      var types = filters.Types;
      var states = filters.States;
      var initiators = filters.Initiators;
      var natInts = filters.NatInts;
      var priorities = filters.Priorities;
      var directions = filters.Directions;

      if (zones != null && zones.Length > 0 && zones.Contains(ticket.Zone) == false) return false;
      if (types != null && types.Length > 0 && types.Contains(ticket.Type) == false) return false;
      if (states != null && states.Length > 0 && states.Contains(ticket.State) == false) return false;
      if (initiators != null && initiators.Length > 0 && initiators.Contains(ticket.Initiator) == false) return false;
      if (natInts != null && natInts.Length > 0 && natInts.Contains(ticket.NatInt) == false) return false;
      if (priorities != null && priorities.Length > 0 && priorities.Contains(ticket.TicketPriority) == false) return false;
      if (directions != null && directions.Length > 0 && directions.Contains(ticket.Direction) == false) return false;
      return true;
    }

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
          TicketPriorities = priorities
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

    public FileStreamResult DownloadReport(Filters filters) {
      var report = GetFilteredTicketsReportBulk(filters).ticketReportDTOs;
      using (var workbook = new XLWorkbook())
      {
        var worksheet = workbook.AddWorksheet("TTReport");
        var currentRow = 1;
        worksheet.Cell(currentRow, 1).Value = "TN";
        worksheet.Cell(currentRow, 2).Value = "Create Time";
        worksheet.Cell(currentRow, 3).Value = "Client";
        worksheet.Cell(currentRow, 4).Value = "Zone";
        worksheet.Cell(currentRow, 5).Value = "Type";
        worksheet.Cell(currentRow, 6).Value = "Description";
        worksheet.Cell(currentRow, 7).Value = "Initiator";
        worksheet.Cell(currentRow, 8).Value = "Direction";
        worksheet.Cell(currentRow, 9).Value = "National/International";
        worksheet.Cell(currentRow, 10).Value = "Priority";
        worksheet.Cell(currentRow, 11).Value = "State";


        foreach(var item in report)
        {
          currentRow++;
          worksheet.Cell(currentRow, 1).DataType = XLDataType.Number;
          worksheet.Cell(currentRow, 1).Value = item.TN;
          worksheet.Cell(currentRow, 2).Value = item.CreateTime;
          worksheet.Cell(currentRow, 3).Value = item.Client;
          worksheet.Cell(currentRow, 4).Value = item.Zone;
          worksheet.Cell(currentRow, 5).Value = item.Type;
          worksheet.Cell(currentRow, 6).Value = item.Description;
          worksheet.Cell(currentRow, 7).Value = item.Initiator;
          worksheet.Cell(currentRow, 8).Value = item.Direction;
          worksheet.Cell(currentRow, 9).Value = item.NatInt;
          worksheet.Cell(currentRow, 10).Value = item.TicketPriority;
          worksheet.Cell(currentRow, 11).Value = item.State;
        }


        var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Seek(0, SeekOrigin.Begin);
        return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
      }
    }
  }
}