using StationDiagnosticService.Models;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace StationDiagnosticService.Services
{
    internal class DiagnosticStackManager : IDiagnosticStackManager
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DiagnosticStackManager> _logger;
        private ConcurrentStack<DataFrame> _dataFrameStack;

        public DiagnosticStackManager(IConfiguration configuration, ILogger<DiagnosticStackManager> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _dataFrameStack = new ConcurrentStack<DataFrame>();
        }



        public void PrepareJsonFrame(string topic, JsonObject jsonData,DateTime date)
        {
            
          var dataFrame = new DataFrame 
          {
              jsonData = jsonData,
              frameDate = date.ToString("yyyy-MM-dd HH:mm:ss.ffffff"),
              dataType = "Json",
              dataSource = topic
          };

          PushFrame(dataFrame);
        }


        public  void PrepareTextFrame(string topic, string textData, DateTime date)
        {
            var dataFrame = new DataFrame
            {
                textData = textData,
                frameDate = date.ToString("yyyy-MM-dd HH:mm:ss.ffffff"),
                dataType= "text",
                dataSource=topic
               
                

            };


            PushFrame(dataFrame);

        }


        private void PushFrame(DataFrame dataFrame)
        {
            _dataFrameStack.Push(dataFrame);
            _logger.LogInformation($"Data pushed to stack, current stack count is {GetStackCount()}");
        }


        public int GetStackCount() {  return _dataFrameStack.Count; }




        public bool TryPopFrame(out DataFrame dataFrame)
        {
            return _dataFrameStack.TryPop(out dataFrame);
        }

 
    }
}
