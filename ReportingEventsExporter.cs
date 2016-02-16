using System;
using System.Linq;

using net_coolblue_datastore_clients.RavenDb;

namespace ApplicationReportingDataDownloader
{
    public class ReportingEventsExporter
    {
        private readonly IRavenRepository<ReportingEventDataEntity, int> _reportingEventRepository;
        private readonly EventExcelExporter _excelExporter;

        public ReportingEventsExporter(IRavenRepository<ReportingEventDataEntity, int> reportingEventRepository, EventExcelExporter excelExporter)
        {
            _reportingEventRepository = reportingEventRepository;
            _excelExporter = excelExporter;
        }

        public ExportResult Export(DateTime fromDate, DateTime toDate, string fileName)
        {
            var events =
                _reportingEventRepository.GetAll()
                                         .Where(evt => evt.EventTime >= fromDate && evt.EventTime <= toDate)
                                         .ToList();

            if(!events.Any())
                return ExportResult.NoEvents;

            var eventsPerName = events
                .GroupBy(evt => evt.Name)
                .ToDictionary(evt => evt.Key, evts => evts.ToList());

            var exported = _excelExporter.Export(eventsPerName, fileName);

            if(exported)
                return ExportResult.Ok;
            else
                return ExportResult.Error;
        }
    }
}