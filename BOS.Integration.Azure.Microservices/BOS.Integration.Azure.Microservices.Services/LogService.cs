using AutoMapper;
using BOS.Integration.Azure.Microservices.DataAccess.Abstraction.Repositories;
using BOS.Integration.Azure.Microservices.Domain.DTOs;
using BOS.Integration.Azure.Microservices.Domain.Entities;
using BOS.Integration.Azure.Microservices.Services.Abstraction;
using System;
using System.Collections.Generic;
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

            newTimeLine.DateTime = DateTime.Now.ToString("yyyyMMdd hh:mm:ss");
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

                newTimeLine.DateTime = timeLine.DateTime.ToString("yyyyMMdd hh:mm:ss");
                newTimeLine.Description = timeLine.Description;
                newTimeLine.Status = timeLine.Status;

                newTimeLines.Add(newTimeLine);
            }

            await timeLineRepository.AddRangeAsync(newTimeLines, erpInfo.Object);
        }
    }
}
