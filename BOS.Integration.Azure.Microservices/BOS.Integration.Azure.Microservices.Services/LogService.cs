using AutoMapper;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.Entities;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using BOS.Integration.Azure.Microservices.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BOS.Integration.Azure.Microservices.Services
{
    public class LogService : ILogService
    {
        private readonly IErpMessageRepository erpMessageRepository;
        private readonly ITimeLineRepository timeLineRepository;
        private readonly IMapper mapper;

        public LogService(IErpMessageRepository erpMessageRepository, ITimeLineRepository timeLineRepository, IMapper mapper)
        {
            this.erpMessageRepository = erpMessageRepository;
            this.timeLineRepository = timeLineRepository;
            this.mapper = mapper;
        }

        public async Task<List<TimeLine>> GetTimeLinesByFilterAsync(TimeLineRequestDTO timeLineRequest)
        {
            timeLineRequest.Objects = timeLineRequest.Objects?.Select(x => x.ToLower())?.ToList() ?? new List<string>();
            timeLineRequest.Statuses = timeLineRequest.Statuses?.Select(x => x.ToLower())?.ToList() ?? new List<string>();

            timeLineRequest.FromDate ??= DateTime.MinValue;
            timeLineRequest.ToDate ??= DateTime.MaxValue;

            var timeLines = await timeLineRepository.GetByFilterAsync(timeLineRequest);

            return timeLines.Where(t => DateHelper.ConvertStringToDateTime(t.DateTime) > timeLineRequest.FromDate
                                            && DateHelper.ConvertStringToDateTime(t.DateTime) < timeLineRequest.ToDate).ToList();
        }

        public async Task AddErpMessageAsync(LogInfo erpInfo, string status)
        {
            var newErpMessage = this.mapper.Map<ErpMessage>(erpInfo);

            newErpMessage.Status = status;

            await erpMessageRepository.AddAsync(newErpMessage, erpInfo.Object);
        }

        public async Task AddErpMessagesAsync(LogInfo erpInfo, ICollection<string> statuses)
        {
            var erpMessages = new List<ErpMessage>();

            foreach (var status in statuses)
            {
                var newErpMessage = this.mapper.Map<ErpMessage>(erpInfo);

                newErpMessage.Status = status;

                erpMessages.Add(newErpMessage);
            }

            await erpMessageRepository.AddRangeAsync(erpMessages, erpInfo.Object);
        }

        public async Task AddTimeLineAsync(LogInfo erpInfo, string description, string status)
        {
            var newTimeLine = this.mapper.Map<TimeLine>(erpInfo);

            newTimeLine.DateTime = DateHelper.ConvertDateTimeToString(DateTime.Now);
            newTimeLine.Description = description;
            newTimeLine.Status = status;

            await timeLineRepository.AddAsync(newTimeLine, erpInfo.Object);
        }

        public async Task AddTimeLinesAsync(LogInfo erpInfo, ICollection<TimeLineDTO> timeLines)
        {
            var newTimeLines = new List<TimeLine>();

            foreach (var timeLine in timeLines)
            {
                var newTimeLine = this.mapper.Map<TimeLine>(erpInfo);

                newTimeLine.DateTime = DateHelper.ConvertDateTimeToString(timeLine.DateTime);
                newTimeLine.Description = timeLine.Description;
                newTimeLine.Status = timeLine.Status;

                newTimeLines.Add(newTimeLine);
            }

            await timeLineRepository.AddRangeAsync(newTimeLines, erpInfo.Object);
        }
    }
}
